using Microsoft.AspNetCore.Mvc;

namespace JDSWeb.Controllers
{
    public class SocietyController : Controller
    {
        public IActionResult Committee()
        {
            return View();
        }
    }
}
