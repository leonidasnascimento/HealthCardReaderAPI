using API.Models;
using System.Collections.Generic;

namespace API.Patterns
{
    /// <summary>
    /// Health card reader strategy class. Used to dynamically instantiate a "HealthCardReader" object 
    /// </summary>
    /// <seealso cref="http://www.dofactory.com/net/strategy-design-pattern"/>
    public class HealthCardReaderStrategy
    {
        /// <summary>
        /// Get the right instance of a Health Card Reader Classe
        /// </summary>
        /// <param name="ocrJsonData">JSON data from the OCR proccess</param>
        /// <param name="acceptedHealthCareProviders">Accepted Health Care Providers</param>
        /// <returns>"HealthCardReader" object instance</returns>
        public HealthCardReader GetHealthCardInstance(string ocrJsonData, List<string> acceptedHealthCareProviders)
        {
            //Validations
            if (string.IsNullOrWhiteSpace(ocrJsonData)) return null; //Can not proceed with empty OCR result
            if (acceptedHealthCareProviders.Count <= 0) return null; //Whether there is no accepted health care providers, no reason to proceed also.

            var readerInstance = default(HealthCardReader);
            var foundHealthCareProvider = string.Empty;

            for (int i = 0; i < acceptedHealthCareProviders.Count; i++)
            {
                if (string.IsNullOrWhiteSpace(acceptedHealthCareProviders[i]))
                    continue; //arrHealthProviders can not have a null position. Goes to next item.

                if (ocrJsonData.ToLowerInvariant().Contains(acceptedHealthCareProviders[i]))
                    foundHealthCareProvider = acceptedHealthCareProviders[i]; //Health care provider found!
            }

            switch (foundHealthCareProvider.ToLowerInvariant())
            {
                case "bradesco":
                    readerInstance = new Bradesco();
                    break;
                case "sulamerica":
                case "sulamérica":
                case "sul américa":
                case "sul america":
                    readerInstance = new SulAmerica();
                    break;
                default:
                    return null;
            }

            return readerInstance;
        }
    }
}