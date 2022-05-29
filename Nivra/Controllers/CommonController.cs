using Microsoft.AspNetCore.Mvc;
using Nivara.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nivara.Web.Controllers
{
    public class CommonController : Controller
    {
        private readonly ICommonServices _commonServices;
        public CommonController(ICommonServices commonServices)
        {
            _commonServices = commonServices;
        }
        public async Task<IActionResult> GetStatesByCountryId(int id=0)
        {
            var states = await _commonServices.GetStatesByCountryId(id);
            return Json(states);
        }

        public async Task<IActionResult> GetCitiesByStateId(int id = 0)
        {
            var cities = await _commonServices.GetCitiesByStateId(id);
            return Json(cities);
        }
    }
}
