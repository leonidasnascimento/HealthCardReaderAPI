using ImageMagick;
using System;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http.Cors;
using System.Web.Mvc;


namespace API.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class HomeController : Controller
    {

        [HttpPost]
        public string ProcessarImagemAsync(string image)
         {
            //'InputStram' validations
            if (string.IsNullOrWhiteSpace(image))
                return "There's no image on the 'image' parameter... Please try again sending a valid image format."; 
                
            try
            {
                var imgEnhanced = default(byte[]);
                var readData = default(Task<string>);
                var bytes = Convert.FromBase64String(image);

                imgEnhanced = EnhanceImage(bytes);
                readData = ImageOCRAsync(new MemoryStream(imgEnhanced));

                if (!(readData is null))
                    return readData.Result;
                else
                    return "There's no processed data to read!";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

        
        /// <summary>
        /// Method user for image enhancement
        /// </summary>
        /// <param name="inputStream">Image input stream</param>
        /// <returns>Byte array on an enhanced image</returns>
        private byte[] EnhanceImage(byte[] input)
        {
            if (input is null) return null;

            using (MagickImage image = new MagickImage(input))
            {
                image.Grayscale(PixelIntensityMethod.Average); //Turned it to black and white
                image.Contrast(true); //Turned up the contrast
                image.Negate(); //Inverted back and white

                //Chcking if the image was correctly adjusted according to the steps above
                SaveImageForVisualValidation(image);

                return image.ToByteArray(MagickFormat.Jpg);
            }
        }

        /// <summary>
        /// Method used for save image for visual validation
        /// </summary>
        /// <param name="image">Image</param>
        private static void SaveImageForVisualValidation(MagickImage image)
        {
            var path = Path.Combine(ConfigurationManager.AppSettings["PATH_FOR_VISUAL_VALIDATION"], "TesteImg.jpg");

            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);

            using (var memory = new MemoryStream(image.ToByteArray(MagickFormat.Jpg)))
                memory.WriteTo(new FileStream(path, FileMode.CreateNew));
        }

        /// <summary>
        /// Send a stram (image) to the clo\ud for computer vision processing & optical charactere recogniton (OCR)
        /// </summary>
        /// <param name="imgStream">Image in stream</param>
        /// <returns></returns>
        private async System.Threading.Tasks.Task<string> ImageOCRAsync(Stream imgStream)
        {
            var contentArray = default(byte[]);

            using (var ms = new MemoryStream())
            {
                imgStream.CopyTo(ms);
                contentArray = ms.ToArray();
            }

            var compVisionUrl = ConfigurationManager.AppSettings["COMPUTER_VISION_URL"];
            var defaultLang = ConfigurationManager.AppSettings["COMPUTER_VISION_DEFAULT_LANGUAGE"];
            var cvSubscriptionKey = ConfigurationManager.AppSettings["COMPUTER_VISION_SUBSCRIPTION_KEY"];
            var client = new HttpClient();
            var fullUrl = string.Format(compVisionUrl, string.Format("language={0}&detectOrientation=true", defaultLang));
            var byteContent = new ByteArrayContent(contentArray);

            byteContent.Headers.Add("Content-Type", "application/octet-stream");
            byteContent.Headers.Add("Ocp-Apim-Subscription-Key", cvSubscriptionKey);

            var response = await client.PostAsync(fullUrl, byteContent);
            var responseString = await response.Content.ReadAsStringAsync();

            return responseString;
        }
    }
}
