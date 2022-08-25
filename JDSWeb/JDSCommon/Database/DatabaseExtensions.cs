using JDSCommon.Database.DataContract;
using JDSCommon.Database.Models;
using JDSCommon.Services;
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
        #region Fetch Extensions

        public static Cloth[] Fetch(this DbSet<Models.Cloth> cloths)
        {
            return cloths
                .Select(c => c.ToDataContract())
                .Copy();
        }

        public static Event[] Fetch(this DbSet<Models.Event> events)
        {
            return events
                .Select(e => e.ToDataContract())
                .Copy();
        }

        public static User[] Fetch(this DbSet<Models.User> users)
        {
            return users
                .Select(u => u.ToDataContract())
                .Copy();
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

        public static EntityEntry<Models.Cloth>? Update(this DbSet<Models.Cloth> table, DataContract.Cloth entity)
        {
            Models.Cloth? clothToUpdate = entity.ToModel(table);

            if(clothToUpdate is not null)
            {
                clothToUpdate.Type = entity.Type.Id;
                clothToUpdate.Size = entity.Size.Id;
                clothToUpdate.Color = entity.Color.Id;
                clothToUpdate.Name = entity.Name;
                clothToUpdate.Description = entity.Description;

                // TODO : Update images of cloth

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

                // TODO : Update images of event

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

                // TODO : Update images of cloth

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
