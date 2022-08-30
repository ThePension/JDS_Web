using System;
using System.Collections.Generic;

namespace JDSCommon.Database.Models
{
    public partial class Image
    {
        public Image()
        {
            Cloths = new HashSet<Cloth>();
            Events = new HashSet<Event>();
        }

        public int Id { get; set; }
        public byte[] Picture { get; set; } = null!;
        public string Alt { get; set; } = null!;

        public virtual ICollection<Cloth> Cloths { get; set; }
        public virtual ICollection<Event> Events { get; set; }
    }
}
