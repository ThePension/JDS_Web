using JDSCommon.Database;
using JDSCommon.Database.DataContract;
using JDSWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JDSContext = JDSCommon.Database.Models.JDSContext;

namespace JDSWeb.Controllers
{
    public class EventController : Controller
    {
        public IActionResult ActualEvents()
        {
            ActualEventViewModel vm = new ActualEventViewModel
            {
                Events = FetchEvents(),
            };

            return View(vm);
        }

        public IActionResult PassedEvents()
        {
            return View();
        }

        private static Event[] FetchEvents()
        {
            JDSContext ctx = new JDSContext();

            Event[] events = ctx.Events.Fetch();

            ctx.Dispose();

            return events;
        }
    }
}
