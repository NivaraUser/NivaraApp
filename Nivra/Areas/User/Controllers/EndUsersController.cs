using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Nivara.Common.Constants;
using Nivara.Core.Common;
using Nivara.Core.EndUser;
using Nivara.Models;
using Nivara.Web.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Nivara.Web.Controllers;

namespace Nivara.Web.Areas.User.Controllers
{
    [AllowAnonymous]
    [Area("User")]
    public class EndUsersController : BaseController
    {
        private readonly IEndUserService _endUserService;
        private readonly ICommonServices _commonServices;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public EndUsersController(IEndUserService endUserService, ICommonServices commonServices, SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, IWebHostEnvironment hostEnvironment)
        {
            _endUserService = endUserService;
            _commonServices = commonServices;
            _signInManager = signInManager;
            _userManager = userManager;
            _webHostEnvironment = hostEnvironment;
        }
        //public async Task< IActionResult> Index()
        //{
        //   var  model = new List<EndUserModel>();

        //    model= await _endUserService.GetEndUsers();

        //    return View(model);
        //}
        public async Task<IActionResult> Manage(int id, string returnUrl)
        {
            EndUserModel model = new EndUserModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            model.Countries = await _commonServices.GetCountries();
            model.Titles = Common.StaticMethod.Common.CreateDropDownForName<Common.Enums.TitlesEnum>();
            if (id > 0)
            {

            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Manage(EndUserModel model)
        {
            var result = false;
            List<UsersTaskDocumentModel> fileDetails = new List<UsersTaskDocumentModel>();
            if (ModelState.IsValid)
            {
                if (model.Files != null)
                {
                    string uniqueFileName = UploadedFile(model.Files.FirstOrDefault());
                    model.ProfilePiture = uniqueFileName;
                }

               result  = await _endUserService.ManageEndUser(model);
            }
            if(result)
            return RedirectToAction("Login", "EndUsers");
            else
                return RedirectToAction("Manage", "EndUsers");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
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
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel user)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(user.Email, user.Password, user.RememberMe, false);
                if (result.Succeeded)
                {
                    var loginUser = await _endUserService.GetEndUserByEmailId(user.Email);
                    if (loginUser.Email == null)
                    {
                        ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
                    }
                    else
                    {
                        HttpContext.Session.SetComplexData(SessionConstants.UserId, loginUser.AspNetUserId);
                        HttpContext.Session.SetComplexData(SessionConstants.EndUserId, loginUser.Id);
                        HttpContext.Session.SetString(SessionConstants.EndUserProfilePicture, "/images/" + loginUser.ProfilePiture);
                        //if (loginUser.Id > 0)
                        //    HttpContext.Session.SetComplexData(SessionConstants.EmployeeId, loginUser.Id);
                        return RedirectToAction("DashBoard", "UsersTask", new { Area = "User" });
                    }
                }

                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");

            }
            return View(user);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            HttpContext.Session.Remove(SessionConstants.UserId);
            HttpContext.Session.Remove(SessionConstants.EndUserId);
            HttpContext.Session.Remove(SessionConstants.EndUserProfilePicture);
            return RedirectToAction("Login", "EndUsers");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Profile()
        {
            try
            {
                //var GetEmployeeId = HttpContext.Session.GetString(SessionConstants.EmployeeId);
                var GetEndUserId = HttpContext.Session.GetString(SessionConstants.EndUserId);
                //var GetCompanyId = HttpContext.Session.GetString(SessionConstants.CompanyId);
                if (GetEndUserId != null)
                    return RedirectToAction("EndUserProfile");

                return RedirectToAction("Login", "EndUsers");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }
        }


        public async Task<IActionResult> EndUserProfile()
        {
            EndUserModel model = new EndUserModel();
            var GetEndUserId = HttpContext.Session.GetString(SessionConstants.EndUserId);

            if (string.IsNullOrEmpty(GetEndUserId))
                return RedirectToAction("Error", "Home");
            model.Id = Convert.ToInt32(GetEndUserId);
            model = await _endUserService.GetEndUsersById(model.Id);

            model.Countries = await _commonServices.GetCountries();
            model.States = await _commonServices.GetStatesByCountryId(model.CountryId);
            model.Cities = await _commonServices.GetCitiesByStateId(model.StateId);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EndUserProfile(EndUserModel model)
        {
            try
            {
                var GetEndUserId = HttpContext.Session.GetString(SessionConstants.EndUserId);

                if (!string.IsNullOrEmpty(GetEndUserId))
                    model.Id = Convert.ToInt32(GetEndUserId);
                else
                    return RedirectToAction("Error", "Home");

                string uniqueFileName = UploadedFile(model.Files.FirstOrDefault());
                model.ProfilePiture = uniqueFileName;
                await _endUserService.UpdateEndUserProfile(model);

                //await _companiesServices.UpdateCompanies(model);
                if (!string.IsNullOrEmpty(model.ProfilePiture))
                    HttpContext.Session.SetString(SessionConstants.EndUserProfilePicture, "/images/" + model.ProfilePiture);
                SuccessPassMessage(MessageConstants.SavedSuccessfully);
                return RedirectToAction("DashBoard", "UsersTask", new { Area = "User" });
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

        [AllowAnonymous]
        [HttpPost]
        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "EndUsers", new { ReturnUrl = returnUrl });
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
                return RedirectToAction("DashBoard", "UsersTask", new { Area = "User" });
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
                    return RedirectToAction("DashBoard", "UsersTask", new { Area = "User" });


                }
                ViewBag.ErrorTitle = $"Email claim not receved from : {info.LoginProvider}";
                return View("Error");
            }

        }
    }
}
