using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace JDSCommon.Services.Extensions
{
    public static class StringExtensions
    {
        public static string ToSHA256(this string password)
        {
            if (password == null) return "";

            return string.Join("", SHA256.Create().ComputeHash(Encoding.ASCII.GetBytes(password)).Select(s => s.ToString("x2")));
        }
    }
}
