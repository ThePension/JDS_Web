using System;
using System.Collections.Generic;

namespace JDSCommon.Database.Models
{
    public partial class Event
    {
        public Event()
        {
            Images = new HashSet<Image>();
        }

        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public bool Public { get; set; }

        public virtual ICollection<Image> Images { get; set; }
    }
}
