using JDSCommon.Database;
using JDSCommon.Database.DataContract;
using JDSWeb.Models;
using Microsoft.AspNetCore.Mvc;
using JDSContext = JDSCommon.Database.Models.JDSContext;

namespace JDSWeb.Controllers
{
    public class ShopController : Controller
    {
        public IActionResult Products()
        {
            JDSContext ctx = new JDSContext();

            Cloth[] clothes = ctx.Cloths.Fetch();

            ctx.Dispose();

            ShopViewModel vm = new ShopViewModel
            {
                Clothes = clothes
            };

            return View(vm);
        }
    }
}
