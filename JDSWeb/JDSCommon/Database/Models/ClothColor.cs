using System;
using System.Collections.Generic;

namespace JDSCommon.Database.Models
{
    public partial class ClothColor
    {
        public ClothColor()
        {
            ClothTypes = new HashSet<ClothType>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Hexa { get; set; } = null!;

        public virtual ICollection<ClothType> ClothTypes { get; set; }
    }
}
