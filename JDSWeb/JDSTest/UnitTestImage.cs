using JDSCommon.Services;
using System.Runtime.Serialization;
using JDSCommon.Database.Models;
using JDSCommon.Database.DataContract;

namespace JDSTest
{
    [TestClass]
    public class UnitTestImage
    {
        /* ASSUMPTION:
         * 
         * The tests needs to be executed with the image "JDS_insta_1.jpg" in the folder "JDSWeb\JDSTest\bin\Debug\net6.0\JDS_insta_1.jpg"
         */

        [TestMethod]
        public void TestImage()
        {
            byte[] byteImage = ImageService.FromImageToBytes(@"JDS_insta_1.jpg");

            string? base64Image = ImageService.FromByteToBase64(byteImage);

            Assert.IsNotNull(base64Image);
        }

        [TestMethod]
        public void TestAddImage()
        {
            byte[] byteImage = ImageService.FromImageToBytes(@"JDS_insta_1.jpg");

            JDSCommon.Database.Models.Image image = new JDSCommon.Database.Models.Image
            {
                Alt = "test",
                Picture = byteImage,
            };

            JDSContext ctx = new JDSContext();

            // Add to database
            ctx.Images.Add(image);
            ctx.SaveChanges();

            // Select from database
            var imageFromDatabase = ctx.Images.FirstOrDefault(i => i.Id == image.Id);

            // Test
            Assert.IsNotNull(imageFromDatabase);
            Assert.IsTrue(imageFromDatabase.Alt == "test");
            Assert.IsTrue(imageFromDatabase.Picture == byteImage);

            // Remove from database
            ctx.Images.Remove(image);
            ctx.SaveChanges();
            ctx.Dispose();

            // Select from database
            imageFromDatabase = ctx.Images.FirstOrDefault(i => i.Id == image.Id);

            // Test
            Assert.IsNull(imageFromDatabase);
        }

        [TestMethod]
        public void AddImages()
        {
            /*
            byte[] img1 = ImageService.FromImageToBytes(@"");
            byte[] img2 = ImageService.FromImageToBytes(@"");
            byte[] img3 = ImageService.FromImageToBytes(@"");
            byte[] img4 = ImageService.FromImageToBytes(@"");
            byte[] img5 = ImageService.FromImageToBytes(@"");
            byte[] img6 = ImageService.FromImageToBytes(@"");
            byte[] img7 = ImageService.FromImageToBytes(@"");
            byte[] img8 = ImageService.FromImageToBytes(@"");

            JDSCommon.Database.Models.Image image = new JDSCommon.Database.Models.Image
            {
                Alt = "test",
                Picture = byteImage,
            };

            JDSContext ctx = new JDSContext();

            // Add to database
            ctx.Images.Add(image);
            ctx.SaveChanges();
            */
        }
    }
}