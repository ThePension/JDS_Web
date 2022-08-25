using System;
using System.Collections.Generic;

namespace JDSCommon.Database.Models
{
    public partial class ShopGallery
    {
        public int ClothId { get; set; }
        public int Image { get; set; }

        public virtual Cloth Cloth { get; set; } = null!;
        public virtual Image ImageNavigation { get; set; } = null!;
    }
}
