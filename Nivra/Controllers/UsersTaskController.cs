using Grpc.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nivara.Common.Constants;
using Nivara.Common.Enums;
using Nivara.Core.Common;
using Nivara.Core.CompanyDetail;
using Nivara.Core.Employee;
using Nivara.Core.PreDefinedTask;
using Nivara.Core.UsersTask;
using Nivara.Data.Models;
using Nivara.Models;
using Nivara.Web.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HttpGetAttribute = Microsoft.AspNetCore.Mvc.HttpGetAttribute;
using HttpPostAttribute = Microsoft.AspNetCore.Mvc.HttpPostAttribute;

namespace Nivara.Web.Controllers
{
    public class UsersTaskController : BaseController
    {
        private readonly IUsersTaskService usersTaskService;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly ICompaniesServices companiesServices;
        private readonly IEmployeeServices employeeServices;
        private readonly ICommonServices commonServices;
        private readonly IPreDefinedTaskService preDefinedTaskService;
        public UsersTaskController(IUsersTaskService _usersTaskService, ICompaniesServices _companiesServices, IWebHostEnvironment _webHostEnvironment, IHostingEnvironment _hostingEnvironment, IEmployeeServices _employeeServices, ICommonServices _commonServices, IPreDefinedTaskService _preDefinedTaskService)
        {
            usersTaskService = _usersTaskService;
            webHostEnvironment = _webHostEnvironment;
            hostingEnvironment = _hostingEnvironment;
            companiesServices = _companiesServices;
            employeeServices = _employeeServices;
            commonServices = _commonServices;
            preDefinedTaskService = _preDefinedTaskService;
        }
        public async Task<IActionResult> Index()
        {
            ShowPassMessage();
            var model = new UsersTaskViewModel();
            var companyId = HttpContext.Session.GetComplexData<int>(SessionConstants.CompanyId);
            var employeeId = HttpContext.Session.GetComplexData<int>(SessionConstants.EmployeeId);
            var company = companiesServices.GetCompanyById(companyId).Result;
            ViewBag.CompanyId = companyId;
            ViewBag.EmployeeId = employeeId;
            ViewBag.IsAdmin = company.IsAdmin;

            int IsAdmin = 0; int IsClient = 0; int IsEmployee = 0;
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
           // ViewBag.PreDefinedNotificationTasks = await usersTaskService.GetNotification(IsAdmin, IsClient, IsEmployee);
            var usersTaskNotificationTasks = await preDefinedTaskService.GetNotification(IsAdmin, IsClient, IsEmployee);
            usersTaskNotificationTasks.AddRange(await usersTaskService.GetNotification(IsAdmin, IsClient, IsEmployee));
            ViewBag.PreDefinedNotificationTasks = usersTaskNotificationTasks;            


            if (company.IsAdmin && employeeId == 0)
            {
                model.UsersTasks = await usersTaskService.GetUsersTaskForAdmin(new UserTaskSearchParameters());
            }
            else if (!company.IsAdmin && employeeId == 0)
            {
                model.UsersTasks = await usersTaskService.GetUsersTaskByCompanyId(companyId, new UserTaskSearchParameters());
            }
            else
            {
                model.UsersTasks = await usersTaskService.GetUsersTaskByEmployeeId(employeeId, new UserTaskSearchParameters());
            }
            model.TaskStatusModels = await commonServices.GetTaskStatus();
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Manage(int? id)
        {
            UsersTaskModel model = new UsersTaskModel();
            var companyId = HttpContext.Session.GetComplexData<int>(SessionConstants.CompanyId);
            var employeeId = HttpContext.Session.GetComplexData<int>(SessionConstants.EmployeeId);
            var company = companiesServices.GetCompanyById(companyId).Result;
            ViewBag.CompanyId = companyId;
            ViewBag.EmployeeId = employeeId;
            ViewBag.IsAdmin = company.IsAdmin;

            if (id > 0)
            {
                model = await usersTaskService.GetUsersTaskById((int)id);
            }
            else
            {
                model.TaskStatusId =(int)TaskStatusEnum.New;
                model.CompanyId = companyId;
            }
            model.Companies =await companiesServices.GetAllCompanies();
            model.TaskStatus = await usersTaskService.GetAllTaskStatus();
            return View(model);
        }

        [HttpPost]
        [Obsolete]
        public async Task<IActionResult> Manage(UsersTaskModel usersTaskModel)
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
                    usersTaskModel.IsAdminRead = 1;
                    usersTaskModel.IsClientRead = 0;
                    usersTaskModel.IsEmployeeRead = 0;
                }
                // For Client
                else if (ViewBag.IsAdmin == false && ViewBag.EmployeeId == 0)
                {
                    usersTaskModel.IsAdminRead = 0;
                    usersTaskModel.IsClientRead = 1;
                    usersTaskModel.IsEmployeeRead = 0;
                }
                // For Employee
                else //if (ViewBag.EmployeeId != 0)
                {
                    usersTaskModel.IsAdminRead = 0;
                    usersTaskModel.IsClientRead = 0;
                    usersTaskModel.IsEmployeeRead = 1;
                }

