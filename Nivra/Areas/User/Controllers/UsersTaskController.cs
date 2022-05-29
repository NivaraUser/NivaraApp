using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nivara.Common.Constants;
using Nivara.Core.UsersTask;
using Nivara.Models;
using Nivara.Web.Helper;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Nivara.Web.Controllers;

namespace Nivara.Web.Areas.User.Controllers
{
    [Area("User")]
    public class UsersTaskController : BaseController
    {
        private readonly IUsersTaskService usersTaskService;
        public UsersTaskController(IUsersTaskService _usersTaskService)
        {
            usersTaskService = _usersTaskService;
        }

        public IActionResult DashBoard()
        {
            return View();
        }
        public async Task<IActionResult> Index()
        {
            ShowPassMessage();
            var model = new List<UsersTaskModel>();
            model = await usersTaskService.GetUsersTaskByEndUserId(HttpContext.Session.GetComplexData<int>(SessionConstants.EndUserId));
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Manage(int? id)
        {
            UsersTaskModel model = new UsersTaskModel();

            if (id > 0)
            {
                model = await usersTaskService.GetUsersTaskById((int)id);
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Manage(UsersTaskModel usersTaskModel)
        {
            if (ModelState.IsValid)
            {
                List<UsersTaskDocumentModel> fileDetails = new List<UsersTaskDocumentModel>();
                string path = @"c:\temp\MyTest";
                // var path = Path.Combine(Server.MapPath("~/App_Data/Upload/"), fileDetail.Id + fileDetail.Extension);

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
                usersTaskModel.EndUserId = HttpContext.Session.GetComplexData<int>(SessionConstants.EndUserId);
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

          
            if (result)
            {
                SuccessPassMessage(MessageConstants.DeleteSuccessfully);
                return Json(new { result = true, url = Url.Action("Index", "UsersTask") });
            }
               
            else
                return Json(new { result = false });
        }

        public async Task<IActionResult> CompanyTasks()
        {
            var model = new List<UsersTaskModel>();
            model = await usersTaskService.GetCompanyTasks();
            return View(model);
        }

       
        //public async Task<IActionResult> GetRemarks(int id)
        //{
        //    UsersTaskModel model = new UsersTaskModel();

        //    if (id > 0)
        //    {
        //        model = await usersTaskService.GetUsersTaskById((int)id);
        //    }
        //    return View(model.Remarks);
        //}
    }
}
