using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Nivara.Common.Constants;
using Nivara.Core.ChatModule;
using Nivara.Core.Employee;
using Nivara.Core.EndUser;
using Nivara.Core.UsersTask;
using Nivara.Models;
using Nivara.Web.ChatHub;
using Nivara.Web.ChatHub.UserConnection;
using Nivara.Web.Controllers;
using Nivara.Web.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Nivara.Web.Areas.User.Controllers
{
    [Area("User")]
    public class ChatController : Controller
    {

        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IChatService _chatService;
        private readonly IEndUserService _endUserService;
        private readonly IUsersTaskService _userTaskService;
        private readonly IUserConnectionManager _userConnectionManager;
        private readonly IHubContext<MessengerHub> _notificationUserHubContext;
        private readonly IEmployeeServices _employeeServices;
        public ChatController(IWebHostEnvironment hostEnvironment, IChatService chatService, IEndUserService endUserService, IUsersTaskService usersTaskService,
            IUserConnectionManager userConnectionManager, IHubContext<MessengerHub> notificationUserHubContext, IEmployeeServices employeeServices)
        {
           
            _webHostEnvironment = hostEnvironment;
            _chatService = chatService;
            _endUserService = endUserService;
            _userTaskService = usersTaskService;
            _userConnectionManager = userConnectionManager;
            _notificationUserHubContext = notificationUserHubContext;
            _employeeServices = employeeServices;
        }
        public async Task<IActionResult> LoadEndUserTaskChat(int TaskId)
        {
            try
            {
                if (TaskId > 0)
                {
                    var GetSenderUserId = HttpContext.Session.GetComplexData<string>(SessionConstants.UserId);
                    var ChatHistory = await _chatService.GetChatByTaskId(TaskId, GetSenderUserId, string.Empty);
                    var GetAssgnedEmp = await _userTaskService.GetAssignedEmpUserByTaskId(TaskId);

                    var GetReceiverUser = await _employeeServices.GetEmployeeByUserId(GetAssgnedEmp);
                    if (GetReceiverUser != null && !string.IsNullOrEmpty(GetReceiverUser.ProfilePicture))
                        GetReceiverUser.ProfilePicture= "/images/" + GetReceiverUser.ProfilePicture;
                    else
                        GetReceiverUser.ProfilePicture= "/assets/images/users/avatar-1.jpg";

                    return Json(new { status = "success", ChatHistory, AssignedEmpUserId = GetAssgnedEmp, Name = GetReceiverUser.FirstName + " " + GetReceiverUser.LastName, ProfilePiture = GetReceiverUser.ProfilePicture });
                }
                return Json(new { status = "error", msg = "Invalid Details" });
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", msg = ex.Message });
            }

        }

        private string UploadedFile(IFormFile ProfileImage)
        {
            string uniqueFileName = null;

            if (ProfileImage != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + ProfileImage.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    ProfileImage.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }

        public async Task<IActionResult> UploadChatDocument()
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (Request.HasFormContentType)
                    {
                        var form = Request.Form;
                        var model = new ChatModel();
                        foreach (var formFile in form.Files)
                        {
                            model = new ChatModel();
                            model.SenderUserId = form["senderid"].ToString();
                            model.ReceiverUserId = form["receiverid"].ToString();
                            model.Message = form["chatmessage"].ToString();
                            model.UsersTaskId = Convert.ToInt32(form["usertaskid"]);
                            model.ChatDocument = formFile;
                            model.Document = _chatService.UploadedChatFile(model);
                            await _chatService.CreateChat(model);
                            model.Document = "/ChatDocuments/" + model.Document;
                            model.IsDocument = true;
                            var connections = _userConnectionManager.GetUserConnections(model.ReceiverUserId);
                            if (connections != null && connections.Count > 0)
                            {
                                foreach (var connectionId in connections)
                                {
                                    await _notificationUserHubContext.Clients.Client(connectionId).SendAsync("sendDocToUser", model.Message,model.Document);
                                }
                            }
                        }
                    }

                }
                return Json(new
                {
                    status = "success",
                });
            }
            catch (Exception ex)
            {

                var msg = ex.Message;
                return Json(new
                {
                    status = "error",
                });
            }


        }

    }
}
