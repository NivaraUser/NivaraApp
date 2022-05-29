using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Nivara.Data.Models;
using Nivara.Data.Models.NivaraDbContext;
using Nivara.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace Nivara.Core.ChatModule
{
    public class ChatService : IChatService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly NivaraDbContext context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ChatService(UserManager<IdentityUser> userManager, NivaraDbContext nivaraDb, IWebHostEnvironment hostEnvironment)
        {
            _userManager = userManager;
            context = nivaraDb;
            _webHostEnvironment = hostEnvironment;
        }

        public async Task<List<ChatModel>> GetChatByTaskId(int UsersTaskId, string SenderUserId, string receiverUserID)
        {
            var chatHistory = new List<ChatModel>();
            ChatModel newChat;
            var Chat =await context.Chats.Where(f =>f.UsersTaskId== UsersTaskId && ((f.SenderUserId == SenderUserId && f.ReceiverUserId == receiverUserID) || (f.SenderUserId == receiverUserID && f.ReceiverUserId == SenderUserId))).ToListAsync();
            foreach (var item in Chat.OrderBy(f => f.CreatedOn))
            {
                newChat=new ChatModel();
                newChat.Message = item.Message;
                newChat.UsersTaskId = item.UsersTaskId;
                newChat.SenderUserId = item.SenderUserId;
                newChat.ReceiverUserId = item.ReceiverUserId;
                newChat.Document = !string.IsNullOrEmpty(item.DocumentPath)? "/ChatDocuments/"+item.DocumentPath:"";
                if (!string.IsNullOrEmpty(item.DocumentPath))
                    newChat.IsDocument = true;
                if (SenderUserId == item.SenderUserId)
                    newChat.IsSent = true;
                chatHistory.Add(newChat);
            }
            return chatHistory;
        }
        
        public async Task<bool> CreateChat(ChatModel chat)
        {
            var result = false;
            using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(30), TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    Chat newChat = new Chat();
                    newChat.Message = chat.Message;
                    newChat.SenderUserId = chat.SenderUserId;
                    newChat.ReceiverUserId = chat.ReceiverUserId;
                    newChat.CreatedOn = DateTime.UtcNow;
                    newChat.UsersTaskId = chat.UsersTaskId;
                    newChat.DocumentPath = chat.Document;
                    await context.Chats.AddAsync(newChat);
                    await context.SaveChangesAsync();
                    result = true;
                    transaction.Complete();
                    
                }
                catch (Exception ex)
                {
                    transaction.Dispose();
                    throw new Exception(ex.Message);
                }
            }
            return result;
        }

        public string UploadedChatFile(ChatModel model)
        {
            string uniqueFileName = null;
            if (model.ChatDocument != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "ChatDocuments");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ChatDocument.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.ChatDocument.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }

        //---------- For DB Details-------------------
        public async Task<CompanyModel> GetCompanyDetailsByTaskId(int taskId)
        {
            var userTasks = await context.UsersTask.FirstOrDefaultAsync(x => x.Id == taskId && x.IsDeleted == false);
            if (userTasks == null)
            {
                return new CompanyModel();
            }

            return await (from comp in context.Companies.Where(c => c.Id == userTasks.CompanyId)
                          join city in context.Cities on comp.CityId equals city.Id
                          join state in context.States on city.StateId equals state.Id
                          join aspNetUser in context.Users on comp.AspNetUserId equals aspNetUser.Id
                          select new CompanyModel()
                          {
                              Id = comp.Id,
                              Name = comp.Name,
                              Website = comp.Website,
                              Email = aspNetUser.Email,
                              PhoneNo = comp.PhoneNo,
                              PostalCode = comp.PostalCode,
                              CityId = comp.CityId,
                              Address = comp.Address,
                              ProfilePicture = comp.ProfilePiture,
                              IsAdmin = comp.IsAdmin,
                              AspNetUserId = aspNetUser.Id
                          }).FirstOrDefaultAsync();
        }
        public async Task<CompanyModel> GetAdminDetails()
        {

            return await (from comp in context.Companies.Where(c => c.IsAdmin == true)
                          join city in context.Cities on comp.CityId equals city.Id
                          join aspNetUser in context.Users on comp.AspNetUserId equals aspNetUser.Id
                          select new CompanyModel()
                          {
                              Id = comp.Id,
                              Name = comp.Name,
                              Website = comp.Website,
                              Email = aspNetUser.Email,
                              PhoneNo = comp.PhoneNo,
                              PostalCode = comp.PostalCode,
                              CityId = comp.CityId,
                              Address = comp.Address,
                              ProfilePicture = comp.ProfilePiture,
                              IsAdmin = comp.IsAdmin,
                              AspNetUserId = aspNetUser.Id
                          }).FirstOrDefaultAsync();
        }
        public async Task<EmployeeModel> GetEmployeeDetailsByTaskId(int taskId)
        {
            var employee = await context.EmployeesTasks.FirstOrDefaultAsync(x => x.TaskId == taskId && x.IsDeleted == false);
            if (employee == null)
            {
                return new EmployeeModel();
            }
            return await (from emp in context.Employees.Where(x => x.Id == employee.EmployeeId)
                          join city in context.Cities on emp.CityId equals city.Id
                          join state in context.States on city.StateId equals state.Id
                          join aspNetUser in context.Users on emp.AspNetUserId equals aspNetUser.Id
                          select new EmployeeModel()
                          {
                              Id = emp.Id,
                              Prefix = emp.Prefix,
                              FirstName = emp.FirstName,
                              LastName = emp.LastName,
                              Email = aspNetUser.Email,
                              ContactNumber = emp.PhoneNumber,
                              PostalCode = emp.PostalCode,
                              CityId = emp.CityId,
                              StateId = city.StateId,
                              CountryId = state.CounteryId,
                              UserName = emp.FirstName + " " + emp.LastName,
                              Address = emp.Address,
                              ProfilePicture = emp.ProfilePiture,
                              AspNetUserId = emp.AspNetUserId
                          }).FirstOrDefaultAsync();
        }
    }
}
