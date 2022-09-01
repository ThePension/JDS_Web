﻿using JDSCommon.Api;
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

        public IActionResult ActualEvents()
        {
            EventViewModel vm = new EventViewModel
            {
                Events = FetchActualEvents(),
            };

            return View(vm);
        }

        public IActionResult PassedEvents()
        {
            EventViewModel vm = new EventViewModel
            {
                Events = FetchPassedEvents(),
            };

            return View(vm);
        }

        public IActionResult ShowEvent(int id)
        {
            Event? @event = FetchEventById(id);

            if (@event is null)
            {
                return RedirectToAction("ActualEvents", "Event");
            }
            else
            {
                EventViewModel vm = new EventViewModel
                {
                    Event = @event,
                };

                return View(vm);
            }
        }

        public IActionResult Create()
        {
            /*
            if ((ERole)(HttpContext.Session.GetInt32(UserViewModel.SessionKeyUserRole) ?? -1) < ERole.Manager)
            {
                return RedirectToAction("ActualEvents", "Event");
            }
            */

            return View();
        }

        public IActionResult Update(int id)
        {
            /*
            if ((ERole)(HttpContext.Session.GetInt32(UserViewModel.SessionKeyUserRole) ?? -1) < ERole.Manager)
            {
                return RedirectToAction("ActualEvents", "Event");
            }
            */

            Event? @event = FetchEventById(id);

            if (@event is null)
            {
                return RedirectToAction("ActualEvents", "Event");
            }
            else
            {
                EventViewModel vm = new EventViewModel
                {
                    Event = @event,
                };

                return View(vm);
            }
        }

        public IActionResult Delete(int id)
        {
            //if ((ERole)(HttpContext.Session.GetInt32(UserViewModel.SessionKeyUserRole) ?? -1) >= ERole.Manager)
            //{
            Event? eventToDelete = FetchEventById(id);

            if (eventToDelete is not null)
            {
                JDSContext ctx = new JDSContext();

                ctx.Events.Remove(eventToDelete);

                ctx.SaveChanges();
                ctx.Dispose();
            }
            //}

            return RedirectToAction("ActualEvents", "Event");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ParseEvent(string title, string description, DateTime date, ICollection<IFormFile> files)
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

            return RedirectToAction("ActualEvents", "Event");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ParseEventUpdate(int id, string title, string description, DateTime date, ICollection<IFormFile> files)
        {
            Event? eventToUpdate = FetchEventById(id);

            if (eventToUpdate is not null)
            {
                JDSContext ctx = new JDSContext();

                eventToUpdate.Title = title;
                eventToUpdate.Description = description;
                eventToUpdate.Date = date;

                var images = ctx.Events.Include(e => e.Images).FirstOrDefault(e => e.Id == id).Images;

                foreach (var image in ImagesFromFileNames(files))
                {
                    images.Add(image.ToModel());
                }
                ctx.SaveChanges();

                ctx.Events.Update(eventToUpdate);

                ctx.SaveChanges();
                ctx.Dispose();
            }

            return RedirectToAction("ActualEvents", "Event");
        }

        public IActionResult DeleteImage(int imageId, int eventId)
        {
            // NEED TO BE PROTECTED BY TEST ON USER'S RIGHT !

            Event? @event = FetchEventById(eventId);
            Image? imageToDelete = (@event is null) ? null : @event.Images.FirstOrDefault(i => i.Id == imageId);

            if (imageToDelete is not null && @event is not null)
            {
                JDSContext ctx = new JDSContext();

                ctx.Events.Include(e => e.Images).FirstOrDefault(e => e.Id == @event.Id).Images.Remove(imageToDelete.ToModel(ctx.Images));

                ctx.SaveChanges();
                ctx.Dispose();
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
