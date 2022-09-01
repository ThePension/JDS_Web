using JDSCommon.Database.DataContract;
using JDSCommon.Database.Models;
using JDSCommon.Services.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JDSCommon.Database
{
    public static class DatabaseExtensions
    {
        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           PUBLIC METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        #region Fetch Extensions

        public static DataContract.Cloth[] Fetch(this DbSet<Models.Cloth> cloths)
        {
            return cloths
                .Include(c => c.SizeNavigation)
                .Include(c => c.TypeNavigation)
                .Include(c => c.TypeNavigation.ColorNavigation)
                .Include(c => c.BookedNavigation)
                .Select(c => c.ToDataContract())
                .Copy();
        }

        public static DataContract.ClothType[] Fetch(this DbSet<Models.ClothType> clothTypes)
        {
            return clothTypes
                .Include(t => t.ColorNavigation)
                .Include(t => t.Images)
                .Select(t => t.ToDataContract())
                .Copy();
        }

        public static DataContract.ClothSize[] Fetch(this DbSet<Models.ClothSize> clothSizes)
        {
            return clothSizes
                .Select(s => s.ToDataContract())
                .Copy();
        }

        public static DataContract.Event[] Fetch(this DbSet<Models.Event> events)
        {
            return events
                .Include(e => e.Images)
                .Select(e => e.ToDataContract())
                .Copy();
        }

        public static DataContract.User[] Fetch(this DbSet<Models.User> users)
        {
            return users
                .Include(u => u.RoleNavigation)
                .Select(u => u.ToDataContract())
                .Copy();
        }

        #endregion

        #region Fetch by ID Extensions

        public static DataContract.Event? FetchById(this DbSet<Models.Event> events, int id)
        {
            Models.Event? @event = events
                .Include(e => e.Images)
                .FirstOrDefault(e => e.Id == id);

            return @event is null ? null : @event.ToDataContract();
        }

        public static DataContract.Image? FetchById(this DbSet<Models.Image> images, int id)
        {
            Models.Image? image = images
                .FirstOrDefault(i => i.Id == id);

            return image is null ? null : image.ToDataContract();
        }

        #endregion

        #region Add Extensions

        public static EntityEntry<Models.Cloth> Add(this DbSet<Models.Cloth> table, DataContract.Cloth entity)
        {
            return table.Add(entity.ToModel());
        }

        public static EntityEntry<Models.Event> Add(this DbSet<Models.Event> table, DataContract.Event entity)
        {
            return table.Add(entity.ToModel());
        }
        //public static EntityEntry<Models.Event> AddEvent(this JDSContext ctx, DataContract.Event entity)
        //{
        //    EntityEntry<Models.Event> eventAdded = ctx.Events.Add(entity.ToModel());
        //    ctx.SaveChanges();

        //    foreach (var eventImage in entity.Images)
        //    {
        //        EntityEntry<Models.Image> imageAdded = ctx.Images.Add(eventImage.ToModel());
        //        ctx.SaveChanges();

        //        EventGallery eventGallery = new EventGallery
        //        {
        //            EventId = eventAdded.Entity.Id,
        //            ImageId = imageAdded.Entity.Id,
        //        };

        //        ctx.Add(eventGallery);
        //        ctx.SaveChanges();
        //    }

        //    return eventAdded;
        //}

        public static EntityEntry<Models.User> Add(this DbSet<Models.User> table, DataContract.User entity)
        {
            return table.Add(entity.ToModel());
        }

        #endregion

        #region Remove Extensions

        public static EntityEntry<Models.Cloth>? Remove(this DbSet<Models.Cloth> table, DataContract.Cloth entity)
        {
            Models.Cloth? entityToRemove = entity.ToModel(table);

            return entityToRemove is null ? null : table.Remove(entityToRemove);
        }

        public static EntityEntry<Models.Event>? Remove(this DbSet<Models.Event> table, DataContract.Event entity)
        {
            Models.Event? entityToRemove = entity.ToModel(table);

            return entityToRemove is null ? null : table.Remove(entityToRemove);
        }

        public static EntityEntry<Models.User>? Remove(this DbSet<Models.User> table, DataContract.User entity)
        {
            Models.User? entityToRemove = entity.ToModel(table);

            return entityToRemove is null ? null : table.Remove(entityToRemove);
        }

        #endregion

        #region Update Extensions

        public static EntityEntry<Models.Event>? Update(this DbSet<Models.Event> table, DataContract.Event entity)
        {
            Models.Event? eventToUpdate = entity.ToModel(table);

            if (eventToUpdate is not null)
            {
                eventToUpdate.Date = entity.Date;
                eventToUpdate.Title = entity.Title;
                eventToUpdate.Description = entity.Description;
                eventToUpdate.Images = entity.Images.Select(i => i.ToModel()).ToArray();

                /*
                #region Update images in database

                JDSContext ctx = new JDSContext();

                // Get image for this event in database
                var eventDBImages = ctx.EventGalleries
                    .Where(i => i.EventId == entity.Id)
                    .ToArray();

                // Erase all relation eventId - Image
                foreach (var @event in eventDBImages)
                {
                    ctx.EventGalleries.Remove(@event);
                }

                // Add every relation eventId - Image
                foreach (var image in entity.Images)
                {
                    // Check if image exist
                    var imageDB = ctx.Images.FirstOrDefault(i => i.Id == image.Id);

                    if (imageDB is null)
                    {
                        ctx.Images.Add(image.ToModel());
                    }

                    ctx.EventGalleries.Add(new EventGallery
                    {
                        EventId = eventToUpdate.Id,
                        ImageId = image.Id,
                    });
                }

                ctx.SaveChanges();
                ctx.Dispose();

                #endregion
                */

                return table.Update(eventToUpdate);
            }
            else
            {
                return null;
            }
        }

        public static EntityEntry<Models.User>? Update(this DbSet<Models.User> table, DataContract.User entity)
        {
            Models.User? userToUpdate = entity.ToModel(table);

            if (userToUpdate is not null)
            {
                userToUpdate.Role = entity.Role.Id;
                userToUpdate.Username = entity.Username;
                userToUpdate.Email = entity.Email;
                userToUpdate.Password = entity.Password;
                userToUpdate.Newsletter = entity.Newsletter;

                return table.Update(userToUpdate);
            }
            else
            {
                return null;
            }
        }

        #endregion
    }
}