                #endregion --Added By Nilasish for notificationwork on 19052022



                string path = Directory.CreateDirectory(hostingEnvironment.WebRootPath + "\\Documents\\").ToString();

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                if (usersTaskModel.postedFiles != null)
                {
                    foreach (IFormFile postedFile in usersTaskModel.postedFiles)
                    {
                        string fileName = Path.GetFileName(postedFile.FileName);
                        using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                        {
                            var files = new UsersTaskDocumentModel();
                            postedFile.CopyTo(stream);
                            files.DocumentName = fileName;
                            files.DocumentPath = path;
                            usersTaskModel.UsersTaskDocuments.Add(files);
                        }

                    }
                }
                //usersTaskModel.CompanyId = HttpContext.Session.GetComplexData<int>(SessionConstants.CompanyId);
                var result = await usersTaskService.ManageUsersTask(usersTaskModel);




                if (result)
                    SuccessPassMessage(MessageConstants.SavedSuccessfully);
                else
                    FailPassMessage(MessageConstants.GeneralErrorMessage);
            }


            //  model = await _employeeServices.GetEmployeeById();
            // model.Countries = await _commonServices.GetCountries();
            // model.CompanyRoles = await _companyRolesServices.GetRolesByCompanyId(HttpContext.Session.GetComplexData<int>(SessionConstants.CompanyId));


            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteUsersTask(int id)
        {
            var result = await usersTaskService.DeleteUsersTaskById(id);

            SuccessPassMessage(MessageConstants.DeleteSuccessfully);
            if (result)
                return Json(new { result = true, url = Url.Action("Index", "UsersTask") });
            else
                return Json(new { result = false });
        }

