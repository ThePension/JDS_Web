using JDSCommon.Api;
using JDSCommon.Database;
using JDSCommon.Database.DataContract;
using JDSCommon.Services;
using JDSCommon.Services.Extensions;
using JDSWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Configuration;
using System.Data;
using JDSContext = JDSCommon.Database.Models.JDSContext;
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

		public IActionResult Profil()
		{
			int? userId = HttpContext.Session.GetInt32(UserViewModel.SessionKeyUserId);

			JDSContext ctx = new JDSContext();

			User? user = ctx.Users
				.Fetch()
				.FirstOrDefault(u => u.Id == userId);

			ctx.Dispose();

			if (user is null) return RedirectToAction("Index", "Home");

			UserViewModel vm = new UserViewModel
			{
				User = user,
			};

			return View(vm);
		}

		public IActionResult List()
		{
			ERole role = (ERole)(HttpContext.Session.GetInt32(UserViewModel.SessionKeyUserRole) ?? -1);

			if (role < ERole.Manager)
			{
				return RedirectToAction(nameof(Profil));
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

			Request.Cookies.TryGetValue(UserViewModel.CookieKeyError, out string? error);
			Response.Cookies.Delete(UserViewModel.CookieKeyError);

			JDSContext ctx = new JDSContext();

			Role[] roles = ctx.Roles
				.Fetch()
				.Skip(1)
				.ToArray();

			ctx.Dispose();

			UserViewModel vm = new UserViewModel
			{
				Roles = roles,
				Error = bool.Parse(error ?? "false"),
			};

			return View(vm);
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

		[Route("User/Update/Informations")]
		public IActionResult UpdateInformations(int id)
		{
			ERole role = (ERole)(HttpContext.Session.GetInt32(UserViewModel.SessionKeyUserRole) ?? -1);
			int? userId = HttpContext.Session.GetInt32(UserViewModel.SessionKeyUserId);

			Request.Cookies.TryGetValue(UserViewModel.CookieKeyError, out string? error);
			Response.Cookies.Delete(UserViewModel.CookieKeyError);

			JDSContext ctx = new JDSContext();

			Role[] roles = ctx.Roles.Fetch();
			User? user = ctx.Users
				.Fetch()
				.FirstOrDefault(u => u.Id == id);

			ctx.Dispose();

			if (user is null) return RedirectToAction("Index", "Home");

			if (role < ERole.Manager && user.Id != id)
			{
				return RedirectToAction("Index", "Home");
			}

			UserViewModel vm = new UserViewModel
			{
				User = user,
				Roles = roles,
				IsManager = role >= ERole.Manager,
				Error = bool.Parse(error ?? "false"),
			};

			return user is null ? RedirectToAction(nameof(List)) : View(vm);
		}

		[Route("User/Update/Password")]
		public IActionResult UpdatePassword(int id)
		{
			ERole role = (ERole)(HttpContext.Session.GetInt32(UserViewModel.SessionKeyUserRole) ?? -1);
			int? userId = HttpContext.Session.GetInt32(UserViewModel.SessionKeyUserId);

			Request.Cookies.TryGetValue(UserViewModel.CookieKeyError, out string? error);
			Response.Cookies.Delete(UserViewModel.CookieKeyError);

			JDSContext ctx = new JDSContext();

			User? user = ctx.Users
				.Fetch()
				.FirstOrDefault(u => u.Id == id);

			ctx.Dispose();

			if (user is null) return RedirectToAction("Index", "Home");

			if (role < ERole.Manager && user.Id != id)
			{
				return RedirectToAction("Index", "Home");
			}

			UserViewModel vm = new UserViewModel
			{
				User = user,
				Error = bool.Parse(error ?? "false"),
			};

			return user is null ? RedirectToAction(nameof(List)) : View(vm);
		}

		public IActionResult Delete(int id)
		{
			ERole role = (ERole)(HttpContext.Session.GetInt32(UserViewModel.SessionKeyUserRole) ?? -1);
			int? userId = HttpContext.Session.GetInt32(UserViewModel.SessionKeyUserId);

			if (role < ERole.Manager)
			{
				return RedirectToAction("Index", "Home");
			}

			Request.Cookies.TryGetValue(UserViewModel.CookieKeyError, out string? error);
			Response.Cookies.Delete(UserViewModel.CookieKeyError);

			JDSContext ctx = new JDSContext();

			User? user = ctx.Users
				.Fetch()
				.FirstOrDefault(u => u.Id == id);

			if (user is null)
			{
				ctx.Dispose();

				return RedirectToAction("Index", "Home");
			}

			if (user.Id == userId)
			{
				ctx.Dispose();

				return RedirectToAction(nameof(List));
			}

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
			User? user = FetchUserByUsername(username);

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
		public IActionResult ParseCreate(string username, string email, int role, string newsletter)
		{
			JDSContext ctx = new JDSContext();

			bool usernameTaken = ctx.Users.Any(u => u.Username == username);

			if (usernameTaken)
			{
				ctx.Dispose();

				Response.Cookies.Append(UserViewModel.CookieKeyError, "true");

				return RedirectToAction(nameof(Create));
			}

			string password = SecurityService.GeneratePassword(8);

			User user = new User
			{
				Role = new Role { ERole = (ERole)role },
				Username = username,
				Email = email,
				Password = password.ToSHA256(),
				Newsletter = newsletter == "on",
			};

			ctx.Users.Add(user);
			ctx.SaveChanges();
			ctx.Dispose();

			new EMailMessage(email)
				.AddSubject("Bienvenue")
				.AddLine("Adieu !")
				.AddEmptyLine()
				.AddLine("Tout le comité de la Jeunesse de Savagnier te souhaite la bienvenue !")
				.AddLine("Voici tes identifiants de connexion pour le site internet, pense à changer ton mot de passe à ta première connexion.")
				.AddEmptyLine()
				.AddLine($"- <b>Nom d'utilisateur :</b> {username}")
				.AddLine($"- <b>Mot de passe :</b> {password}")
				.AddEmptyLine()
				.AddLine("À tout bientôt !")
				.Send();

			return RedirectToAction(nameof(List));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult ParseUpdateInformations(int id, string username, string email, int role, string newsletter)
		{
			JDSContext ctx = new JDSContext();

			User[] allUsers = ctx.Users.Fetch();

			User? user = allUsers.FirstOrDefault(c => c.Id == id);

			if (user is null)
			{
				ctx.Dispose();

				return RedirectToAction("Index", "Home");
			}

			bool usernameTaken = allUsers.Any(u => u.Username == username && u.Username != user.Username);

			if (usernameTaken)
			{
				ctx.Dispose();

				Response.Cookies.Append(UserViewModel.CookieKeyError, "true");

				return RedirectToAction(nameof(UpdateInformations));
			}

			user.Username = username;
			user.Email = email;
			user.Role = new Role { ERole = (ERole)role };
			user.Newsletter = newsletter == "on";

			HttpContext.Session.SetInt32(UserViewModel.SessionKeyUserRole, user.Role.Id);
			HttpContext.Session.SetString(UserViewModel.SessionKeyUserName, user.Username);

			ctx.Users.Update(user);
			ctx.SaveChanges();
			ctx.Dispose();

			return RedirectToAction(nameof(List));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult ParseUpdatePassword(int id, string passwordOne, string passwordTwo)
		{
			JDSContext ctx = new JDSContext();

			User? user = ctx.Users
				.Fetch()
				.FirstOrDefault(c => c.Id == id);

			if (user is null)
			{
				ctx.Dispose();

				return RedirectToAction("Index", "Home");
			}

			if (passwordOne != passwordTwo)
			{
				ctx.Dispose();

				Response.Cookies.Append(UserViewModel.CookieKeyError, "true");

				return RedirectToAction(nameof(UpdatePassword));
			}

			user.Password = passwordOne.ToSHA256();

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

		private static User[] FetchUsers()
		{
			JDSContext ctx = new JDSContext();

			User[] users = ctx.Users.Fetch();

			ctx.Dispose();

			return users;
		}

		private static User? FetchUserById(int id)
		{
			JDSContext ctx = new JDSContext();

			User? user = ctx.Users.FetchById(id);

			ctx.Dispose();

			return user;
		}

		private static User? FetchUserByUsername(string username)
		{
			JDSContext ctx = new JDSContext();

			User? user = ctx.Users.FetchByUsername(username);

			ctx.Dispose();

			return user;
		}

		#endregion
	}
}