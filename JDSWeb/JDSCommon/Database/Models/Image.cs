using System;
using System.Collections.Generic;

namespace JDSCommon.Database.Models
{
    public partial class Image
    {
        public Image()
        {
            ClothTypes = new HashSet<ClothType>();
            Events = new HashSet<Event>();
        }

        public int Id { get; set; }
        public string Url { get; set; } = null!;
        public string Alt { get; set; } = null!;

        public virtual ICollection<ClothType> ClothTypes { get; set; }
        public virtual ICollection<Event> Events { get; set; }
    }
}
