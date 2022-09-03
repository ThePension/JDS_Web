using JDSCommon.Api;
using JDSCommon.Database;
using JDSCommon.Database.DataContract;
using JDSCommon.Database.Models;
using JDSCommon.Services;
using JDSCommon.Services.Extensions;
using JDSWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Configuration;
using DBUser = JDSCommon.Database.DataContract.User;
using JDSContext = JDSCommon.Database.Models.JDSContext;
using Role = JDSCommon.Database.DataContract.Role;
using User = JDSCommon.Database.DataContract.User;

namespace JDSWeb.Controllers
{
	public class UserController : Controller
	{
		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           PUBLIC METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		#region IActionResult Methods

		public IActionResult Login()
		{
			if (HttpContext.Session.GetInt32(UserViewModel.SessionKeyUserId) is not null)
			{
				return RedirectToAction("Index", "Home");
			}

			Request.Cookies.TryGetValue(UserViewModel.CookieKeyUsername, out string? username);
			Request.Cookies.TryGetValue(UserViewModel.CookieKeyError, out string? error);
			Request.Cookies.TryGetValue(UserViewModel.CookieKeyPasswordResetSuccess, out string? passwordResetSuccess);

			Response.Cookies.Delete(UserViewModel.CookieKeyUsername);
			Response.Cookies.Delete(UserViewModel.CookieKeyError);
			Response.Cookies.Delete(UserViewModel.CookieKeyPasswordResetSuccess);

			UserViewModel vm = new UserViewModel
			{
				Username = username,
				Error = bool.Parse(error ?? "false"),
				PasswordResetSuccess = bool.Parse(passwordResetSuccess ?? "false"),
			};

			return View(vm);
		}

		public IActionResult Logout()
		{
			HttpContext.Session.Remove(UserViewModel.SessionKeyUserId);
			HttpContext.Session.Remove(UserViewModel.SessionKeyUserName);
			HttpContext.Session.Remove(UserViewModel.SessionKeyUserRole);

			Response.Cookies.Append(UserViewModel.CookieKeyLoggedOut, "true");

			return RedirectToAction("Index", "Home");
		}

		[Route("Users")]
		public IActionResult List()
		{
			if ((ERole)(HttpContext.Session.GetInt32(UserViewModel.SessionKeyUserRole) ?? -1) < ERole.Manager)
			{
				return RedirectToAction("Index", "Home");
			}

			UserViewModel vm = new UserViewModel
			{
				Users = FetchUsers(),
			};

			return View(vm);
		}

		public IActionResult Create()
		{
			ERole role = (ERole)(HttpContext.Session.GetInt32(UserViewModel.SessionKeyUserRole) ?? -1);

			if (role < ERole.Manager)
			{
				return RedirectToAction("Index", "Home");
			}

			string rndPw = SecurityService.GeneratePassword(12);

			return View("Create", rndPw);
		}

		public IActionResult Update(int id)
		{
			ERole role = (ERole)(HttpContext.Session.GetInt32(UserViewModel.SessionKeyUserRole) ?? -1);
			int userId = HttpContext.Session.GetInt32(UserViewModel.SessionKeyUserId) ?? -1;

			if (role < ERole.Manager && userId != id)
			{
				return RedirectToAction("Index", "Home");
			}

			User? user = FetchUserById(id);

			return (user is null) ? RedirectToAction(nameof(List)) : View(user);
		}

		public IActionResult Delete(int id)
		{
			if ((ERole)(HttpContext.Session.GetInt32(UserViewModel.SessionKeyUserRole) ?? -1) < ERole.Manager)
			{
				return RedirectToAction("Index", "Home");
			}

			User? user = FetchUserById(id);

			if (user is null)
			{
				return RedirectToAction(nameof(List));
			}

			JDSContext ctx = new JDSContext();

			ctx.Users.Remove(user);
			ctx.SaveChanges();
			ctx.Dispose();

			return RedirectToAction(nameof(List));
		}

		public IActionResult ForgotPassword()
		{
			if (HttpContext.Session.GetInt32(UserViewModel.SessionKeyUserId) is not null)
			{
				return RedirectToAction("Index", "Home");
			}

			Request.Cookies.TryGetValue(UserViewModel.CookieKeyError, out string? error);
			Response.Cookies.Delete(UserViewModel.CookieKeyError);

			UserViewModel vm = new UserViewModel
			{
				Error = bool.Parse(error ?? "false"),
			};

			return View(vm);
		}

		public IActionResult ResetPassword()
		{
			if (HttpContext.Session.GetInt32(UserViewModel.SessionKeyUserResetPasswordCode) is null)
			{
				return RedirectToAction("Index", "Home");
			}

			Request.Cookies.TryGetValue(UserViewModel.CookieKeyError, out string? error);
			Response.Cookies.Delete(UserViewModel.CookieKeyError);

			UserViewModel vm = new UserViewModel
			{
				Error = bool.Parse(error ?? "false"),
			};

			return View(vm);
		}

		[Route("User/ForgotPassword/ResetCode")]
		public IActionResult ResetCode()
		{
			if (HttpContext.Session.GetInt32(UserViewModel.SessionKeyUserResetPasswordCode) is null)
			{
				return RedirectToAction("Index", "Home");
			}

			Request.Cookies.TryGetValue(UserViewModel.CookieKeyError, out string? error);
			Response.Cookies.Delete(UserViewModel.CookieKeyError);

			UserViewModel vm = new UserViewModel
			{
				Error = bool.Parse(error ?? "false"),
			};

			return View(vm);
		}

