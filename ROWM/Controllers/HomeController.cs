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
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
