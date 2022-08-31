using System;
using System.Collections.Generic;

namespace JDSCommon.Database.Models
{
    public partial class Cloth
    {
        public int Id { get; set; }
        public int Type { get; set; }
        public int? Size { get; set; }

        public virtual ClothSize? SizeNavigation { get; set; }
        public virtual ClothType TypeNavigation { get; set; } = null!;
    }
}