		#endregion

		#region POST Actions

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult ParseEmail(string email)
		{
			JDSContext ctx = new JDSContext();

			User? user = ctx.Users
				.Fetch()
				.FirstOrDefault(u => u.Email == email);

			ctx.Dispose();

			if (user is null)
			{
				Response.Cookies.Append(UserViewModel.CookieKeyError, "true");

				return RedirectToAction(nameof(ForgotPassword));
			}

			// Generate random code
			string code = SecurityService.GeneratePassword(6);

			EMailMessage mail = new EMailMessage(email)
				.AddSubject("Réinitialisation du mot de passe")
				.AddLine($"Bonjour {user.Username},")
				.AddEmptyLine()
				.AddLine($"Voici votre code de réinitialisation : {code}.");

			// Provide random generated code to the user
			mail.Send();

			// Keep track of the user id and reset code
			HttpContext.Session.SetString(UserViewModel.SessionKeyUserResetPasswordCode, code);
			HttpContext.Session.SetInt32(UserViewModel.SessionKeyUserResetPasswordId, user.Id);

			return RedirectToAction(nameof(ResetCode));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult ParseLogin(string username, string password)
		{
			DBUser? user = FetchUserByUsername(username);

			string hPwd = password.ToSHA256();

			if (user is null || hPwd != user.Password)
			{
				Response.Cookies.Append(UserViewModel.CookieKeyUsername, username);
				Response.Cookies.Append(UserViewModel.CookieKeyError, "true");

				return RedirectToAction(nameof(Login));
			}

			Response.Cookies.Delete(UserViewModel.CookieKeyUsername);
			Response.Cookies.Delete(UserViewModel.CookieKeyError);

			HttpContext.Session.SetInt32(UserViewModel.SessionKeyUserId, user.Id);
			HttpContext.Session.SetInt32(UserViewModel.SessionKeyUserRole, user.Role.Id);
			HttpContext.Session.SetString(UserViewModel.SessionKeyUserName, user.Username);

			Response.Cookies.Append(UserViewModel.CookieKeyLoggedIn, "true");

			return RedirectToAction("Index", "Home");
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult ParseCreate(string username, string email, string password, int role, string newsletter)
		{
			JDSContext ctx = new JDSContext();

			ctx.Users.Add(new User
			{
				Username = username,
				Email = email,
				Password = password.ToSHA256(),
				Role = new Role { ERole = (ERole)role, },
				Newsletter = newsletter == "on" ? true : false,
			});

			ctx.SaveChanges();

			ctx.Dispose();

			return RedirectToAction(nameof(List));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult ParseUpdate(int id, string username, string email, string password, int role, string newsletter)
		{
			User? user = FetchUserById(id);

			if (user is null)
			{
				return RedirectToAction(nameof(List));
			}

			user.Username = username;
			user.Email = email;
			user.Role = new Role { ERole = (ERole)role, };
			user.Newsletter = newsletter == "on" ? true : false;

			if (password is not null && password != "") user.Password = password.ToSHA256();

			JDSContext ctx = new JDSContext();

			ctx.Users.Update(user);
			ctx.SaveChanges();
			ctx.Dispose();

			return RedirectToAction(nameof(List));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult ParseCode(string code)
		{
			string? sentCode = HttpContext.Session.GetString(UserViewModel.SessionKeyUserResetPasswordCode);

			if (sentCode is null) return RedirectToAction(nameof(Login));

			if (sentCode != code)
			{
				Response.Cookies.Append(UserViewModel.CookieKeyError, "true");

				return RedirectToAction(nameof(ResetCode));
			}

			return RedirectToAction(nameof(ResetPassword));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult ParseNewPassword(string newPasswordOne, string newPasswordTwo)
		{
			int? userId = HttpContext.Session.GetInt32(UserViewModel.SessionKeyUserResetPasswordId);

			if (userId is null) return RedirectToAction(nameof(Login));

			if (newPasswordOne != newPasswordTwo)
			{
				Response.Cookies.Append(UserViewModel.CookieKeyError, "true");

				return RedirectToAction(nameof(ResetPassword));
			}

			HttpContext.Session.Remove(UserViewModel.SessionKeyUserResetPasswordId);
			HttpContext.Session.Remove(UserViewModel.SessionKeyUserResetPasswordCode);

			JDSContext ctx = new JDSContext();

			User? user = ctx.Users
				.Fetch()
				.FirstOrDefault(u => u.Id == userId);

			if (user is null)
			{
				ctx.Dispose();

				return RedirectToAction(nameof(Login));
			}

			user.Password = newPasswordOne.ToSHA256();

			ctx.Users.Update(user);
			ctx.SaveChanges();

			ctx.Dispose();

			Response.Cookies.Append(UserViewModel.CookieKeyPasswordResetSuccess, "true");

			return RedirectToAction(nameof(Login));
		}

		#endregion

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          PRIVATE METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		#region Fetch

		private static DBUser[] FetchUsers()
		{
			JDSContext ctx = new JDSContext();

			DBUser[] users = ctx.Users.Fetch();

			ctx.Dispose();

			return users;
		}

		private static DBUser? FetchUserById(int id)
		{
			JDSContext ctx = new JDSContext();

			User? user = ctx.Users.FetchById(id);

			ctx.Dispose();

			return user;
		}

		private static DBUser? FetchUserByUsername(string username)
		{
			JDSContext ctx = new JDSContext();

			DBUser? user = ctx.Users.FetchByUsername(username);

			ctx.Dispose();

			return user;
		}

		#endregion
	}
}