using JDSCommon.Database.DataContract;
using JDSCommon.Database.Models;
using JDSCommon.Services.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

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
                .Include(c => c.TypeNavigation.Images)
                .Include(c => c.BookedNavigation)
                .Include(c => c.BookedNavigation!.RoleNavigation)
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

        public static DataContract.User? FetchByUsername(this DbSet<Models.User> users, string username)
        {
            Models.User? user = users
                .Include(u => u.RoleNavigation)
                .FirstOrDefault(u => u.Username == username);

            return user is null ? null : user.ToDataContract();
        }

        public static DataContract.User? FetchById(this DbSet<Models.User> users, int id)
        {
            Models.User? user = users
                .Include(u => u.RoleNavigation)
                .FirstOrDefault(u => u.Id == id);

            return user is null ? null : user.ToDataContract();
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

        public static EntityEntry<Models.User> Add(this DbSet<Models.User> table, DataContract.User entity)
        {
            return table.Add(entity.ToModel());
        }

        public static void AddImagesInEvent(this JDSContext ctx, int eventId, DataContract.Image[] images)
        {
            Models.Event? eventModel = ctx.Events.Include(e => e.Images).FirstOrDefault(e => e.Id == eventId);

            if (eventModel is not null)
            {
                foreach (DataContract.Image image in images)
                {
                    Models.Image? imageModel = ctx.Images.FirstOrDefault(i => i.Url == image.URL);
                    eventModel.Images.Add(imageModel is not null ? imageModel : image.ToModel());
                }
            }
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

        public static void RemoveImageInEvent(this JDSContext ctx, int eventId, int imageId)
        {
            Models.Event? eventModel = ctx.Events.Include(e => e.Images).FirstOrDefault(e => e.Id == eventId);

            if (eventModel is not null)
            {
                Models.Image? imageModel = eventModel.Images.FirstOrDefault(i => i.Id == imageId);

                if (imageModel is not null) eventModel.Images.Remove(imageModel);
            }
        }

        #endregion

        #region Update Extensions

        public static EntityEntry<Models.Cloth>? Update(this DbSet<Models.Cloth> table, DataContract.Cloth entity)
        {
            Models.Cloth? clothToUpdate = entity.ToModel(table);

            if (clothToUpdate is not null)
            {
                clothToUpdate.Booked = entity.Booked?.Id;

                return table.Update(clothToUpdate);
            }
            else
            {
                return null;
            }
        }

        public static EntityEntry<Models.Event>? Update(this DbSet<Models.Event> table, DataContract.Event entity)
        {
            Models.Event? eventToUpdate = entity.ToModel(table);

            if (eventToUpdate is not null)
            {
                eventToUpdate.Date = entity.Date;
                eventToUpdate.Title = entity.Title;
                eventToUpdate.Description = entity.Description;

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
