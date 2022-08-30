using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JDSCommon.Services
{
    public static class ImageService
    {
        public static string? FromByteToBase64(byte[] image)
        {
            return image is null ? null : Convert.ToBase64String(image, 0, image.Length);
        }

        public static byte[] FromImageToBytes(string filename)
        {
             return File.ReadAllBytes(filename);
        }

        public static byte[] FromStreamToBytes(Stream file)
        {
            MemoryStream ms = new MemoryStream();

            file.CopyTo(ms);

            return ms.ToArray();
        }
    }
}
