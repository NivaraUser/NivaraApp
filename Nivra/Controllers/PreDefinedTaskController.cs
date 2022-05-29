using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nivara.Common.Constants;
using Nivara.Core.Common;
using Nivara.Core.CompanyDetail;
using Nivara.Core.Employee;
using Nivara.Core.PreDefinedTask;
using Nivara.Core.UsersTask;
using Nivara.Models;
using Nivara.Web.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Nivara.Web.Controllers
{
    public class PreDefinedTaskController : BaseController
    {
        private readonly IPreDefinedTaskService preDefinedTaskService;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly ICompaniesServices companiesServices;
        private readonly IEmployeeServices employeeServices;
        private readonly ICommonServices commonServices;
        private readonly IUsersTaskService usersTaskService;
        public PreDefinedTaskController(IPreDefinedTaskService _preDefinedTaskService, ICompaniesServices _companiesServices, IWebHostEnvironment _webHostEnvironment, IHostingEnvironment _hostingEnvironment, IEmployeeServices _employeeServices, ICommonServices _commonServices, IUsersTaskService _usersTaskService)
        {
            preDefinedTaskService = _preDefinedTaskService;
            webHostEnvironment = _webHostEnvironment;
            hostingEnvironment = _hostingEnvironment;
            companiesServices = _companiesServices;
            employeeServices = _employeeServices;
            commonServices = _commonServices;
            usersTaskService = _usersTaskService;
        }
        public async Task<IActionResult> Index()
        {
            ShowPassMessage();
            var companyId = HttpContext.Session.GetComplexData<int>(SessionConstants.CompanyId);
            var employeeId = HttpContext.Session.GetComplexData<int>(SessionConstants.EmployeeId);
            var company = companiesServices.GetCompanyById(companyId).Result;
            ViewBag.CompanyId = companyId;
            ViewBag.EmployeeId = employeeId;
            ViewBag.IsAdmin = company.IsAdmin;
            var model = new PreDefinedTaskViewModel();
            model.PreDefinedTasks = await preDefinedTaskService.GetAllPreDefinedTask(new PreDefinedTaskSearchParameters() { SearchText = string.Empty });
            int IsAdmin=0;int IsClient=0; int IsEmployee=0;
            // For Admin
            if (ViewBag.IsAdmin == true && ViewBag.EmployeeId == 0 && ViewBag.CompanyId != 0)
            {
                IsAdmin = 1;
            }
            // For Client /Company
            else if (ViewBag.IsAdmin == false && ViewBag.EmployeeId == 0 && ViewBag.CompanyId != 0)
            {
                IsClient = 1;
            }
            // For Employee
            else if (ViewBag.IsAdmin == true && ViewBag.EmployeeId != 0 && ViewBag.CompanyId != 0)
            {
                IsEmployee = 1;
            }
            var preDefinedNotificationTasks = await preDefinedTaskService.GetNotification(IsAdmin, IsClient, IsEmployee);
            preDefinedNotificationTasks.AddRange(await usersTaskService.GetNotification(IsAdmin, IsClient, IsEmployee));
            ViewBag.PreDefinedNotificationTasks = preDefinedNotificationTasks;
            //model.PreDefinedTasks.ForEach(c =>
            //{
            //    c.ServiceDescription = c.ServiceDescription.Length > 50 ? c.ServiceDescription.Substring(0, 49) : c.ServiceDescription;
            //    string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "PreDefinedTaskLogo");
            //    c.LogoName = uploadsFolder + "\\" + c.LogoName;
            //});
            model.UserName = User.Identity.Name;
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Manage(int? id)
        {
            PreDefinedTaskModel model = new PreDefinedTaskModel();
            var companyId = HttpContext.Session.GetComplexData<int>(SessionConstants.CompanyId);
            var employeeId = HttpContext.Session.GetComplexData<int>(SessionConstants.EmployeeId);
            var company = companiesServices.GetCompanyById(companyId).Result;
            ViewBag.CompanyId = companyId;
            ViewBag.EmployeeId = employeeId;
            ViewBag.IsAdmin = company.IsAdmin;

            if (id > 0)
            {
                model = await preDefinedTaskService.GetPreDefinedTaskById((int)id);
            }
            return View(model);
        }

        [HttpPost]
        [Obsolete]
        public async Task<IActionResult> Manage(PreDefinedTaskModel preDefinedTaskModel)
        {
            if (ModelState.IsValid)
            {
                List<UsersTaskDocumentModel> fileDetails = new List<UsersTaskDocumentModel>();

                #region  --Added By Nilasish for notificationwork on 19052022
                var companyId = HttpContext.Session.GetComplexData<int>(SessionConstants.CompanyId);
                var employeeId = HttpContext.Session.GetComplexData<int>(SessionConstants.EmployeeId);
                var company = companiesServices.GetCompanyById(companyId).Result;
                ViewBag.CompanyId = companyId;
                ViewBag.EmployeeId = employeeId;
                ViewBag.IsAdmin = company.IsAdmin;
                // For Admin
                if (ViewBag.IsAdmin == true && ViewBag.EmployeeId == 0)
                {
                    preDefinedTaskModel.IsAdminRead = 1;
                    preDefinedTaskModel.IsClientRead = 0;
                    preDefinedTaskModel.IsEmployeeRead = 0;
                }
                // For Client
                else if (ViewBag.IsAdmin == false && ViewBag.EmployeeId == 0)
                {
                    preDefinedTaskModel.IsAdminRead = 0;
                    preDefinedTaskModel.IsClientRead = 1;
                    preDefinedTaskModel.IsEmployeeRead = 0;
                }
                // For Employee
                else //if (ViewBag.EmployeeId != 0)
                {
                    preDefinedTaskModel.IsAdminRead = 0;
                    preDefinedTaskModel.IsClientRead = 0;
                    preDefinedTaskModel.IsEmployeeRead = 1;
                }

                #endregion --Added By Nilasish for notificationwork on 19052022



                string path = (hostingEnvironment.WebRootPath + "\\PreDefinedTaskLogo\\").ToString();
                
                if (preDefinedTaskModel.PostedFiles != null)
                {
                    foreach (IFormFile postedFile in preDefinedTaskModel.PostedFiles)
                    {
                        string fileName = DateTime.Now.ToString("ddMMyyyyhhmmss") + "_"+ Path.GetFileName(postedFile.FileName);
                        using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                        {
                            postedFile.CopyTo(stream);
                            preDefinedTaskModel.LogoName = fileName;
                        }

                    }
                }
                var result = await preDefinedTaskService.ManagePreDefinedTask(preDefinedTaskModel);
                if (result)
                    SuccessPassMessage(MessageConstants.SavedSuccessfully);
                else
                    FailPassMessage(MessageConstants.GeneralErrorMessage);
            }

            return RedirectToAction("Index");
        }



        //[HttpGet]
        public async Task<IActionResult> ManagePreDefineTask(int Id)
        {
            var preDefinedTaskModel = new PreDefinedTaskModel();
            preDefinedTaskModel.Id = Id;
            preDefinedTaskModel.IsAdminRead = 0;
            preDefinedTaskModel.IsClientRead = 0;
            preDefinedTaskModel.IsEmployeeRead = 0;
            var result = await preDefinedTaskService.ManagePreDefinedNotificationTask(preDefinedTaskModel); 
            return RedirectToAction("Index");
           
        }

        public IActionResult MarkReadTaskNotificationList(List<int> predefinedTaskId, List<int> userTaskId)
        {
           
            predefinedTaskId.ForEach(c =>
            {
                var preDefinedTaskModel = new PreDefinedTaskModel();
                preDefinedTaskModel.Id = c;
                preDefinedTaskModel.IsAdminRead = 0;
                preDefinedTaskModel.IsClientRead = 0;
                preDefinedTaskModel.IsEmployeeRead = 0;
                preDefinedTaskService.ManagePreDefinedNotificationTask(preDefinedTaskModel);
            });

            userTaskId.ForEach(c =>
            {
                var preDefinedTaskModel = new PreDefinedTaskModel();
                preDefinedTaskModel.Id = c;
                preDefinedTaskModel.IsAdminRead = 0;
                preDefinedTaskModel.IsClientRead = 0;
                preDefinedTaskModel.IsEmployeeRead = 0;
                usersTaskService.ManagePreDefinedNotificationTask(preDefinedTaskModel);
            });

            return Json(new { result = true });

        }


        public async Task<IActionResult> DeletePreDefinedTask(int id)
        {
            var preDefinedTask = await preDefinedTaskService.GetPreDefinedTaskById((int)id);

            string path = (hostingEnvironment.WebRootPath + "\\PreDefinedTaskLogo\\").ToString()+ preDefinedTask.LogoName;

            if(System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }

            var result = await preDefinedTaskService.DeletePreDefinedTask(id);

            SuccessPassMessage(MessageConstants.DeleteSuccessfully);
            if (result)
                return Json(new { result = true, url = Url.Action("Index", "PreDefinedTask") });
            else
                return Json(new { result = false });
        }

        public async Task<IActionResult> AutoCompleteProjectList(string projectName)
        {
            var model = new PreDefinedTaskViewModel();

            model.PreDefinedTasks = await preDefinedTaskService.GetAllPreDefinedTask(new PreDefinedTaskSearchParameters() { SearchText = projectName });
            var projectList = model.PreDefinedTasks.Where(c => c.JobTitle.ToLower().Contains(projectName.ToLower())).Select(c => c.JobTitle);
            return Json(projectList);
        }

        public async Task<IActionResult> GetPreDefinedTaskList(PreDefinedTaskSearchParameters searchParameter)
        {
            var companyId = HttpContext.Session.GetComplexData<int>(SessionConstants.CompanyId);
            var employeeId = HttpContext.Session.GetComplexData<int>(SessionConstants.EmployeeId);
            var company = companiesServices.GetCompanyById(companyId).Result;
            ViewBag.CompanyId = companyId;
            ViewBag.EmployeeId = employeeId;
            ViewBag.IsAdmin = company.IsAdmin;
            var model = await preDefinedTaskService.GetAllPreDefinedTask(searchParameter);
            return PartialView("_TaskList", model);
        }

        public async Task<ActionResult> ShowTaskDetails(int taskId)
        {
            var companyId = HttpContext.Session.GetComplexData<int>(SessionConstants.CompanyId);
            var employeeId = HttpContext.Session.GetComplexData<int>(SessionConstants.EmployeeId);
            var company = companiesServices.GetCompanyById(companyId).Result;
            ViewBag.CompanyId = companyId;
            ViewBag.EmployeeId = employeeId;
            ViewBag.IsAdmin = company.IsAdmin;

            var model = await preDefinedTaskService.GetPreDefinedTaskById(taskId);
            if (company.IsAdmin)
            {
                return PartialView("_TaskDetails", model);
            }
            else
            {
                return PartialView("_ViewTaskDetails", model);
            }
        }

        public async Task<ActionResult> SavePreDefinedTaskPopupDetail(int preDefinedTaskId, string propertyName, string propertyValue)
        {
            PreDefinedTaskModel model = new PreDefinedTaskModel();
            #region  --Added By Nilasish for notificationwork on 19052022
            var companyId = HttpContext.Session.GetComplexData<int>(SessionConstants.CompanyId);
            var employeeId = HttpContext.Session.GetComplexData<int>(SessionConstants.EmployeeId);
            var company = companiesServices.GetCompanyById(companyId).Result;
            ViewBag.CompanyId = companyId;
            ViewBag.EmployeeId = employeeId;
            ViewBag.IsAdmin = company.IsAdmin;
            // For Admin
            if (ViewBag.IsAdmin == true && ViewBag.EmployeeId == 0)
            {
                model.IsAdminRead = 1;
                model.IsClientRead = 0;
                model.IsEmployeeRead = 0;
            }
            // For Client
            else if (ViewBag.IsAdmin == false && ViewBag.EmployeeId == 0)
            {
                model.IsAdminRead = 0;
                model.IsClientRead = 1;
                model.IsEmployeeRead = 0;
            }
            // For Employee
            else //if (ViewBag.EmployeeId != 0)
            {
                model.IsAdminRead = 0;
                model.IsClientRead = 0;
                model.IsEmployeeRead = 1;
            }

            #endregion --Added By Nilasish for notificationwork on 19052022

            model.Id = preDefinedTaskId;
            if (propertyName == "JobTitle")
            {
                model.JobTitle = propertyValue;
            }
            else if (propertyName == "ServiceDescription")
            {
                model.ServiceDescription = propertyValue;
            }
            else if (propertyName == "ETD")
            {
                model.ETD = propertyValue;
            }
            var result = await preDefinedTaskService.UpdatePreDefinedTaskDetails(model);
            return this.Json(result);
        }

        //public async Task<IActionResult> LoadCompanyTaskChat(int TaskId, string ReceiverUserId)
        //{
        //    try
        //    {
        //        if (TaskId > 0 && !string.IsNullOrEmpty(ReceiverUserId))
        //        {
        //            var GetSenderUserId = HttpContext.Session.GetComplexData<string>(SessionConstants.UserId);
        //            var GetReceiverUser = await _employeeServices.GetEmployeeByUserId(ReceiverUserId);

        //            if (GetReceiverUser != null && !string.IsNullOrEmpty(GetReceiverUser.ProfilePicture))
        //                GetReceiverUser.ProfilePicture = "/images/" + GetReceiverUser.ProfilePicture;
        //            else
        //                GetReceiverUser.ProfilePicture = "/assets/images/users/avatar-1.jpg";

        //            var ChatHistory = await _chatService.GetChatByTaskId(TaskId, GetSenderUserId);
        //            return Json(new { status = "success", ChatHistory, Name = GetReceiverUser.FirstName + " " + GetReceiverUser.LastName, ProfilePiture = GetReceiverUser.ProfilePicture });
        //        }
        //        return Json(new { status = "error", msg = "Invalid Details" });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { status = "error", msg = ex.Message });
        //    }
        //}





    }
}
