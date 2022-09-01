using JDSCommon.Database;
using JDSCommon.Database.DataContract;
using JDSWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;
using DBUser = JDSCommon.Database.DataContract.User;
using JDSContext = JDSCommon.Database.Models.JDSContext;

namespace JDSWeb.Controllers
{
    public class ShopController : Controller
    {
        public IActionResult Products()
        {
            Request.Cookies.TryGetValue(ShopViewModel.CookieKeyError, out string? error);
            Request.Cookies.TryGetValue(ShopViewModel.CookieKeyArticleAdded, out string? articleAdded);

            Response.Cookies.Delete(ShopViewModel.CookieKeyError);
            Response.Cookies.Delete(ShopViewModel.CookieKeyArticleAdded);

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
                Error = bool.Parse(error ?? "false"),
                ArticleAdded = bool.Parse(articleAdded ?? "false"),
            };

            return View(vm);
        }

        public IActionResult Cart(string step)
        {
            if (HttpContext.Session.GetInt32(UserViewModel.SessionKeyUserId) is null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (ShopViewModel.PossibleViews.Contains(step))
            {
                return View($"Cart.{step}");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IActionResult AddToCart(int cloth_type_id, int? cloth_size)
        {
            JDSContext ctx = new JDSContext();

            Cloth? cloth = ctx.Cloths
                .Fetch()
                .FirstOrDefault(c => c.Type.Id == cloth_type_id && c.Size?.Id == cloth_size && c.Booked is null);

            if (cloth is null)
            {
                ctx.Dispose();

                // Error : This product is not available anymore

                Response.Cookies.Append(ShopViewModel.CookieKeyError, "true");

                return RedirectToAction(nameof(Products));
            }

            // Book this cloth for the user
            int? userId = HttpContext.Session.GetInt32(UserViewModel.SessionKeyUserId);

            if (userId is not null)
            {
                DBUser user = ctx.Users
                    .Fetch()
                    .First(u => u.Id == userId);

                cloth.Booked = user;
            }

            ctx.Cloths.Update(cloth);
            ctx.SaveChanges();

            ctx.Dispose();

            Response.Cookies.Append(ShopViewModel.CookieKeyArticleAdded, "true");

            return RedirectToAction(nameof(Products));
        }
    }
}
