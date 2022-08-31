using System;
using System.Collections.Generic;

namespace JDSCommon.Database.Models
{
    public partial class Cloth
    {
        public Cloth()
        {
            Images = new HashSet<Image>();
        }

        public int Id { get; set; }
        public int Type { get; set; }
        public int? Size { get; set; }

        public virtual ClothSize? SizeNavigation { get; set; }
        public virtual ClothType TypeNavigation { get; set; } = null!;

        public virtual ICollection<Image> Images { get; set; }
    }
}
