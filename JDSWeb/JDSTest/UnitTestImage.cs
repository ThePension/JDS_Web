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
            //byte[] ib4 = ImageService.FromImageToBytes(@"C:\Users\Théo\Pictures\HE-Arc\HES-ETE\jds_cap.jpg");
            //byte[] ib5 = ImageService.FromImageToBytes(@"C:\Users\Théo\Pictures\HE-Arc\HES-ETE\jds_hat.jpg");
            byte[] ib6 = ImageService.FromImageToBytes(@"C:\Users\Théo\Pictures\HE-Arc\HES-ETE\jds_tshirt.jpg");
            //byte[] ib7 = ImageService.FromImageToBytes(@"C:\Users\Théo\Pictures\HE-Arc\HES-ETE\jds_yellow_pullover.jpg");
            //byte[] ib8 = ImageService.FromImageToBytes(@"C:\Users\Théo\Pictures\HE-Arc\HES-ETE\jds_blue_pullover.jpg");

            //JDSCommon.Database.Models.Image img4 = new JDSCommon.Database.Models.Image
            //{
            //    Alt = "Cap",
            //    Picture = ib4,
            //};

            //JDSCommon.Database.Models.Image img5 = new JDSCommon.Database.Models.Image
            //{
            //    Alt = "Hat",
            //    Picture = ib5,
            //};

            JDSCommon.Database.Models.Image img6 = new JDSCommon.Database.Models.Image
            {
                Alt = "T-Shirt",
                Picture = ib6,
            };

            //JDSCommon.Database.Models.Image img7 = new JDSCommon.Database.Models.Image
            //{
            //    Alt = "Yellow pullover",
            //    Picture = ib7,
            //};

            //JDSCommon.Database.Models.Image img8 = new JDSCommon.Database.Models.Image
            //{
            //    Alt = "Blue pullover",
            //    Picture = ib8,
            //};

            JDSContext ctx = new JDSContext();

            //// Add to database
            //ctx.Images.Add(img4);
            //ctx.Images.Add(img5);
            ctx.Images.Add(img6);
            //ctx.Images.Add(img7);
            //ctx.Images.Add(img8);

            ctx.SaveChanges();

            ctx.Dispose();
        }
    }
}