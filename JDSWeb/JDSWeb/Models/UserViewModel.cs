using JDSCommon.Database.DataContract;

namespace JDSWeb.Models
{
    public class UserViewModel
    {
        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                               FIELDS                              *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public static readonly string SessionKeyUserId = "_User.Id";
        public static readonly string SessionKeyUserName = "_User.Name";
        public static readonly string SessionKeyUserRole = "_User.Role";

        public static readonly string CookieKeyUsername = "c_User.Name";
        public static readonly string CookieKeyError = "c_User.Error";
        public static readonly string CookieKeyLoggedIn = "c_User.Login";
        public static readonly string CookieKeyLoggedOut = "c_User.Logout";

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                             PROPERTIES                            *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public string? Username { get; set; }
        public User[]? Users { get; set; }
        public bool Error { get; set; } = false;

    }
}
