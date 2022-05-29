using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Nivara.Common.Constants;
using Nivara.Core.Common;
using Nivara.Core.CompanyDetail;
using Nivara.Core.CompanyRole;
using Nivara.Core.Employee;
using Nivara.Models;
using Nivara.Web.Helper;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Nivara.Common.Helpers;
using Microsoft.Extensions.Options;


namespace Nivara.Web.Controllers
{
    public class AccountController : BaseController
    {

        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ICompaniesServices _companiesServices;
        private readonly ICommonServices _commonServices;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEmployeeServices _employeeServices;
        private readonly ICompanyRolesServices _companyRolesServices;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IOptions<AppSettingsModel> options;
        EmailHelpers emailHelpers;
        ConfigHelper configMgr;
        //private readonly IOptions<AppSettingsModel> _options;
        public AccountController(SignInManager<IdentityUser> signInManager, ICompaniesServices companiesServices, ICommonServices commonServices, IWebHostEnvironment hostEnvironment, IEmployeeServices employeeServices, ICompanyRolesServices companyRolesServices, UserManager<IdentityUser> userManager, IOptions<AppSettingsModel> _options)
        {

            _signInManager = signInManager;
            _companiesServices = companiesServices;
            _commonServices = commonServices;
            _webHostEnvironment = hostEnvironment;
            _employeeServices = employeeServices;
            _companyRolesServices = companyRolesServices;
            _userManager = userManager;
            options = _options;
            configMgr = new ConfigHelper(_options);
            emailHelpers = new EmailHelpers(options);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl)
        {
            ShowPassMessage();
            LoginViewModel model = new LoginViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            return View(model);
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel user)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(user.Email, user.Password, user.RememberMe, false);
                if (result.Succeeded)
                {
                    var loginUser = await _companiesServices.GetCompanyDetailByEmailId(user.Email);
                    if (loginUser.Email == null)
                    {
                        FailMessage("Invalid Login Attempt");
                    }
                    else
                    {
                        HttpContext.Session.SetComplexData(SessionConstants.UserId, loginUser.AspNetUserId);
                        HttpContext.Session.SetComplexData(SessionConstants.CompanyId, loginUser.Id);
                        if (!string.IsNullOrEmpty(loginUser.ProfilePicture))
                            HttpContext.Session.SetString(SessionConstants.ProfilePicture, @loginUser.ProfilePicture);
                        
                        if (loginUser.EmployeeId > 0)
                        {
                            HttpContext.Session.SetComplexData(SessionConstants.EmployeeId, loginUser.EmployeeId);

                            //return RedirectToAction("Dashboard", "Employee");
                            return RedirectToAction("Index", "UsersTask");
                        }

                        //return RedirectToAction("AdminDashboard", "Employee");
                        return RedirectToAction("Index", "PreDefinedTask");
                    }
                }

