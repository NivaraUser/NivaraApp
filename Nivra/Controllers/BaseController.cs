using Microsoft.AspNetCore.Mvc;

namespace Nivara.Web.Controllers
{
    public class BaseController : Controller
    {

        public void SuccessMessage(string text)
        {
            ViewData["SuccessMessage"] = text;
        }
        public void FailMessage(string text)
        {
            ViewData["ErrorMessage"] = text;
        }
        public void WarningMessage(string text)
        {
            ViewData["WarningMessage"] = text;
        }
        public void SuccessPassMessage(string text)
        {
            TempData["SuccessPassMessage"] = text;
        }
        public void FailPassMessage(string text)
        {
            TempData["ErrorPassMessage"] = text;
        }
        public void WarningPassMessage(string text)
        {
            TempData["WarningPassMessage"] = text;
        }

        public void ShowPassMessage()
        {
            if (TempData["SuccessPassMessage"] != null)
            {
                SuccessMessage(TempData["SuccessPassMessage"].ToString());
                TempData["SuccessPassMessage"] = null;
            }
               
            if (TempData["ErrorPassMessage"] != null)
            {
                FailMessage(TempData["ErrorPassMessage"].ToString());
                TempData["ErrorPassMessage"] = null;
            }
               
            if (TempData["WarningPassMessage"] != null)
            {
                WarningMessage(TempData["WarningPassMessage"].ToString());
                TempData["WarningPassMessage"] = null;
            }
               
        }
    }
}
