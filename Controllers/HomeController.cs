using API.Models;
using API.Patterns;
using ImageMagick;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Cors;
using System.Web.Mvc;

namespace API.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class HomeController : Controller
    {
        #region HTTP Methods

        [HttpPost]
        public string ProcessarImagemAsync(string image)
        {
            //'InputStram' validations
            if (string.IsNullOrWhiteSpace(image))
                return "There's no image on the 'image' parameter... Please try again sending a valid image format.";

            if (!ConfigurationManager.AppSettings.AllKeys.Contains("ACCEPTED_HEALTH_PROVIDERS"))
                return "No allowed health care provider found in configurations. Please, contact your system administrator.";

            try
            {
                var readData = default(string);
                var bytes = Convert.FromBase64String(image);
                var healthCardReader = default(HealthCardReader);
                var healthCardInfo = default(HealthCardInfo);
                var healthCareProviderList = ConfigurationManager.AppSettings["ACCEPTED_HEALTH_PROVIDERS"].Split(',').ToList();

                readData = ImageOCRAsync(bytes);
                healthCardReader = new HealthCardReaderStrategy().GetHealthCardInstance(readData, healthCareProviderList);

                if (!(healthCardReader is null)) //Getting the "first" health card informations
                    healthCardInfo = healthCardReader.ReadCardInfo(readData);

                if (!(healthCardInfo is null)) //Getting the elegibility for a medical exam & hospital
                    healthCardInfo.AddEligibility(healthCardReader.GetHealthCarePlanElegibility(healthCardInfo, "HIAE", string.Empty));

                if (!(healthCardInfo is null))
                    return Newtonsoft.Json.JsonConvert.SerializeObject(healthCardInfo);
                else
                    return "No data found from the given Health Card";
            }

            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        #endregion HTTP Methods

        #region Private Methods

        /// <summary>
        /// Method used for save image for visual validation
        /// </summary>
        /// <param name="image">Image</param>
        private static void SaveImageForVisualValidation(MagickImage image)
        {
            var appBase = AppContext.BaseDirectory;
            var dirName = Path.GetDirectoryName(appBase);
            var path = Path.Combine(dirName, "TesteImg.jpg");

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
        private string ImageOCRAsync(byte[] imgStream)
        {
            var compVisionUrl = ConfigurationManager.AppSettings["COMPUTER_VISION_URL"];
            var defaultLang = ConfigurationManager.AppSettings["COMPUTER_VISION_DEFAULT_LANGUAGE"];
            var cvSubscriptionKey = ConfigurationManager.AppSettings["COMPUTER_VISION_SUBSCRIPTION_KEY"];
            var client = new HttpClient();
            var fullUrl = string.Format(compVisionUrl, string.Format("language={0}&detectOrientation=false", defaultLang));
            var byteContent = new ByteArrayContent(imgStream);

            byteContent.Headers.Add("Content-Type", "application/octet-stream");
            byteContent.Headers.Add("Ocp-Apim-Subscription-Key", cvSubscriptionKey);

            var response = client.PostAsync(fullUrl, byteContent).Result;
            var responseString = response.Content.ReadAsStringAsync().Result;

            return responseString;
        }

        #endregion Private Methods
    }
}