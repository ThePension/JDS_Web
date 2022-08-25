using System;
using System.Collections.Generic;

namespace JDSCommon.Database.Models
{
    public partial class ClothType
    {
        public ClothType()
        {
            Cloths = new HashSet<Cloth>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }

        public virtual ICollection<Cloth> Cloths { get; set; }
    }
}
