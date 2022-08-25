using JDSCommon.Database.DataContract;
using JDSCommon.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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

        public static ClothColor[] Fetch(this DbSet<Models.ClothColor> clothColors)
        {
            return clothColors
                .Select(c => c.ToDataContract())
                .Copy();
        }

        #endregion
    }
}
