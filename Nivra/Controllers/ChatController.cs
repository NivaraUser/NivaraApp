using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Nivara.Common.Constants;
using Nivara.Core.ChatModule;
using Nivara.Core.Common;
using Nivara.Core.CompanyDetail;
using Nivara.Core.CompanyRole;
using Nivara.Core.Employee;
using Nivara.Core.EndUser;
using Nivara.Core.UsersTask;
using Nivara.Models;
using Nivara.Web.ChatHub;
using Nivara.Web.ChatHub.UserConnection;
using Nivara.Web.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;


namespace Nivara.Web.Controllers
{
    public partial class ChatController : BaseController
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ICompaniesServices _companiesServices;
        private readonly ICommonServices _commonServices;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEmployeeServices _employeeServices;
        private readonly ICompanyRolesServices _companyRolesServices;
        private readonly IChatService _chatService;
        private readonly IEndUserService _endUserService;
        private readonly IUsersTaskService _userTaskService;
        private readonly IHubContext<MessengerHub> _notificationUserHubContext;
        private readonly IUserConnectionManager _userConnectionManager;

        public ChatController(SignInManager<IdentityUser> signInManager, ICompaniesServices companiesServices, ICommonServices commonServices,IWebHostEnvironment hostEnvironment, IEmployeeServices employeeServices,
            ICompanyRolesServices companyRolesServices,IChatService chatService, IEndUserService endUserService,IUsersTaskService usersTaskService, IHubContext<MessengerHub> notificationUserHubContext
            , IUserConnectionManager userConnectionManager)
        {
            _signInManager = signInManager;
            _companiesServices = companiesServices;
            _commonServices = commonServices;
            _webHostEnvironment = hostEnvironment;
            _employeeServices = employeeServices;
            _companyRolesServices = companyRolesServices;
            _chatService = chatService;
            _endUserService = endUserService;
            _userTaskService = usersTaskService;
            _notificationUserHubContext = notificationUserHubContext;
            _userConnectionManager = userConnectionManager;
        }

        //public async Task<IActionResult> LoadTaskChat(int TaskId, string ReceiverUserId)
        //{
        //    try
        //    {
        //        if (TaskId > 0 && !string.IsNullOrEmpty(ReceiverUserId))
        //        {
        //            var GetSenderUserId = HttpContext.Session.GetComplexData<string>(SessionConstants.UserId);

        //            var GetReceiverUser = await _endUserService.GetEndUserByUserId(ReceiverUserId);
        //            if (GetReceiverUser != null && !string.IsNullOrEmpty(GetReceiverUser.ProfilePiture))
        //                GetReceiverUser.ProfilePiture = "/images/" + GetReceiverUser.ProfilePiture;
        //            else
        //                GetReceiverUser.ProfilePiture ="/assets/images/users/avatar-1.jpg";

