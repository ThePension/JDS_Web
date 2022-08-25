using System;
using System.Collections.Generic;

namespace JDSCommon.Database.Models
{
    public partial class User
    {
        public int Id { get; set; }
        public int Role { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public bool Newsletter { get; set; }

        public virtual Role RoleNavigation { get; set; } = null!;
    }
}
