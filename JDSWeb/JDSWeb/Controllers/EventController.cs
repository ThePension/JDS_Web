using JDSCommon.Api;
using JDSCommon.Database;
using JDSCommon.Database.DataContract;
using JDSCommon.Services;
using JDSWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JDSContext = JDSCommon.Database.Models.JDSContext;

namespace JDSWeb.Controllers
{
    public class EventController : Controller
    {
        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           PUBLIC METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

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
            else
            {
                EventViewModel vm = new EventViewModel
                {
                    Events = new Event[] { @event },
                };

                return View(vm);
            }
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
            else
            {
                EventViewModel vm = new EventViewModel
                {
                    Events = new Event[] { @event },
                };

                return View(vm);
            }
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
        public ActionResult ParseUpdate(int id, string title, string description, DateTime date, ICollection<IFormFile> files)
        {
            Event? eventToUpdate = FetchEventById(id);

            if (eventToUpdate is not null)
            {
                JDSContext ctx = new JDSContext();

                eventToUpdate.Title = title;
                eventToUpdate.Description = description;
                eventToUpdate.Date = date;

                //var images = ctx.Events.Include(e => e.Images).FirstOrDefault(e => e.Id == id).Images;

                ctx.AddImagesInEvent(id, ImagesFromFileNames(files));

                //foreach (var image in ImagesFromFileNames(files))
                //{
                //    images.Add(image.ToModel());
                //}
                //ctx.SaveChanges();

                ctx.Events.Update(eventToUpdate);

                ctx.SaveChanges();
                ctx.Dispose();
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

                //event? @event = fetcheventbyid(eventid);
                //image? imagetodelete = (@event is null) ? null : @event.images.firstordefault(i => i.id == imageid);

                //if (imagetodelete is not null && @event is not null)
                //{
                //    jdscontext ctx = new jdscontext();

                //    ctx.events.include(e => e.images).firstordefault(e => e.id == @event.id).images.remove(imagetodelete.tomodel(ctx.images));

                //    ctx.savechanges();
                //    ctx.dispose();
                //}
            }

            return RedirectToAction("Update", "Event", new { id = eventId });
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                          PRIVATE METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        private static Image[] ImagesFromFileNames(ICollection<IFormFile> files)
        {
            Image[] images = new Image[files.Count];

            int count = 0;

            foreach (var image in files)
            {
                images[count++] = new Image
                {
                    Alt = image.FileName,
                    URL = ImageKitAPI.UploadImage(ImageService.FromStreamToBytes(image.OpenReadStream()), image.FileName),
                };
            }

            return images;
        }

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

        private static Image? FetchImageById(int id)
        {
            JDSContext ctx = new JDSContext();

            Image? image = ctx.Images.FetchById(id);

            ctx.Dispose();

            return image;
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

    }
}
