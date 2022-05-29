using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Nivara.Common.Constants;
using Nivara.Common.Helpers;
using Nivara.Core.Common;
using Nivara.Core.CompanyDetail;
using Nivara.Core.CompanyRole;
using Nivara.Core.Employee;
using Nivara.Models;
using Nivara.Web.Helper;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Nivara.Web.Controllers
{
    [Authorize]
    public class EmployeeController : BaseController
    {
        private readonly IEmployeeServices _employeeServices;
        private readonly ICommonServices _commonServices;
        private readonly ICompanyRolesServices _companyRolesServices;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IOptions<AppSettingsModel> options;
        public EmployeeController(IEmployeeServices employeeServices, ICommonServices commonServices, ICompanyRolesServices companyRolesServices, IWebHostEnvironment hostEnvironment, IOptions<AppSettingsModel> _options)
        {
            _employeeServices = employeeServices;
            _commonServices = commonServices;
            _companyRolesServices = companyRolesServices;
            _webHostEnvironment = hostEnvironment;
            options = _options;
        }

        public async Task<IActionResult> Index()
        {
            ShowPassMessage();
            List<EmployeeModel> model = new List<EmployeeModel>();
            try
            {
                model = await _employeeServices.GetEmployeesByCompanyId(HttpContext.Session.GetComplexData<int>(SessionConstants.CompanyId));

            }
            catch (Exception ex)
            {

            }
            return View(model);
        }
        public  ActionResult GetData(JqueryDatatableParam param)
        {
            var employees =  _employeeServices.GetEmployeesByCompanyId(HttpContext.Session.GetComplexData<int>(SessionConstants.CompanyId));


            //employees.ToList().ForEach(x => x.StartDateString = x.StartDate.ToString("dd'/'MM'/'yyyy"));

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                //employees = employees.Where(x => x.UserName.ToLower().Contains(param.sSearch.ToLower()));
                                             // || x.Position.ToLower().Contains(param.sSearch.ToLower())
                                             // || x.Location.ToLower().Contains(param.sSearch.ToLower())
                                            //  || x.Salary.ToString().Contains(param.sSearch.ToLower())
                                            //  || x.Age.ToString().Contains(param.sSearch.ToLower())
                                             // || x.StartDate.ToString("dd'/'MM'/'yyyy").ToLower().Contains(param.sSearch.ToLower())).ToList();
            }

            //var sortColumnIndex = Convert.ToInt32(HttpContext.Request.QueryString["iSortCol_0"]);
            //var sortDirection = HttpContext.Request.QueryString["sSortDir_0"];

            //if (sortColumnIndex == 3)
            //{
            //    employees = sortDirection == "asc" ? employees.OrderBy(c => c.Age) : employees.OrderByDescending(c => c.Age);
            //}
            //else if (sortColumnIndex == 4)
            //{
            //    employees = sortDirection == "asc" ? employees.OrderBy(c => c.StartDate) : employees.OrderByDescending(c => c.StartDate);
            //}
            //else if (sortColumnIndex == 5)
            //{
            //    employees = sortDirection == "asc" ? employees.OrderBy(c => c.Salary) : employees.OrderByDescending(c => c.Salary);
            //}
            //else
            //{
            //    Func<Employee, string> orderingFunction = e => sortColumnIndex == 0 ? e.Name :
            //                                                   sortColumnIndex == 1 ? e.Position :
            //                                                   e.Location;

            //    employees = sortDirection == "asc" ? employees.OrderBy(orderingFunction) : employees.OrderByDescending(orderingFunction);
            //}

            //var displayResult = employees.Skip(param.iDisplayStart)
            //    .Take(param.iDisplayLength).ToList();
            //var totalRecords = employees.Count();

            return Json(new
            {
                //param.sEcho,
                //iTotalRecords = totalRecords,
                //iTotalDisplayRecords = totalRecords,
                //aaData = displayResult
            });

        
    }
        
        public async Task<ActionResult> Employee_Read()
        {
            //var length = Request.Form["length"].FirstOrDefault();
            //var data = await _employeeServices.GetEmployeesByCompanyId(HttpContext.Session.GetComplexData<int>(SessionConstants.CompanyId));

            //return Ok(data);

            try
            {
                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() +
                                              "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                var customerData = await _employeeServices.GetEmployeesByCompanyId(HttpContext.Session.GetComplexData<int>(SessionConstants.CompanyId));

                //if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                //{
                //    customerData = customerData.OrderBy(sortColumn + " " + sortColumnDirection);
                //}
                //if (!string.IsNullOrEmpty(searchValue))
                //{
                //    customerData = customerData.Where(m => m.FirstName.Contains(searchValue)
                //                        || m.LastName.Contains(searchValue)
                //                        || m.Contact.Contains(searchValue)
                //                        || m.Email.Contains(searchValue));
                //}
                recordsTotal = customerData.Count();
                var data = customerData.Skip(skip).Take(pageSize).ToList();
                var jsonData = new
                {
                    draw = draw,
                    recordsFiltered = recordsTotal,
                    recordsTotal = recordsTotal,
                    data = data
                };
                return Ok(jsonData);
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        [HttpGet]
        public async Task<IActionResult> Manage(string id)
        {
            ShowPassMessage();
            EmployeeModel model = new EmployeeModel();

            if (!string.IsNullOrEmpty(id))
            {
                var eId = Convert.ToInt32(id);//Convert.ToInt32(SecurityHelper.Decrypt(id));
                model = await _employeeServices.GetEmployeeById(eId);
                model.States = await _commonServices.GetStatesByCountryId(model.CountryId);
                model.Cities = await _commonServices.GetCitiesByStateId(model.StateId);
                model.EncrytedId = id;
                model.Id = eId;
                model.EmpId = eId;
            }

            model.Countries = await _commonServices.GetCountries();
            //model.CompanyRoles = await _companyRolesServices.GetRolesByCompanyId(HttpContext.Session.GetComplexData<int>(SessionConstants.CompanyId));
            model.Titles = Common.StaticMethod.Common.CreateDropDownForName<Common.Enums.TitlesEnum>();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Manage(EmployeeModel model)
        {
            try
            {
                model.Id = model.EmpId; //Convert.ToInt32(SecurityHelper.Decrypt(model.EncrytedId));
                string uniqueFileName = UploadedFile(model);
                model.ProfilePicture = uniqueFileName;
                model.CompanyId = HttpContext.Session.GetComplexData<int>(SessionConstants.CompanyId);
                model.CreatedBy = HttpContext.Session.GetComplexData<string>(SessionConstants.UserId);
                model.ModifiedBy = model.CreatedBy;
                await _employeeServices.ManageEmployee(model);
                SuccessPassMessage(MessageConstants.SavedSuccessfully);
            }
            catch (Exception ex)
            {
                model.Countries = await _commonServices.GetCountries();
                model.States = await _commonServices.GetStatesByCountryId(model.CountryId);
                model.Cities = await _commonServices.GetCitiesByStateId(model.StateId);
                //model.CompanyRoles = await _companyRolesServices.GetRolesByCompanyId(HttpContext.Session.GetComplexData<int>(SessionConstants.CompanyId));
                model.Titles = Common.StaticMethod.Common.CreateDropDownForName<Common.Enums.TitlesEnum>();
                FailMessage(ex.Message);
                return View(model);
            }
            return RedirectToAction("Index");
        }


        private string UploadedFile(EmployeeModel model)
        {
            string uniqueFileName = null;

            if (model.ProfileImage != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ProfileImage.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.ProfileImage.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {

            var result = await _employeeServices.DeleteEmployeeById(id);
            SuccessPassMessage(MessageConstants.DeleteSuccessfully);
            if (result)
                return Json(new { result = true, url = Url.Action("Index", "Employee") });
            else
                return Json(new { result = false });
            //return RedirectToAction("Index");
        }

        public async Task<IActionResult> Dashboard() //Not Usable
        {
            ShowPassMessage();
            List<EmployeesTaskModel> model = new List<EmployeesTaskModel>();
            try
            {
                model = await _employeeServices.GetEmployeeTaskById(HttpContext.Session.GetComplexData<int>(SessionConstants.EmployeeId));

            }
            catch (Exception ex)
            {

            }
            return View(model);
        }

        public IActionResult AdminDashboard()
        {
            return View();
        }
        public async Task<IActionResult> ResetEmployeePassword(int id, string email)
        {
            try
            {
                //EmployeeModel model = new EmployeeModel();

                ////generate random password
                //var randomPassword = GeneratePasswordHelper.CreatePassword(7);

                //model.CompanyId = HttpContext.Session.GetComplexData<int>(SessionConstants.CompanyId);
                //model.CreatedBy = HttpContext.Session.GetComplexData<string>(SessionConstants.UserId);
                //model.IsResetPwd = true;
                //model.Id = id;
                //model.Email = email;
                //model.Password = randomPassword;


                ////var decryptUserName = SecurityHelper.Decrypt(model.UserName);
                ////model.UserName = decryptUserName;

                //await _employeeServices.ManageEmployee(model);
                var encryptUserName = SecurityHelper.Encrypt(email);

                var lnkHref = "<a href='" + Url.Action("ResetPassword", "Account", new { email = encryptUserName }, Request.Scheme) + "'>Reset Password</a>";
                //HTML Template for Send email
                string subject = "Reset Password";
                string body = "<b>Please find the Password Reset Link. </b><br/>" + lnkHref;

                CommonHelper.SendEmail(options, email, subject, body, null);
                ViewBag.Message = "Reset password link has been sent to your email id.";
                SuccessPassMessage(ViewBag.Message);

                // CommonHelper.SendEmail(options, email, "Reset Password", "Your updated password is: " + randomPassword, null);
                // SuccessPassMessage("Reset Password Successfully");
               

                return Json(new { result = true, url = Url.Action("Index", "Employee") });
            }
            catch (Exception ex)
            {
                FailMessage(ex.Message);
                return Json(new { result = false });
            }
        }
    }
}
