using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ROWM.Controllers
{
    public class HomeController : Controller
    {
        SiteDecoration _dec;

        public HomeController(SiteDecoration d) => _dec = d;

        public IActionResult Index()
        {
            return View(_dec);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {            
            var p = ClaimsPrincipal.Current;
            var m = string.Join(",", p.Claims.Select(cx => $"{cx.Type}:{cx.Value}"));
            ViewData["Message"] = m;

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
