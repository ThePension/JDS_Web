using JDSCommon.Database;
using JDSCommon.Database.DataContract;
using JDSCommon.Services;
using JDSCommon.Services.Extensions;
using JDSWeb.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using DBUser = JDSCommon.Database.DataContract.User;
using JDSContext = JDSCommon.Database.Models.JDSContext;

namespace JDSWeb.Controllers
{
    public class UserController : Controller
    {
        public ActionResult Login()
        {
            if (HttpContext.Session.GetInt32(UserViewModel.SessionKeyUserId) is not null)
            {
                return RedirectToAction("Index", "Home");
            }

            Request.Cookies.TryGetValue(UserViewModel.CookieKeyUsername, out string? username);
            Request.Cookies.TryGetValue(UserViewModel.CookieKeyError, out string? error);

            UserViewModel vm = new UserViewModel
            {
                Username = username,
                Error = bool.Parse(error ?? "false"),
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ParseLogin(string username, string password)
        {
            JDSContext ctx = new JDSContext();

            DBUser? user = ctx.Users
                .Fetch()
                .FirstOrDefault(u => u.Username == username);

            ctx.Dispose();

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

        public ActionResult Logout()
        {
            HttpContext.Session.Remove(UserViewModel.SessionKeyUserId);
            HttpContext.Session.Remove(UserViewModel.SessionKeyUserName);
            HttpContext.Session.Remove(UserViewModel.SessionKeyUserRole);

            Response.Cookies.Append(UserViewModel.CookieKeyLoggedOut, "true");

            return RedirectToAction("Index", "Home");
        }

        // GET: UserController
        public ActionResult Index()
        {
            return View();
        }

        // GET: UserController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: UserController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UserController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: UserController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: UserController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: UserController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: UserController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
