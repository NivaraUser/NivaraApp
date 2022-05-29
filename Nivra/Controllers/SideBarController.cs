using Microsoft.AspNetCore.Mvc;
using Nivara.Common.Constants;
using Nivara.Core.SideBar;
using Nivara.Web.Helper;
using System.Threading.Tasks;

namespace Nivara.Web.Controllers
{
    //public class SideBarController : Controller
    //{
    //    private readonly ISideBar _sideBar;
    //    public SideBarController(ISideBar sideBar)
    //    {
    //        _sideBar = sideBar;
    //    }
    //    public async Task<IActionResult> Index()
    //    {
    //        var result = await _sideBar.GetModulesByUserId(HttpContext.Session.GetComplexData<string>(SessionConstants.UserId));
    //        return PartialView(result);
    //    }
    //}

    public class sideBarViewComponent : ViewComponent
    {
        private readonly ISideBar _sideBar;
        public sideBarViewComponent(ISideBar sideBar)
        {
            _sideBar = sideBar;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var result = await _sideBar.GetModulesByUserId(HttpContext.Session.GetComplexData<string>(SessionConstants.UserId));
            return View("~/Views/Components/sideBar/Default.cshtml", result);
        }
    }
}
