using System;
using System.Collections.Generic;

namespace JDSCommon.Database.Models
{
    public partial class ClothSize
    {
        public ClothSize()
        {
            Cloths = new HashSet<Cloth>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Shortcut { get; set; } = null!;

        public virtual ICollection<Cloth> Cloths { get; set; }
    }
}
