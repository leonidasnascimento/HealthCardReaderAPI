using API.Models;
using API.Patterns;
using ImageMagick;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Cors;
using System.Web.Mvc;

namespace API.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class HomeController : Controller
    {
        #region HTTP Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
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
                var readData = string.Empty;
                var recognizeTextResponse = string.Empty;
                var bytes = Convert.FromBase64String(image);
                var healthCardReader = default(HealthCardReader);
                var healthCardInfo = default(HealthCardInfo);
                var healthCareProviderList = ConfigurationManager.AppSettings["ACCEPTED_HEALTH_PROVIDERS"].Split(',').ToList();

                recognizeTextResponse = PostImageForOCR(bytes);
                readData = GetOCRResponse(recognizeTextResponse);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="imageUrl"></param>
        /// <returns></returns>
        [HttpPost]
        public string GetImageFromUrl(string imageUrl)
        {
            var data = default(byte[]);

            using (var webClient = new WebClient())
            {
                data = webClient.DownloadData(imageUrl);

                return ProcessarImagemAsync(Convert.ToBase64String(data));
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
        private string PostImageForOCR(byte[] imgStream)
        {
            var compVisionUrl = ConfigurationManager.AppSettings["COMPUTER_VISION_URL"];
            var defaultLang = ConfigurationManager.AppSettings["COMPUTER_VISION_DEFAULT_LANGUAGE"];
            var cvSubscriptionKey = ConfigurationManager.AppSettings["COMPUTER_VISION_SUBSCRIPTION_KEY"];
            var byteContent = new ByteArrayContent(imgStream);

            byteContent.Headers.Add("Content-Type", "application/octet-stream");
            byteContent.Headers.Add("Ocp-Apim-Subscription-Key", cvSubscriptionKey);

            return Post(compVisionUrl, byteContent, true, "Operation-Location");
        }

        /// <summary>
        /// Gets the processed image OCR response
        /// </summary>
        /// <param name="url">Service URL</param>
        /// <returns>Response JSON</returns>
        private string GetOCRResponse(string url)
        {
            //Wait for the image to be processed
            System.Threading.Thread.Sleep(3000);

            var response = Get(url, new Dictionary<string, string> { { "Ocp-Apim-Subscription-Key", ConfigurationManager.AppSettings["COMPUTER_VISION_SUBSCRIPTION_KEY"] } });

            return response;
        }

        /// <summary>
        /// Post some content to a given service
        /// </summary>
        /// <typeparam name="TInputContent">Content Type. MUST be inherited from "HttpContent"</typeparam>
        /// <param name="url">Service URL</param>
        /// <param name="inputContent">Content</param>
        /// <returns>Service's response JSON</returns>
        private string Post<TInputContent>(string url, TInputContent inputContent, bool returnResponseHeaderContent = false, string headKey = "") where TInputContent : HttpContent
        {
            var client = new HttpClient();

            var response = client.PostAsync(url, inputContent).Result;
            var responseString = string.Empty;

            if (!returnResponseHeaderContent)
                responseString = response.Content.ReadAsStringAsync().Result;
            else
            {
                if (string.IsNullOrWhiteSpace(headKey))
                {
                    foreach (var head in response.Headers)
                        responseString = string.Concat(responseString, head.Key, ": ", head.Value, Environment.NewLine);
                }
                else
                {
                    if (response.Headers.Contains(headKey))
                        responseString = response.Headers.GetValues(headKey).FirstOrDefault();
                }
            }

            return responseString;
        }

        /// <summary>
        /// Gets some content from a given service
        /// </summary>
        /// <param name="url">Service URL</param>
        /// <param name="headerCollection">Request header collection</param>
        /// <returns>Service's response JSON</returns>
        private string Get(string url, Dictionary<string, string> headerCollection)
        {
            var response = default(HttpResponseMessage);
            var client = new HttpClient();
            var responseString = string.Empty;

            foreach (var item in headerCollection)
                client.DefaultRequestHeaders.Add(item.Key, item.Value);

            response = client.GetAsync(url).Result;
            responseString = response.Content.ReadAsStringAsync().Result;

            return responseString;
        }

        #endregion Private Methods
    }
}