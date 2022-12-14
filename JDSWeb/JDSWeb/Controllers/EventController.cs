using JDSCommon.Api;
using JDSCommon.Database;
using JDSCommon.Database.DataContract;
using JDSCommon.Services;
using JDSWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using JDSContext = JDSCommon.Database.Models.JDSContext;

namespace JDSWeb.Controllers
{
    public class EventController : Controller
    {
        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           PUBLIC METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        #region IActionResult Methods

        public IActionResult Actual()
        {
            EventViewModel vm = new EventViewModel
            {
                Events = FetchActualEvents(),
            };

            return (vm.Events is null) ? RedirectToAction("Index", "Home") : View(vm);
        }

        public IActionResult Passed()
        {
            EventViewModel vm = new EventViewModel
            {
                Events = FetchPassedEvents(),
            };

            return (vm.Events is null) ? RedirectToAction("Index", "Home") : View(vm);
        }

        public IActionResult Show(int id)
        {
            Event? @event = FetchEventById(id);

            if (@event is null)
            {
                return RedirectToAction("Actual", "Event");
            }

            EventViewModel vm = new EventViewModel
            {
                Events = new Event[] { @event },
            };

            return View(vm);
        }

        public IActionResult Create()
        {
            if ((ERole)(HttpContext.Session.GetInt32(UserViewModel.SessionKeyUserRole) ?? -1) < ERole.Manager)
            {
                return RedirectToAction("Actual", "Event");
            }

            return View();
        }

        public IActionResult Update(int id)
        {
            if ((ERole)(HttpContext.Session.GetInt32(UserViewModel.SessionKeyUserRole) ?? -1) < ERole.Manager)
            {
                return RedirectToAction("Actual", "Event");
            }

            Event? @event = FetchEventById(id);

            if (@event is null)
            {
                return RedirectToAction("Actual", "Event");
            }

            EventViewModel vm = new EventViewModel
            {
                Events = new Event[] { @event },
            };

            return View(vm);
        }

        public IActionResult Delete(int id)
        {
            if ((ERole)(HttpContext.Session.GetInt32(UserViewModel.SessionKeyUserRole) ?? -1) >= ERole.Manager)
            {
                Event? eventToDelete = FetchEventById(id);

                if (eventToDelete is not null)
                {
                    JDSContext ctx = new JDSContext();

                    ctx.Events.Remove(eventToDelete);

                    ctx.SaveChanges();
                    ctx.Dispose();
                }
            }

            return RedirectToAction("Actual", "Event");
        }

        public IActionResult DeleteImage(int imageId, int eventId)
        {
            if ((ERole)(HttpContext.Session.GetInt32(UserViewModel.SessionKeyUserRole) ?? -1) >= ERole.Manager)
            {
                JDSContext ctx = new JDSContext();

                ctx.RemoveImageInEvent(eventId, imageId);

                ctx.SaveChanges();
                ctx.Dispose();
            }

            return RedirectToAction("Update", "Event", new { id = eventId });
        }

        #endregion

        #region POST Actions

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ParseCreate(string title, string description, DateTime date, ICollection<IFormFile> files)
        {
            JDSContext ctx = new JDSContext();

            ctx.Events.Add(new Event
            {
                Title = title,
                Description = description,
                Date = date,
                Images = ImagesFromFileNames(files)
            });

            ctx.SaveChanges();
            ctx.Dispose();

            return RedirectToAction("Actual", "Event");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SearchResults(string pattern)
        {
            Regex rx = new Regex(pattern, RegexOptions.IgnoreCase);
            JDSContext ctx = new JDSContext();

            Event[] events = ctx.Events
                .Fetch()
                .Where(e => rx.IsMatch(e.Title))
                .OrderByDescending(e => e.Date)
                .ToArray();

            ctx.Dispose();

            EventViewModel vm = new EventViewModel
            {
                Events = events,
                SearchPattern = pattern,
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ParseUpdate(int id, string title, string description, DateTime date, ICollection<IFormFile> files)
        {
            Event? eventToUpdate = FetchEventById(id);

            if (eventToUpdate is not null)
            {
                eventToUpdate.Title = title;
                eventToUpdate.Description = description;
                eventToUpdate.Date = date;

                JDSContext ctx = new JDSContext();

                ctx.AddImagesInEvent(id, ImagesFromFileNames(files));

                ctx.Events.Update(eventToUpdate);
                ctx.SaveChanges();
                ctx.Dispose();
            }

            return RedirectToAction("Actual", "Event");
        }

        #endregion

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          PRIVATE METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        #region Helper

        private static Image[] ImagesFromFileNames(ICollection<IFormFile> files)
        {
            Image[] images = new Image[files.Count];

            int count = 0;

            foreach (IFormFile image in files)
            {
                images[count++] = new Image
                {
                    Alt = image.FileName,
                    URL = ImageKitAPI.UploadImage(ImageService.FromStreamToBytes(image.OpenReadStream()), image.FileName),
                };
            }

            return images;
        }

        #endregion

        #region Fetch methods

        private static Event[] FetchEvents()
        {
            JDSContext ctx = new JDSContext();

            Event[] events = ctx.Events.Fetch();

            ctx.Dispose();

            return events;
        }

        private static Event? FetchEventById(int id)
        {
            JDSContext ctx = new JDSContext();

            Event? @event = ctx.Events.FetchById(id);

            ctx.Dispose();

            return @event;
        }

        private static Event[] FetchActualEvents()
        {
            Event[] actualEvents = FetchEvents().Where(e => DateTime.Compare(e.Date, DateTime.Now) >= 0).ToArray();

            return actualEvents;
        }

        private static Event[] FetchPassedEvents()
        {
            Event[] passedEvents = FetchEvents().Where(e => DateTime.Compare(e.Date, DateTime.Now) < 0).ToArray();

            return passedEvents;
        }

        #endregion
    }
}
