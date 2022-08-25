using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JDSCommon.Database.DataContract
{
    internal class Cloth
    {
        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                             PROPERTIES                            *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public int Id { get; }
        public ClothType Type { get; set; }
        public ClothSize Size { get; set; }
        public ClothColor Color { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }    

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
        |*                           PUBLIC METHODS                          *|
        \* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
    }
}