        public string UploadFiles(IFormFile file)
        {
            string uniqueFileName = null;

            if (file != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }

        public async Task<ActionResult> ShowTaskDetails(int taskId)
        {
            UsersTaskModel model = new UsersTaskModel();
            var companyId = HttpContext.Session.GetComplexData<int>(SessionConstants.CompanyId);
            var employeeId = HttpContext.Session.GetComplexData<int>(SessionConstants.EmployeeId);
            var company = companiesServices.GetCompanyById(companyId).Result;
            ViewBag.CompanyId = companyId;
            ViewBag.EmployeeId = employeeId;

            model = await usersTaskService.GetUsersTaskById(taskId);

            model.Companies = await companiesServices.GetAllCompanies();
            model.Employees = await employeeServices.GetEmployeesByCompanyId(companyId);
            model.TaskStatus = await usersTaskService.GetAllTaskStatus();
            model.TaskComments = (await usersTaskService.GetAllTaskComments()).TakeLast(3).ToList();
            var latestTaskAssignment = (await employeeServices.GetLatestTaskAssignmentByTaskId(taskId));
            model.AssignedEmployeeId = latestTaskAssignment==null? 0: latestTaskAssignment.EmpId;
            model.AssignedEmployeeName = latestTaskAssignment?.EmployeeName;
            model.UsersTaskDocuments = await usersTaskService.GetAllTaskDocuments(taskId);
            return this.PartialView("_TaskDetails", model);
        }

        public async Task<ActionResult> SaveUserTaskPopupDetail(int userTaskId, string propertyName, string propertyValue)
        {
            UsersTaskModel model = new UsersTaskModel();

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

            model.Id = userTaskId;
            if (propertyName == "ProjectName")
            {
                model.ProjectName = propertyValue;
            }
            else if (propertyName == "ProjectDescription")
            {
                model.ProjectDescription = propertyValue;
            }
            var result = await usersTaskService.UpdateUserTaskDetails(model);
           // var result = true;
            return this.Json(result);
        }

        public async Task<ActionResult> SaveUserTaskComments(TaskCommentsModel taskCommentsModel)
        {
            var companyId = HttpContext.Session.GetComplexData<int>(SessionConstants.CompanyId);
            var employeeId = HttpContext.Session.GetComplexData<int>(SessionConstants.EmployeeId);
            taskCommentsModel.CompanyId = companyId;
            taskCommentsModel.EmployeeId = employeeId;
            taskCommentsModel.CreatedDateTime = DateTime.UtcNow;

            await usersTaskService.SaveUserTaskComments(taskCommentsModel);

            var Comments = (await usersTaskService.GetAllTaskComments()).TakeLast(3).ToList();
            return this.PartialView("_TaskComment", Comments);
        }

        public async Task<ActionResult> SaveAssignTaskToEmployee(EmployeesTaskModel employeesTaskModel)
        {
            var companyId = HttpContext.Session.GetComplexData<int>(SessionConstants.CompanyId);
            var employeeId = HttpContext.Session.GetComplexData<int>(SessionConstants.EmployeeId);
            employeesTaskModel.CreatedBy = employeeId == 0? companyId : employeeId;
            await employeeServices.ManageEmployeesTasks(employeesTaskModel);

            var latestTaskAssignment = (await employeeServices.GetLatestTaskAssignmentByTaskId(employeesTaskModel.TaskId));
            var assignedEmployeeId = latestTaskAssignment == null ? 0 : latestTaskAssignment.EmpId;
            var assignedEmployeeName = latestTaskAssignment?.EmployeeName;

            return this.Json( new { AssignedEmployeeId = assignedEmployeeId, AssignedEmployeeName = assignedEmployeeName });
        }

        [HttpPost]
        public async Task<ActionResult> UploadDocument()
        {
            var files = Request.Form.Files;
            string userTaskId = Request.Form.Where(x => x.Key == "userTaskId").FirstOrDefault().Value;
            var objFile = new UsersTaskDocumentModel();
            foreach (var uploadedFile in files)
            {
                string path = Directory.CreateDirectory(hostingEnvironment.WebRootPath + "\\Documents\\").ToString();
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string fileName = Path.GetFileName(uploadedFile.FileName);
                
                using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                {
                    uploadedFile.CopyTo(stream);
                    objFile.DocumentName = fileName;
                    objFile.DocumentPath = path;
                    objFile.UsersTaskId =int.Parse(userTaskId);
                }
               await usersTaskService.SaveUserTaskDocuments(objFile);
            }
            var usersTaskModel = new UsersTaskModel();
            usersTaskModel.UsersTaskDocuments =await usersTaskService.GetAllTaskDocuments(int.Parse(userTaskId));
            return PartialView("_TaskDocuments", usersTaskModel);
        }
        public async Task<IActionResult> GetUsersTaskList(UserTaskSearchParameters searchParameter)
        {
            var model = new List<UsersTaskModel>();
            var companyId = HttpContext.Session.GetComplexData<int>(SessionConstants.CompanyId);
            var employeeId = HttpContext.Session.GetComplexData<int>(SessionConstants.EmployeeId);
            var company = companiesServices.GetCompanyById(companyId).Result;
            ViewBag.CompanyId = companyId;
            ViewBag.EmployeeId = employeeId;
            ViewBag.IsAdmin = company.IsAdmin;
            if (company.IsAdmin && employeeId == 0)
            {
                model = await usersTaskService.GetUsersTaskForAdmin(searchParameter);
            }
            else if (!company.IsAdmin && employeeId == 0)
            {
                model = await usersTaskService.GetUsersTaskByCompanyId(companyId, searchParameter);
            }
            else
            {
                model = await usersTaskService.GetUsersTaskByEmployeeId(employeeId, searchParameter);
            }
            return PartialView("_TaskList", model);
        }

        public async Task<IActionResult> AutoCompleteProjectList(string projectName)
        {
            var model = new UsersTaskViewModel();
            var companyId = HttpContext.Session.GetComplexData<int>(SessionConstants.CompanyId);
            var employeeId = HttpContext.Session.GetComplexData<int>(SessionConstants.EmployeeId);
            var company = companiesServices.GetCompanyById(companyId).Result;
            ViewBag.CompanyId = companyId;
            ViewBag.EmployeeId = employeeId;
            ViewBag.IsAdmin = company.IsAdmin;
            if (company.IsAdmin && employeeId == 0)
            {
                model.UsersTasks = await usersTaskService.GetUsersTaskForAdmin(new UserTaskSearchParameters());
            }
            else if (!company.IsAdmin && employeeId == 0)
            {
                model.UsersTasks = await usersTaskService.GetUsersTaskByCompanyId(companyId, new UserTaskSearchParameters());
            }
            else
            {
                model.UsersTasks = await usersTaskService.GetUsersTaskByEmployeeId(employeeId, new UserTaskSearchParameters());
            }
            var projectList = model.UsersTasks.Where(c => c.ProjectName.ToLower().Contains(projectName.ToLower())).Select(c => c.ProjectName);
            return Json(projectList);
        }

        public async Task<ActionResult> DeleteUsersTaskDocument(int id, int userTaskId)
        {
            var documentList = await usersTaskService.GetAllTaskDocuments(userTaskId);
            var documentDetails = documentList.FirstOrDefault(c => c.Id == id);
            if(documentDetails !=null)
            {
                string path = hostingEnvironment.WebRootPath + "\\Documents\\"+ documentDetails.DocumentName;
                if(System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
                await usersTaskService.DeleteUsersTaskDocumentById(id);
            }
            var usersTaskModel = new UsersTaskModel();
            usersTaskModel.UsersTaskDocuments = await usersTaskService.GetAllTaskDocuments(userTaskId);
            return PartialView("_TaskDocuments", usersTaskModel);
        }

        [HttpGet]
        public async Task<IActionResult> AddToMyQueue(int preDefinedTaskId)
        {
            UsersTaskModel model = new UsersTaskModel();
            var companyId = HttpContext.Session.GetComplexData<int>(SessionConstants.CompanyId);
            var employeeId = HttpContext.Session.GetComplexData<int>(SessionConstants.EmployeeId);
            var company = companiesServices.GetCompanyById(companyId).Result;
            ViewBag.CompanyId = companyId;
            ViewBag.EmployeeId = employeeId;
            ViewBag.IsAdmin = company.IsAdmin;

            var preDefinedTaskModel = await usersTaskService.GetPreDefinedTaskById(preDefinedTaskId);
            model.ProjectName = preDefinedTaskModel.JobTitle;
            model.ProjectDescription = preDefinedTaskModel.ServiceDescription;

            model.TaskStatusId = (int)TaskStatusEnum.New;
            model.CompanyId = companyId;

            model.Companies = await companiesServices.GetAllCompanies();
            model.TaskStatus = await usersTaskService.GetAllTaskStatus();
            return View("Manage",model);
        }

        [Route("api/UsersTask/GetUsersTaskList/{companyName?}")]
        public async Task<string> GetUsersTaskApiList(string companyName)
        {
            var model = new List<UsersTaskResponseModel>();
            model = await usersTaskService.GetUsersTaskListForApi(companyName);
            var jsonResult = JsonConvert.SerializeObject(model);
            return jsonResult;
        }

        //[HttpGet]
        public async Task<IActionResult> ManagePreDefineTask(int Id)
        {
            var preDefinedTaskModel = new PreDefinedTaskModel();
            preDefinedTaskModel.Id = Id;
            preDefinedTaskModel.IsAdminRead = 0;
            preDefinedTaskModel.IsClientRead = 0;
            preDefinedTaskModel.IsEmployeeRead = 0;
            var result = await usersTaskService.ManagePreDefinedNotificationTask(preDefinedTaskModel);
            return RedirectToAction("Index");

        }



    }
}