                FailMessage("Invalid Login Attempt");
            }
            return View(user);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            HttpContext.Session.Remove(SessionConstants.UserId);
            HttpContext.Session.Remove(SessionConstants.CompanyId);
            HttpContext.Session.Remove(SessionConstants.EmployeeId);
            HttpContext.Session.Remove(SessionConstants.ProfilePicture);
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Register(string returnUrl)
        {
            ShowPassMessage();
            RegisterViewModel model = new RegisterViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            model.Countries = await _commonServices.GetCountries();
            return View(model);
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            try
            {
                await _companiesServices.CreateCompanies(model);
                if (!string.IsNullOrEmpty(model.Email))
                {
                    var loginUser = await _companiesServices.GetCompanyDetailByEmailId(model.Email);
                    HttpContext.Session.SetComplexData(SessionConstants.UserId, loginUser.AspNetUserId);
                    HttpContext.Session.SetComplexData(SessionConstants.CompanyId, loginUser.Id);
                    if (loginUser.EmployeeId > 0)
                        HttpContext.Session.SetComplexData(SessionConstants.EmployeeId, loginUser.EmployeeId);
                    // CommonHelper.SendEmail(options, "simranjitsingh403@gmail.com", "test", "<p>hello..</p>", null);
                    var  response = CommonHelper.SendEmail(options, model.Email, "Welcome Mail", "Welcome to nivara", null);

                    //var resu = emailHelpers.SendEmail("ambikachauhan02@gmail.com", "Testing mail", "abcsgfsjdfskdfsdsdhgf");
                }

            }
            catch (Exception ex)
            {
                model.Countries = await _commonServices.GetCountries();
                model.States = await _commonServices.GetStatesByCountryId(model.CountryId);
                model.Cities = await _commonServices.GetCitiesByStateId(model.StateId);
                FailMessage("ghfjhg");
                return RedirectToAction("Register");
            }
            SuccessPassMessage(MessageConstants.WelcomeMessage);
            return RedirectToAction("Index", "Employee");

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

        #region Profile

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Profile()
        {
            try
            {
                var GetEmployeeId = HttpContext.Session.GetString(SessionConstants.EmployeeId);
              
                var GetCompanyId = HttpContext.Session.GetString(SessionConstants.CompanyId);
                if (!string.IsNullOrEmpty(GetEmployeeId))
                    return RedirectToAction("EmployeeProfile");
                else if (!string.IsNullOrEmpty(GetCompanyId))
                    return RedirectToAction("CompanyProfile");

                return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                FailMessage(ex.Message);
                return RedirectToAction("Error", "Home");
            }
        }

        public async Task<IActionResult> CompanyProfile()
        {
            RegisterViewModel model = new RegisterViewModel();
            var GetCompanyId = HttpContext.Session.GetString(SessionConstants.CompanyId);

            if (string.IsNullOrEmpty(GetCompanyId))
                return RedirectToAction("Error", "Home");
            model.Id = Convert.ToInt32(GetCompanyId);
            model = await _companiesServices.GetCompanyById((int)model.Id);
            model.Countries = await _commonServices.GetCountries();
            model.States = await _commonServices.GetStatesByCountryId(model.CountryId);
            model.Cities = await _commonServices.GetCitiesByStateId(model.StateId);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CompanyProfile(RegisterViewModel model)
        {
            try
            {
                var GetCompanyId = HttpContext.Session.GetString(SessionConstants.CompanyId);
                if (!string.IsNullOrEmpty(GetCompanyId))
                    model.Id = Convert.ToInt32(GetCompanyId);
                else
                    return RedirectToAction("Error", "Home");

                string uniqueFileName = UploadedFile(model.ProfileImage);
                model.ProfilePiture = uniqueFileName;
                await _companiesServices.UpdateCompanies(model);
                if (!string.IsNullOrEmpty(model.ProfilePiture))
                    HttpContext.Session.SetString(SessionConstants.ProfilePicture, "/images/" + model.ProfilePiture);
                SuccessPassMessage(MessageConstants.SavedSuccessfully);
                return RedirectToAction("AdminDashboard", "Employee");
            }
            catch (Exception ex)
            {
                model.Countries = await _commonServices.GetCountries();
                model.States = await _commonServices.GetStatesByCountryId(model.CountryId);
                model.Cities = await _commonServices.GetCitiesByStateId(model.StateId);
                FailPassMessage(MessageConstants.GeneralErrorMessage);
                return View(model);
            }
        }

        public async Task<IActionResult> EmployeeProfile()
        {
            EmployeeModel model = new EmployeeModel();
            var GetEmployeeId = HttpContext.Session.GetString(SessionConstants.EmployeeId);
            if (string.IsNullOrEmpty(GetEmployeeId))
                return RedirectToAction("Error", "Home");
            model.Id = Convert.ToInt32(GetEmployeeId);
            model = await _employeeServices.GetEmployeeById(model.Id);
            model.States = await _commonServices.GetStatesByCountryId(model.CountryId);
            model.Cities = await _commonServices.GetCitiesByStateId(model.StateId);

            model.Countries = await _commonServices.GetCountries();
            model.CompanyRoles = await _companyRolesServices.GetRolesByCompanyId(HttpContext.Session.GetComplexData<int>(SessionConstants.CompanyId));

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EmployeeProfile(EmployeeModel model)
        {
            try
            {
                var GetEmployeeId = HttpContext.Session.GetString(SessionConstants.EmployeeId);
                if (string.IsNullOrEmpty(GetEmployeeId))
                    return RedirectToAction("Error", "Home");
                model.Id = Convert.ToInt32(GetEmployeeId);

                string uniqueFileName = UploadedFile(model.ProfileImage);
                model.ProfilePicture = uniqueFileName;
                model.CompanyId = HttpContext.Session.GetComplexData<int>(SessionConstants.CompanyId);
                model.CreatedBy = HttpContext.Session.GetComplexData<string>(SessionConstants.UserId);

                model.ModifiedBy = model.CreatedBy;
                await _employeeServices.ManageEmployee(model);
                if (!string.IsNullOrEmpty(model.ProfilePicture))
                    HttpContext.Session.SetString(SessionConstants.ProfilePicture, "/images/" + model.ProfilePicture);
                SuccessPassMessage(MessageConstants.SavedSuccessfully);
                return RedirectToAction("Profile");
            }
            catch (Exception ex)
            {
                model.Countries = await _commonServices.GetCountries();
                model.States = await _commonServices.GetStatesByCountryId(model.CountryId);
                model.Cities = await _commonServices.GetCitiesByStateId(model.StateId);
                model.CompanyRoles = await _companyRolesServices.GetRolesByCompanyId(HttpContext.Session.GetComplexData<int>(SessionConstants.CompanyId));
                FailPassMessage(MessageConstants.GeneralErrorMessage);
                return View(model);
            }
        }
        #endregion

        public async Task<IActionResult> Edit()
        {
            string username = User.Identity.Name;

            // Fetch the userprofile
            var user = await _companiesServices.GetCompanyDetailByEmailId(username);
            //UserProfile user = db.UserProfiles.FirstOrDefault(u => u.UserName.Equals(username));

            // Construct the viewmodel
            RegisterViewModel model = new RegisterViewModel();
            model.Name = user.Name;
            model.Email = user.Email;
            model.Website = user.Website;
            model.PhoneNo = user.PhoneNo;
            model.Address = user.Address;
            model.CityId = user.CityId;
            model.PostalCode = user.PostalCode;

            return View(model);
        }
        [AllowAnonymous]
        [HttpPost]
        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }
        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {

            LoginViewModel model = new LoginViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };
            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"error from external provider: { remoteError}");
                return RedirectToAction(nameof(Login));
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Login));
            }
            var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (signInResult.Succeeded)
            {
                return RedirectToAction("Dashboard", "Employee");
            }

            else
            {
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                if (email == null)
                    email = info.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
                if (email != null)
                {
                    var user = await _userManager.FindByEmailAsync(email);
                    if (user == null)
                    {
                        user = new IdentityUser
                        {
                            UserName = email,
                            Email = email
                        };

                        var createUser = await _userManager.CreateAsync(user);
                    }
                    await _userManager.AddLoginAsync(user, info);
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Dashboard", "Employee");


                }
                ViewBag.ErrorTitle = $"Email claim not receved from : {info.LoginProvider}";
                return View("Error");
            }

        }

        public async Task<ActionResult> ForgotPassword(string UserName)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(UserName);
                if (user != null)
                {
                    try
                    {
                        var encryptUserName = SecurityHelper.Encrypt(UserName);
                        var lnkHref = "<a href='" + Url.Action("ResetPassword", "Account", new { email = encryptUserName }, Request.Scheme) + "'>Reset Password</a>";
                        //HTML Template for Send email
                        string subject = "Reset Password";
                        string body = "<b>Please find the Password Reset Link. </b><br/>" + lnkHref;

                        CommonHelper.SendEmail(options, UserName, subject, body, null);
                        ViewBag.Message = "Reset password link has been sent to your email id.";
                        SuccessPassMessage(ViewBag.Message);

                    }
                    catch (Exception ex)
                    {

                        FailPassMessage(ViewBag.Message);
                    }
                }
            }
            return RedirectToAction("Login");
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> ResetPassword(string email)
        {
            ShowPassMessage();
            ResetPasswordModel model = new ResetPasswordModel();
            // model.ResetCode = code;
            model.UserName = email;
            return View(model);
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            bool result = false;
            try
            {
                var decryptUserName = SecurityHelper.Decrypt(model.UserName);
                var user = await _userManager.FindByEmailAsync(decryptUserName);
                if (user != null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var resetPassResult = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
                    if (resetPassResult.Succeeded)
                        SuccessPassMessage("Password reset successfuly");
                    else
                    {
                        //var encryptUserName = SecurityHelper.Encrypt(UserName);
                        FailMessage("dfshdgfds");
                        return RedirectToAction("ResetPassword", "Account", new { email = model.UserName });
                       
                    }

                }
            }
            catch (Exception ex)
            {
                FailMessage(ex.Message);
            }
            return RedirectToAction("Login");
        }

    }
}
