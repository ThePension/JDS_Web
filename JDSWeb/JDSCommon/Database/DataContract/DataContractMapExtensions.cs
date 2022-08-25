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

        public static Models.Event? ToModel(this Event @event, DbSet<Models.Event> table)
            => table.FirstOrDefault(e => e.Id == @event.Id);

        public static Models.User? ToModel(this User user, DbSet<Models.User> table)
            => table.FirstOrDefault(e => e.Id == user.Id);

        public static ClothColor ToDataContract(this Models.ClothColor clothColor) => new ClothColor
        {
            Name = clothColor.Name,
            Hexa = clothColor.Hexa,
        };

        public static Models.ClothColor ToModel(this ClothColor clothColor) => new Models.ClothColor
        {

        };

        public static ClothSize ToDataContract(this Models.ClothSize clothSize) => new ClothSize
        {

        };

        public static Models.ClothSize ToModel(this ClothSize clothSize) => new Models.ClothSize
        {

        };

        public static ClothType ToDataContract(this Models.ClothType clothType) => new ClothType
        {

        };

        public static Models.ClothType ToModel(this ClothType clothType) => new Models.ClothType
        {

        };

        public static Event ToDataContract(this Models.Event @event) => new Event
        {

        };

        public static Models.Event ToModel(this Event @event) => new Models.Event
        {

        };

        public static Image ToDataContract(this Models.Image image) => new Image
        {

        };

        public static Models.Image ToModel(this Image image) => new Models.Image
        {

        };

        public static Role ToDataContract(this Models.Role role) => new Role
        {

        };

        public static Models.Role ToModel(this Role role) => new Models.Role
        {

        };

        public static User ToDataContract(this Models.User user) => new User
        {

        };

        public static Models.User ToModel(this User user) => new Models.User
        {

        };
    }
}
