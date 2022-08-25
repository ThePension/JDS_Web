using System;
using System.Collections.Generic;

namespace JDSCommon.Database.Models
{
    public partial class Cloth
    {
        public int Id { get; set; }
        public int Type { get; set; }
        public int Size { get; set; }
        public int Color { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;

        public virtual ClothColor ColorNavigation { get; set; } = null!;
        public virtual ClothSize SizeNavigation { get; set; } = null!;
        public virtual ClothType TypeNavigation { get; set; } = null!;
    }
}
