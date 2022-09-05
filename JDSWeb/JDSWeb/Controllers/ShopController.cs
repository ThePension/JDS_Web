using JDSCommon.Api;
using JDSCommon.Database;
using JDSCommon.Database.DataContract;
using JDSCommon.Settings;
using JDSWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Newtonsoft.Json;
using System.Globalization;
using System.Runtime.CompilerServices;
using DBUser = JDSCommon.Database.DataContract.User;
using JDSContext = JDSCommon.Database.Models.JDSContext;

namespace JDSWeb.Controllers
{
    public class ShopController : Controller
    {
		private readonly object _locker = new object();

		#region GET Actions

		public IActionResult Index()
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

		public IActionResult Cart(string id)
		{
			int? userId = HttpContext.Session.GetInt32(UserViewModel.SessionKeyUserId);

			Request.Cookies.TryGetValue(ShopViewModel.CookieKeyError, out string? error);
			Response.Cookies.Delete(ShopViewModel.CookieKeyError);

			if (userId is null)
			{
				return RedirectToAction("Index", "Home");
			}

			JDSContext ctx = new JDSContext();

			DBUser user = ctx.Users.Fetch().First(u => u.Id == userId);
			Cloth[] clothes = ctx.Cloths
				.Fetch()
				.Where(c => c.Booked?.Id == userId)
				.ToArray();

			ctx.Dispose();

			ShopViewModel vm = new ShopViewModel
			{
				Clothes = clothes,
				Error = bool.Parse(error ?? "false"),
				User = user,
			};

			if (ShopViewModel.PossibleViews.Contains(id))
			{
				return View($"Cart.{id}", vm);
			}
			else
			{
				return RedirectToAction(nameof(Cart), "Shop", new { id = ShopViewModel.Overview });
			}
		}

		public IActionResult Confirmation()
		{
			if (!Request.Cookies.TryGetValue(ShopViewModel.CookieKeyConfirmation, out string? confirmation) || !bool.Parse(confirmation ?? "false"))
			{
				return RedirectToAction(nameof(Cart), "Shop", new { id = ShopViewModel.Overview });
			}

			Response.Cookies.Delete(ShopViewModel.CookieKeyConfirmation);

			return View();
		}

		

		public IActionResult RemoveFromCart(int cloth_id)
		{
			int? userId = HttpContext.Session.GetInt32(UserViewModel.SessionKeyUserId);

			if (userId is null)
			{
				return RedirectToAction("Index", "Home");
			}

			JDSContext ctx = new JDSContext();

			Cloth? cloth = ctx.Cloths
				.Fetch()
				.FirstOrDefault(c => c.Id == cloth_id);

			if (cloth is null || cloth.Booked?.Id != userId)
			{
				// Error : This product is not booked by the user

				Response.Cookies.Append(ShopViewModel.CookieKeyError, "true");
			}
			else
			{
				cloth.Booked = null;

				ctx.Cloths.Update(cloth);
				ctx.SaveChanges();
			}

			ctx.Dispose();

			return RedirectToAction(nameof(Cart), "Shop", new { id = ShopViewModel.Overview });
		}

		#endregion

		#region POST Actions

		[HttpPost]
		[ValidateAntiForgeryToken]
		[MethodImpl(MethodImplOptions.Synchronized)]
		public IActionResult AddToCart(int cloth_type_id, int? cloth_size)
		{
			lock (_locker)
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

					return RedirectToAction(nameof(Index));
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

				return RedirectToAction(nameof(Index));
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult ParseValidate(string first_name, string last_name, string email)
		{
			int? userId = HttpContext.Session.GetInt32(UserViewModel.SessionKeyUserId);

			if (userId is null)
			{
				return RedirectToAction("Index", "Home");
			}

			JDSContext ctx = new JDSContext();

			Cloth[] clothes = ctx.Cloths
				.Fetch()
				.Where(c => c.Booked?.Id == userId)
				.ToArray();

			ctx.Cloths.RemoveRange(clothes);
			ctx.SaveChanges();

			ctx.Dispose();

			// Email to shop responsable
			EMailMessage responsableEmail = new EMailMessage(Keys.ShopResponsable)
				.AddSubject("Commande de vêtements")
				.AddLine("Une nouvelle commande a été éffectuée sur le site.")
				.AddEmptyLine()
				.AddLine($"- <b>Client :</b> {first_name} {last_name}")
				.AddLine($"- <b>Email :</b> {email}")
				.AddEmptyLine()
				.AddLine("La commande est composée des articles suivants :");

			// Email to customer
			EMailMessage customerEmail = new EMailMessage(email)
				.AddSubject("Commande de vêtements")
				.AddLine("Votre commande a bien été reçue, nous allons la traiter au plus vite.")
				.AddEmptyLine()
				.AddLine("Récapitulatif de la commande :");

			// Fill both with same content
			foreach (Cloth cloth in clothes)
			{
				responsableEmail.AddLine($"- {cloth.Type.Name} {cloth.Type.Color.Name} / {(cloth.Size is not null ? $"Taille : {cloth.Size.Shortcut} /" : "")} {cloth.Type.Price.ToString("0.00", new CultureInfo("en-US", false))} CHF");
				customerEmail.AddLine($"- {cloth.Type.Name} {cloth.Type.Color.Name} / {(cloth.Size is not null ? $"Taille : {cloth.Size.Shortcut} /" : "")} {cloth.Type.Price.ToString("0.00", new CultureInfo("en-US", false))} CHF");
			}

			responsableEmail
				.AddEmptyLine()
				.AddLine($"Total : {clothes.Sum(c => c.Type.Price).ToString("0.00", new CultureInfo("en-US", false))} CHF")
				.AddEmptyLine()
				.AddLine("La bise Deub 😘");

			customerEmail
				.AddEmptyLine()
				.AddLine($"Total : {clothes.Sum(c => c.Type.Price).ToString("0.00", new CultureInfo("en-US", false))} CHF")
				.AddEmptyLine()
				.AddLine("Toute bonne journée,")
				.AddLine("La Jeunesse de Savagnier");

			// Send emails
			responsableEmail.Send();
			customerEmail.Send();

			Response.Cookies.Append(ShopViewModel.CookieKeyConfirmation, "true");

			return RedirectToAction(nameof(Confirmation));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Route("Shop/Cart/Validation")]
		public IActionResult Validate(string first_name, string last_name, string email)
		{
			int? userId = HttpContext.Session.GetInt32(UserViewModel.SessionKeyUserId);

			if (userId is null)
			{
				return RedirectToAction("Index", "Home");
			}

			JDSContext ctx = new JDSContext();

			DBUser user = ctx.Users.Fetch().First(u => u.Id == userId);
			Cloth[] clothes = ctx.Cloths
				.Fetch()
				.Where(c => c.Booked?.Id == userId)
				.ToArray();

			ctx.Dispose();

			ShopViewModel vm = new ShopViewModel
			{
				User = user,
				Clothes = clothes,
				FirstName = first_name,
				LastName = last_name,
				Email = email,
			};

			return View("Cart.Validation", vm);
		}

		#endregion
	}
}
