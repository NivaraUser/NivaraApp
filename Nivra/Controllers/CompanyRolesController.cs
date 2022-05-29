using Microsoft.AspNetCore.Mvc;
using Nivara.Common.Constants;
using Nivara.Core.CompanyDetail;
using Nivara.Core.CompanyRole;
using Nivara.Models;
using Nivara.Web.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nivara.Web.Controllers
{
    public class CompanyRolesController : BaseController
    {
        private readonly ICompanyRolesServices _rolesServices;
        public CompanyRolesController(ICompanyRolesServices rolesServices)
        {
            _rolesServices = rolesServices;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ShowPassMessage();
            List<CompanyRolesModel> model = new List<CompanyRolesModel>();
            try
            {
                model = await _rolesServices.GetRolesByCompanyId(HttpContext.Session.GetComplexData<int>(SessionConstants.CompanyId));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Manage(CompanyRolesModel rolesModel)
        {
            rolesModel.CreatedBy = HttpContext.Session.GetComplexData<string>(SessionConstants.UserId);
            rolesModel.CompanyId = HttpContext.Session.GetComplexData<int>(SessionConstants.CompanyId);
            rolesModel.ModifiedBy = rolesModel.CreatedBy;
            try
            {
                await _rolesServices.ManageRole(rolesModel);
                SuccessPassMessage(MessageConstants.SavedSuccessfully);
            }
            catch (Exception)
            {
                SuccessPassMessage(MessageConstants.GeneralErrorMessage);
            }
           
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id = 0)
        {
            try
            {
                await _rolesServices.DeleteCompanyRoleId(id);
                SuccessPassMessage(MessageConstants.DeleteSuccessfully);
                return Json(new { result = true, url = Url.Action("Index", "CompanyRoles") });
            }
            catch (Exception ex)
            {
                return Json(new { result = false });
            }
           
           
        }

        public async Task<IActionResult> AssignModule()
        {
            ShowPassMessage();
            CompanyRolesModel model = new CompanyRolesModel();
            model.CompanyRoles = await _rolesServices.GetRolesByCompanyId(HttpContext.Session.GetComplexData<int>(SessionConstants.CompanyId));
            return View(model);
        }

        public async Task<IActionResult> GetModules(int companyRoleId = 0)
        {
            CompanyRolesModel model = new CompanyRolesModel();
            model.Modules = await _rolesServices.GetModulesByCompanyRoleId(companyRoleId);
            model.Id = companyRoleId;
            return PartialView("Partial/_RolePages", model);
        }

        [HttpPost]
        public async Task<IActionResult> AssignModule(CompanyRolesModel model)
        {
            try
            {
                await _rolesServices.SaveModulePages(model);
                SuccessPassMessage(MessageConstants.SavedSuccessfully);
            }
            catch (Exception)
            {
                SuccessPassMessage(MessageConstants.GeneralErrorMessage);
            }

            return RedirectToAction("AssignModule");
        }
    }
}
