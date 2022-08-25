using System;
using System.Collections.Generic;

namespace JDSCommon.Database.Models
{
    public partial class ShopGallery
    {
        public int ClothId { get; set; }
        public int ImageId { get; set; }

        public virtual Cloth Cloth { get; set; } = null!;
        public virtual Image Image { get; set; } = null!;
    }
}
