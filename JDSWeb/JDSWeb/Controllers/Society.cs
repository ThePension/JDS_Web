using Microsoft.AspNetCore.Mvc;

namespace JDSWeb.Controllers
{
    public class Society : Controller
    {
        public IActionResult Committee()
        {
            return View();
        }
    }
}
