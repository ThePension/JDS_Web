using JDSWeb.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace JDSWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            Request.Cookies.TryGetValue(UserViewModel.CookieKeyLoggedIn, out string? loggedIn);
            Request.Cookies.TryGetValue(UserViewModel.CookieKeyLoggedOut, out string? loggedOut);
            
            Response.Cookies.Delete(UserViewModel.CookieKeyLoggedIn);
            Response.Cookies.Delete(UserViewModel.CookieKeyLoggedOut);

            HomeViewModel vm = new HomeViewModel
            {
                UserLoggedIn = bool.Parse(loggedIn ?? "false"),
                UserLoggedOut = bool.Parse(loggedOut ?? "false"),
            };

            return View(vm);

        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult ActualEvents()
        {
            return View();
        }

        

        public IActionResult Members()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Developpers()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}