using ImageMagick;
using System;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Web.Http.Cors;
using System.Web.Mvc;

namespace API.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class HomeController : Controller
    {
        //[HttpPost]
        public string ProcessarImagem(string img)
        {
            if (Request.HttpMethod != "POST") throw new Exception("Ainda não é POST... Tenta de novo! :)");

            try
            {
                //var imgEnhanced = EnhanceImage(Request.InputStream);
                //var readData = await ImageOCRAsync(new MemoryStream(imgEnhanced));
                //return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(readData, System.Text.Encoding.UTF8, "application/json") };

                return "Alô, mundo!";
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
        private byte[] EnhanceImage(Stream inputStream)
        {
            using (MagickImage image = new MagickImage(inputStream))
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
        /// Send a stram (image) to the cloud for computer vision processing & optical charactere recogniton (OCR)
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
