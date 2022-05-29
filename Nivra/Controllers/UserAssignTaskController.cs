using Microsoft.AspNetCore.Mvc;
using Nivara.Common.Constants;
using Nivara.Core.CompanyRole;
using Nivara.Core.Employee;
using Nivara.Core.UsersTask;
using Nivara.Models;
using Nivara.Web.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using Nivara.Data.Models.NivaraDbContext;
using Nivara.Core.UserAssignTask;

namespace Nivara.Web.Controllers
{
    public class UserAssignTaskController : BaseController
    {
        private readonly IEmployeeServices _employeeServices;
        private readonly IUsersTaskService _usersTaskService;

        private readonly IUserAssignTaskService _userAssignTaskService;
        public UserAssignTaskController(IEmployeeServices employeeServices, IUsersTaskService usersTaskService, IUserAssignTaskService userAssignTaskService)
        {
            _employeeServices = employeeServices;
            _usersTaskService = usersTaskService;
            _userAssignTaskService = userAssignTaskService;
        }

        public async Task<IActionResult> Manage(int id = 0)
        {

            var model = new EmployeesTaskModel();
            if (id > 0)
            {
                model = await _userAssignTaskService.GetEmployeesTasksById((int)id);
                model.Employees = await _employeeServices.GetEmployeesByCompanyId(HttpContext.Session.GetComplexData<int>(SessionConstants.CompanyId));
                model.UsersTasks = await _usersTaskService.GetTasks(model.TaskId);
            }
            else
            {
                model.Employees = await _employeeServices.GetEmployeesByCompanyId(HttpContext.Session.GetComplexData<int>(SessionConstants.CompanyId));
                model.UsersTasks = await _usersTaskService.GetTasks(0);
            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Manage(EmployeesTaskModel model)
        {
            model.CreatedBy = HttpContext.Session.GetComplexData<int>(SessionConstants.CompanyId);
            try
            {
                await _employeeServices.ManageEmployeesTasks(model);
                SuccessPassMessage(MessageConstants.SavedSuccessfully);
            }
            catch (Exception)
            {
                FailPassMessage(MessageConstants.GeneralErrorMessage);
            }
            return RedirectToAction("Index", "UserAssignTask");
        }

        public async Task<IActionResult> Index()
        {
            ShowPassMessage();
            var employeeTaskModel = new List<EmployeesTaskModel>();
            var companyId = HttpContext.Session.GetComplexData<int>(SessionConstants.CompanyId);
            var employeeId = HttpContext.Session.GetComplexData<int>(SessionConstants.EmployeeId);
            //var company = companiesServices.GetCompanyById(companyId).Result;
            ViewBag.CompanyId = companyId;
            ViewBag.EmployeeId = employeeId;
            //ViewBag.IsAdmin = company.IsAdmin;
            employeeTaskModel = await _userAssignTaskService.GetEmployeesTasks(HttpContext.Session.GetComplexData<int>(SessionConstants.CompanyId));

            return View(employeeTaskModel);
        }
        public async Task<IActionResult> Delete(int id = 0)
        {
            var result = await _userAssignTaskService.DeleteEmpTasks(id);

            if (result)
            {
                SuccessPassMessage(MessageConstants.DeleteSuccessfully);
                return Json(new { result = true, url = Url.Action("Index", "UserAssignTask") });
            }
            else
            {
                SuccessPassMessage(MessageConstants.GeneralErrorMessage);
                return Json(new { result = false });
            }

        }

    }
}
