using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JDSCommon.Services.Extensions
{
    public static class IEnumerableExtensions
    {
        public static T[] Copy<T>(this IEnumerable<T> source)
        {
            int size = source.Count();

            T[] copy = new T[size];

            Array.Copy(source.ToArray(), copy, size);

            return copy;
        }
    }
}
