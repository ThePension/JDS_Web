using System;
using System.Collections.Generic;

namespace JDSCommon.Database.Models
{
    public partial class Image
    {
        public int Id { get; set; }
        public byte[] Picture { get; set; } = null!;
        public string Alt { get; set; } = null!;
    }
}
