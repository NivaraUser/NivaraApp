
using Microsoft.AspNetCore.Mvc.Rendering;
using Nivara.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nivara.Core.ChatModule
{
    public interface IChatService
    {
        Task<bool> CreateChat(ChatModel chat);
        Task<List<ChatModel>> GetChatByTaskId(int UsersTaskId, string SenderUserId, string receiverUserId);
        string UploadedChatFile(ChatModel model);

        //---------- For DB Details-------------------

        Task<CompanyModel> GetCompanyDetailsByTaskId(int taskId);
        Task<CompanyModel> GetAdminDetails();
        Task<EmployeeModel> GetEmployeeDetailsByTaskId(int taskId);
    }
}
