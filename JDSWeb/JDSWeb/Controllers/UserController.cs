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

        #region ActionResult Methods

        public ActionResult Login()
        {
            if (HttpContext.Session.GetInt32(UserViewModel.SessionKeyUserId) is not null)
            {
                return RedirectToAction("Index", "Home");
            }

            Request.Cookies.TryGetValue(UserViewModel.CookieKeyUsername, out string? username);
            Request.Cookies.TryGetValue(UserViewModel.CookieKeyError, out string? error);
            Request.Cookies.TryGetValue(UserViewModel.CookieKeyPasswordResetSuccess, out string? passwordResetSuccess);

            UserViewModel vm = new UserViewModel
            {
                Username = username,
                Error = bool.Parse(error ?? "false"),
                PasswordResetSuccess = bool.Parse(passwordResetSuccess ?? "false"),
            };

            Response.Cookies.Delete(UserViewModel.CookieKeyPasswordResetSuccess);

            return View(vm);
        }

        public ActionResult Logout()
        {
            HttpContext.Session.Remove(UserViewModel.SessionKeyUserId);
            HttpContext.Session.Remove(UserViewModel.SessionKeyUserName);
            HttpContext.Session.Remove(UserViewModel.SessionKeyUserRole);

            Response.Cookies.Append(UserViewModel.CookieKeyLoggedOut, "true");

            return RedirectToAction("Index", "Home");
        }

        // GET: UserController
        public ActionResult List()
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

        public ActionResult Create()
        {
            if ((ERole)(HttpContext.Session.GetInt32(UserViewModel.SessionKeyUserRole) ?? -1) < ERole.Manager)
            {
                return RedirectToAction("Index", "Home");
            }

            string rndPw = SecurityService.GeneratePassword(12);

            return View("Create", rndPw);
        }

        public ActionResult Update(int id)
        {
            if ((ERole)(HttpContext.Session.GetInt32(UserViewModel.SessionKeyUserRole) ?? -1) < ERole.Manager && (HttpContext.Session.GetInt32(UserViewModel.SessionKeyUserId) ?? -1) != id)
            {
                return RedirectToAction("Index", "Home");
            }

            User? user = FetchUserById(id);

            return (user is null) ? RedirectToAction("List", "User") : View(user);
        }

        public ActionResult Delete(int id)
        {
            if ((ERole)(HttpContext.Session.GetInt32(UserViewModel.SessionKeyUserRole) ?? -1) < ERole.Manager)
            {
                return RedirectToAction("Index", "Home");
            }

            User? user = FetchUserById(id);

            if (user is null)
            {
                return RedirectToAction("List", "User");
            }

            JDSContext ctx = new JDSContext();

            ctx.Users.Remove(user);
            ctx.SaveChanges();
            ctx.Dispose();

            return RedirectToAction("List", "User");
        }

        public ActionResult ForgottonPassword()
        {
            return View();
        }

        public ActionResult ResetPassword()
        {
            return View();
        }

        public ActionResult ForgottenPasswordEmailInput()
        {
            Request.Cookies.TryGetValue(UserViewModel.CookieKeyError, out string? error);

            UserViewModel vm = new UserViewModel
            {
                Error = bool.Parse(error ?? "false"),
            };

            Response.Cookies.Delete(UserViewModel.CookieKeyError);

            return View(vm);
        }
        public ActionResult ForgottenPasswordCodeInput()
        {
            Request.Cookies.TryGetValue(UserViewModel.CookieKeyError, out string? error);

            UserViewModel vm = new UserViewModel
            {
                Error = bool.Parse(error ?? "false"),
            };

            Response.Cookies.Delete(UserViewModel.CookieKeyError);

            return View(vm);
        }

        #endregion

        #region POST Actions

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ParseLogin(string username, string password)
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
        public ActionResult ParseCreate(string username, string email, string password, int role, string newsletter)
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

            return RedirectToAction("List", "User");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ParseUpdate(int id, string username, string email, string password, int role, string newsletter)
        {
            User? user = FetchUserById(id);

            if (user is null)
            {
                return RedirectToAction("List", "User");
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

            return RedirectToAction("List", "User");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgottenPasswordSendCode(string email)
        {
            JDSContext ctx = new JDSContext();

            User? user = FetchUserByEmail(email);

            ctx.Dispose();

            if (user is null)
            {
                Response.Cookies.Append(UserViewModel.CookieKeyError, "true");

                return RedirectToAction("ForgottenPasswordEmailInput", "User");
            }

            // Generate random code
            string code = SecurityService.GeneratePassword(6);

            EMailMessage mail = new EMailMessage(email)
                .AddSubject("JDS - Réinitialisation du mot-de-passe")
                .AddLine($"Bonjour {user.Username}, voici votre code de réinitialisation : {code}.");

            // Provide random generated code to the user (Email)
            mail.Send();

            // Keep track of the user id and reset code
            HttpContext.Session.SetString(UserViewModel.SessionKeyUserResetPasswordCode, code);
            HttpContext.Session.SetInt32(UserViewModel.SessionKeyUserId, user.Id);

            return RedirectToAction("ForgottenPasswordCodeInput", "User");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ValidateResetPasswordCode(string code)
        {
            string? sentCode = HttpContext.Session.GetString(UserViewModel.SessionKeyUserResetPasswordCode);

            if (sentCode is null) return RedirectToAction("Login", "User");

            if (sentCode != code)
            {
                Response.Cookies.Append(UserViewModel.CookieKeyError, "true");

                return RedirectToAction("ForgottenPasswordCodeInput", "User");
            }

            HttpContext.Session.Remove(UserViewModel.SessionKeyUserResetPasswordCode);

            return RedirectToAction("ResetPassword", "User");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ParseNewPassword(string newPassword)
        {
            JDSContext ctx = new JDSContext();

            int? userId = HttpContext.Session.GetInt32(UserViewModel.SessionKeyUserId);

            if (userId is null) return RedirectToAction("Login", "User");

            User? user = FetchUserById(userId ?? -1);

            if (user is null)
            {
                ctx.Dispose();

                return RedirectToAction("Login", "User");
            }

            user.Password = newPassword.ToSHA256();

            ctx.Users.Update(user);

            ctx.SaveChanges();

            ctx.Dispose();

            HttpContext.Session.Remove(UserViewModel.SessionKeyUserId);

            Response.Cookies.Append(UserViewModel.CookieKeyPasswordResetSuccess, "true");

            return RedirectToAction("Login", "User");
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

        private static DBUser? FetchUserByEmail(string email)
        {
            JDSContext ctx = new JDSContext();

            DBUser? user = ctx.Users.FetchByEmail(email);

            ctx.Dispose();

            return user;
        }

        #endregion
    }
}