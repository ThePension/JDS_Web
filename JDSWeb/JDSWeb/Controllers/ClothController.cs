using JDSCommon.Database;
using JDSCommon.Database.DataContract;
using JDSWeb.Models;
using Microsoft.AspNetCore.Mvc;
using JDSContext = JDSCommon.Database.Models.JDSContext;

namespace JDSWeb.Controllers
{
    public class ClothController : Controller
    {
        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           PUBLIC METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public ActionResult Index()
        {
            return ShowAllClothes();
        }
        // GET: ClothController
        public ActionResult ShowAllClothes()
        {
            ClothViewModel vm = new ClothViewModel
            {
                Clothes = FetchClothes(),
            };


            return View("ShowAllClothes", vm);
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          PRIVATE METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        private static Cloth[] FetchClothes()
        {
            JDSContext ctx = new JDSContext();

            Cloth[] clothes = ctx.Cloths.Fetch();

            ctx.Dispose();

            return clothes;
        }


        // GET: ClothController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ClothController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ClothController/Create
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

        // GET: ClothController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ClothController/Edit/5
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

        // GET: ClothController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ClothController/Delete/5
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
