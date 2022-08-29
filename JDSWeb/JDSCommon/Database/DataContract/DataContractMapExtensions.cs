using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace JDSCommon.Database.DataContract
{
    public static class DataContractMapExtensions
    {
        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           PUBLIC METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        #region Cloth map extension

        public static Cloth ToDataContract(this Models.Cloth cloth) => new Cloth
        {
            Id = cloth.Id,
            Type = cloth.TypeNavigation.ToDataContract(),
            Size = cloth.SizeNavigation.ToDataContract(),
            Color = cloth.ColorNavigation.ToDataContract(),
            Name = cloth.Name,
            Description = cloth.Description,
        };

        public static Models.Cloth ToModel(this Cloth cloth) => new Models.Cloth
        {
            Type = cloth.Type.Id,
            Size = cloth.Size.Id,
            Color = cloth.Color.Id,
            Name = cloth.Name,
            Description = cloth.Description,
        };

        public static Models.Cloth? ToModel(this Cloth cloth, DbSet<Models.Cloth> table)
            => table.FirstOrDefault(c => c.Id == cloth.Id);

        #endregion

        #region ClothColor map extension

        public static ClothColor ToDataContract(this Models.ClothColor clothColor) => new ClothColor
        {
            Id = clothColor.Id,
            Name = clothColor.Name,
            Hexa = clothColor.Hexa,
        };

        public static Models.ClothColor ToModel(this ClothColor clothColor) => new Models.ClothColor
        {
            Name = clothColor.Name,
            Hexa = clothColor.Hexa,
        };

        #endregion

        #region ClothSize map extension

        public static ClothSize ToDataContract(this Models.ClothSize clothSize) => new ClothSize
        {
            Name = clothSize.Name,
            Shortcut = clothSize.Shortcut,
            ESize = (ESize)clothSize.Id,
        };

        public static Models.ClothSize ToModel(this ClothSize clothSize) => new Models.ClothSize
        {
            Name = clothSize.Name,
            Shortcut = clothSize.Shortcut,
        };

        #endregion

        #region ClothType map extension

        public static ClothType ToDataContract(this Models.ClothType clothType) => new ClothType
        {
            Name = clothType.Name,
            Price = clothType.Price,
            EType = (EType)clothType.Id,
        };

        public static Models.ClothType ToModel(this ClothType clothType) => new Models.ClothType
        {
            Name = clothType.Name,
            Price = clothType.Price,
        };

        #endregion

        #region Event map extension

        public static Event ToDataContract(this Models.Event @event)
        {
            Event dataContractEvent = new Event
            {
                Id = @event.Id,
                Date = @event.Date,
                Title = @event.Title,
                Description = @event.Description,
            };

            dataContractEvent.LoadImages();

            return dataContractEvent;
        }

        public static Models.Event ToModel(this Event @event) => new Models.Event
        {
            Date = @event.Date,
            Title= @event.Title,
            Description = @event.Description,
        };

        public static Models.Event? ToModel(this Event @event, DbSet<Models.Event> table)
            => table.FirstOrDefault(e => e.Id == @event.Id);

        #endregion

        #region Image map extension

        public static Image ToDataContract(this Models.Image image) => new Image
        {
            Id = image.Id,
            Picture = image.Picture,
            Alt = image.Alt,
        };

        public static Models.Image ToModel(this Image image) => new Models.Image
        {
            Picture = image.Picture,
            Alt = image.Alt,
        };

        #endregion

        #region Role map extension

        public static Role ToDataContract(this Models.Role role) => new Role
        {
            ERole = (ERole)role.Id,
            Name = role.Name,
        };

        public static Models.Role ToModel(this Role role) => new Models.Role
        {
            Name = role.Name,
        };

        #endregion

        #region User map extension

        public static User ToDataContract(this Models.User user) => new User
        {
            Id = user.Id,
            Role = user.RoleNavigation.ToDataContract(),
            Username = user.Username,
            Email = user.Email,
            Password = user.Password,
            Newsletter = user.Newsletter,
        };

        public static Models.User ToModel(this User user) => new Models.User
        {
            Role = user.Role.Id,
            Username = user.Username,
            Email = user.Email,
            Password = user.Password,
            Newsletter = user.Newsletter,
        };

        public static Models.User? ToModel(this User user, DbSet<Models.User> table)
            => table.FirstOrDefault(e => e.Id == user.Id);

        #endregion
    }
}