        //            var ChatHistory = await _chatService.GetChatByTaskId(TaskId, GetSenderUserId);
        //            return Json(new { status = "success", ChatHistory ,Name= GetReceiverUser.FirstName+" "+ GetReceiverUser.LastName, ProfilePiture= GetReceiverUser.ProfilePiture });
        //        }
        //        return Json(new { status = "error", msg = "Invalid Details"});
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { status = "error", msg=ex.Message});
        //    }

        //}


        public async Task<IActionResult> LoadCompanyTaskChat_Old(int TaskId, string ReceiverUserId)
        {
            try
            {
                if (TaskId > 0 && !string.IsNullOrEmpty(ReceiverUserId))
                {
                    var GetSenderUserId = HttpContext.Session.GetComplexData<string>(SessionConstants.UserId);
                    var GetReceiverUser = await _employeeServices.GetEmployeeByUserId(ReceiverUserId);

                    if (GetReceiverUser != null && !string.IsNullOrEmpty(GetReceiverUser.ProfilePicture))
                        GetReceiverUser.ProfilePicture = "/images/" + GetReceiverUser.ProfilePicture;
                    else
                        GetReceiverUser.ProfilePicture = "/assets/images/users/avatar-1.jpg";

                    var ChatHistory = await _chatService.GetChatByTaskId(TaskId, GetSenderUserId, ReceiverUserId);
                    return Json(new { status = "success", ChatHistory, Name = GetReceiverUser.FirstName + " " + GetReceiverUser.LastName, ProfilePiture = GetReceiverUser.ProfilePicture });
                }
                return Json(new { status = "error", msg = "Invalid Details" });
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", msg = ex.Message });
            }
        }

        public async Task<IActionResult> LoadCompanyTaskChat(int TaskId, string ReceiverUserId)
        {
            try
            {
                if (TaskId > 0 && !string.IsNullOrEmpty(ReceiverUserId))
                {
                    var GetSenderUserId = HttpContext.Session.GetComplexData<string>(SessionConstants.UserId);
                    var chatModelSenderReceiverDetails = new List<ChatModelSenderReceiverDetail>();
                    var GetReceiverUserEmployee = await _chatService.GetEmployeeDetailsByTaskId(TaskId);
                    if (GetReceiverUserEmployee != null)
                    {
                        chatModelSenderReceiverDetails.Add(new Models.ChatModelSenderReceiverDetail
                        {
                            ReceiverUserId = GetReceiverUserEmployee.AspNetUserId,
                            ReceiverUserName = GetReceiverUserEmployee.FirstName + " " + GetReceiverUserEmployee.LastName,
                            ProfilePiture = string.IsNullOrEmpty(GetReceiverUserEmployee.ProfilePicture) ? "/assets/images/users/avatar-1.jpg" : "/images/" + GetReceiverUserEmployee.ProfilePicture
                        });
                    }
                    var getReceiverUserAdmin = await _chatService.GetAdminDetails();
                    if (getReceiverUserAdmin != null)
                    {
                        chatModelSenderReceiverDetails.Add(new Models.ChatModelSenderReceiverDetail
                        {
                            ReceiverUserId = getReceiverUserAdmin.AspNetUserId,
                            ReceiverUserName = getReceiverUserAdmin.Name,
                            ProfilePiture = string.IsNullOrEmpty(getReceiverUserAdmin.ProfilePicture) ? "/assets/images/users/avatar-1.jpg" : "/images/" + getReceiverUserAdmin.ProfilePicture
                        });
                    }

                    var ChatHistory = await _chatService.GetChatByTaskId(TaskId, GetSenderUserId, ReceiverUserId);
                    return Json(new { status = "success", ChatHistory, senderReceiverDetails = chatModelSenderReceiverDetails });
                }
                return Json(new { status = "error", msg = "Invalid Details" });
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", msg = ex.Message });
            }
        }

        public async Task<IActionResult> LoadAdminTaskChat_Old(int TaskId, string ReceiverUserId)
        {
            try
            {
                if (TaskId > 0 && !string.IsNullOrEmpty(ReceiverUserId))
                {
                    var GetSenderUserId = HttpContext.Session.GetComplexData<string>(SessionConstants.UserId);

                    var GetReceiverUser = await _employeeServices.GetEmployeeByUserId(ReceiverUserId);
                    if (GetReceiverUser != null && !string.IsNullOrEmpty(GetReceiverUser.ProfilePicture))
                        GetReceiverUser.ProfilePicture = "/images/" + GetReceiverUser.ProfilePicture;
                    else
                        GetReceiverUser.ProfilePicture = "/assets/images/users/avatar-1.jpg";

                    var ChatHistory = await _chatService.GetChatByTaskId(TaskId, GetSenderUserId, ReceiverUserId);
                    return Json(new { status = "success", ChatHistory, Name = GetReceiverUser.FirstName + " " + GetReceiverUser.LastName, ProfilePiture = GetReceiverUser.ProfilePicture });
                }
                return Json(new { status = "error", msg = "Invalid Details" });
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", msg = ex.Message });
            }

        }

        public async Task<IActionResult> LoadAdminTaskChat(int TaskId, string ReceiverUserId)
        {
            try
            {
                if (TaskId > 0 && !string.IsNullOrEmpty(ReceiverUserId))
                {
                    var GetSenderUserId = HttpContext.Session.GetComplexData<string>(SessionConstants.UserId);

                    var GetReceiverUserEmployee = await _chatService.GetEmployeeDetailsByTaskId(TaskId);
                    var GetReceiverUserCompany = await _chatService.GetCompanyDetailsByTaskId(TaskId);
                    var chatModelSenderReceiverDetails = new List<ChatModelSenderReceiverDetail>();
                    if (GetReceiverUserEmployee != null)
                    {
                        chatModelSenderReceiverDetails.Add(new Models.ChatModelSenderReceiverDetail
                        {
                            ReceiverUserId = GetReceiverUserEmployee.AspNetUserId,
                            ReceiverUserName = GetReceiverUserEmployee.FirstName + " " + GetReceiverUserEmployee.LastName,
                            ProfilePiture = string.IsNullOrEmpty(GetReceiverUserEmployee.ProfilePicture) ? "/assets/images/users/avatar-1.jpg" : "/images/" + GetReceiverUserEmployee.ProfilePicture
                        });
                    }
                    if (GetReceiverUserCompany != null)
                    {
                        chatModelSenderReceiverDetails.Add(new Models.ChatModelSenderReceiverDetail
                        {
                            ReceiverUserId = GetReceiverUserCompany.AspNetUserId,
                            ReceiverUserName = GetReceiverUserCompany.Name,
                            ProfilePiture = string.IsNullOrEmpty(GetReceiverUserCompany.ProfilePicture) ? "/assets/images/users/avatar-1.jpg" : "/images/" + GetReceiverUserCompany.ProfilePicture
                        });
                    }
                    var ChatHistory = await _chatService.GetChatByTaskId(TaskId, GetSenderUserId, ReceiverUserId);
                    return Json(new { status = "success", ChatHistory, senderReceiverDetails = chatModelSenderReceiverDetails });
                }
                return Json(new { status = "error", msg = "Invalid Details" });
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", msg = ex.Message });
            }
        }

        public async Task<IActionResult> LoadEmployeeTaskChat_Old(int TaskId, string ReceiverUserId)
        {
            try
            {
                if (TaskId > 0)
                {
                    var GetSenderUserId = HttpContext.Session.GetComplexData<string>(SessionConstants.UserId);
                    var ChatHistory = await _chatService.GetChatByTaskId(TaskId, GetSenderUserId, ReceiverUserId);
                    ///var GetAssgnedEmp = await _userTaskService.GetAssignedEmpUserByTaskId(TaskId);

                    var GetReceiverUser = await _companiesServices.GetCompanyByAspNetUserId(ReceiverUserId);

                    if (GetReceiverUser != null && !string.IsNullOrEmpty(GetReceiverUser.ProfilePiture))
                        GetReceiverUser.ProfilePiture = "/images/" + GetReceiverUser.ProfilePiture;
                    else
                        GetReceiverUser.ProfilePiture = "/assets/images/users/avatar-1.jpg";
                   
                    return Json(new { status = "success", ChatHistory, Name = GetReceiverUser.Name, ProfilePiture = GetReceiverUser.ProfilePiture });
                }
                return Json(new { status = "error", msg = "Invalid Details" });
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", msg = ex.Message });
            }

        }

        public async Task<IActionResult> LoadEmployeeTaskChat(int TaskId, string ReceiverUserId)
        {
            try
            {
                if (TaskId > 0 && !string.IsNullOrEmpty(ReceiverUserId))
                {
                    var GetSenderUserId = HttpContext.Session.GetComplexData<string>(SessionConstants.UserId);

                    var chatModelSenderReceiverDetails = new List<ChatModelSenderReceiverDetail>();
                    var ChatHistory = await _chatService.GetChatByTaskId(TaskId, GetSenderUserId, ReceiverUserId);
                    var getReceiverUserAdmin = await _chatService.GetAdminDetails();
                    if (getReceiverUserAdmin != null)
                    {
                        chatModelSenderReceiverDetails.Add(new Models.ChatModelSenderReceiverDetail
                        {
                            ReceiverUserId = getReceiverUserAdmin.AspNetUserId,
                            ReceiverUserName = getReceiverUserAdmin.Name,
                            ProfilePiture = string.IsNullOrEmpty(getReceiverUserAdmin.ProfilePicture) ? "/assets/images/users/avatar-1.jpg" : "/images/" + getReceiverUserAdmin.ProfilePicture
                        });
                    }
                    var getReceiverUserCompany = await _chatService.GetCompanyDetailsByTaskId(TaskId);
                    if (getReceiverUserCompany != null)
                    {
                        chatModelSenderReceiverDetails.Add(new Models.ChatModelSenderReceiverDetail
                        {
                            ReceiverUserId = getReceiverUserCompany.AspNetUserId,
                            ReceiverUserName = getReceiverUserCompany.Name,
                            ProfilePiture = string.IsNullOrEmpty(getReceiverUserCompany.ProfilePicture) ? "/assets/images/users/avatar-1.jpg" : "/images/" + getReceiverUserCompany.ProfilePicture
                        });
                    }
                    return Json(new { status = "success", ChatHistory, senderReceiverDetails = chatModelSenderReceiverDetails });
                }
                return Json(new { status = "error", msg = "Invalid Details" });
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", msg = ex.Message });
            }
        }

        public async Task<IActionResult> GetChatHistory(int TaskId, string ReceiverUserId)
        {
            var senderUserId = HttpContext.Session.GetComplexData<string>(SessionConstants.UserId);
            var ChatHistory = await _chatService.GetChatByTaskId(TaskId, senderUserId, ReceiverUserId);
            return Json(new { status = "success", ChatHistory});
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
                            var connections = _userConnectionManager.GetUserConnections(model.ReceiverUserId);
                            if (connections != null && connections.Count > 0)
                            {
                                foreach (var connectionId in connections)
                                {
                                    await _notificationUserHubContext.Clients.Client(connectionId).SendAsync("sendDocToUser", model.Message, model.Document);
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
