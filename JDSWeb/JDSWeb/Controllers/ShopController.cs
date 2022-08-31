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
            ClothType[] types = ctx.ClothTypes.Fetch();
            ClothSize[] sizes = ctx.ClothSizes.Fetch();

            ctx.Dispose();

            ShopViewModel vm = new ShopViewModel
            {
                Clothes = clothes,
                ClothSizes = sizes,
                ClothTypes = types.OrderBy(t => t.Name).ToArray(),
            };

            return View(vm);
        }
    }
}
