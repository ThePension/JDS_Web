using System;
using System.Collections.Generic;

namespace JDSCommon.Database.Models
{
    public partial class EventGallery
    {
        public int EventId { get; set; }
        public int Image { get; set; }

        public virtual Event Event { get; set; } = null!;
        public virtual Image ImageNavigation { get; set; } = null!;
    }
}
