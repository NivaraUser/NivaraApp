using Microsoft.AspNetCore.Mvc;
using Nivara.Common.Constants;
using Nivara.Core.UsersTask;
using Nivara.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nivara.Web.Controllers
{
    public class EndUsersController : BaseController
    {
        private readonly IUsersTaskService usersTaskService;
        public EndUsersController(IUsersTaskService _usersTaskService)
        {
            usersTaskService = _usersTaskService;
        }
        public async Task<IActionResult> Index()
        {
            ShowPassMessage();
            var model = new List<UsersTaskModel>();
            model = await usersTaskService.GetEndUsersTasks();
            return View(model);
        }

        public async Task<IActionResult> Manage(int id, bool IsActive)
        {
            UsersTaskModel model = new UsersTaskModel();
            model.Id = id;
            model.IsDeleted = IsActive;

            var result = await usersTaskService.UpdateUserTask(model);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> SaveTaskDeActivateReason(int id, string remarks, bool isActive)
        {

            var result = await usersTaskService.SaveTaskRemarks(id, remarks, isActive);
            if (result)
                SuccessPassMessage(MessageConstants.UpdateSuccessfully);
            else
                FailPassMessage(MessageConstants.GeneralErrorMessage);
            return RedirectToAction("Index");
        }

    }
}
