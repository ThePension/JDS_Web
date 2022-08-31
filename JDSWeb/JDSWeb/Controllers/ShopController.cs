using JDSCommon.Database;
using JDSCommon.Database.DataContract;
using JDSWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IActionResult AddToCart(int cloth_type_id, int? cloth_size)
        {
            JDSContext ctx = new JDSContext();

            Cloth? cloth = ctx.Cloths
                .Fetch()
                .FirstOrDefault(c => c.Type.Id == cloth_type_id && c.Size?.Id == cloth_size);

            if (cloth is null)
            {
                ctx.Dispose();
                Environment.Exit(-1); // TODO : À changer
            }

            // Remove temporarely from database
            ctx.Cloths.Remove(cloth);
            //ctx.SaveChanges();

            

            return View();
        }
    }
}
