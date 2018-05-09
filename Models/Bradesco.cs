using System.Collections.Generic;
using System.Linq;

namespace API.Models
{
    public class Bradesco : HealthCardReader
    {
        #region Public Methods

        public override EligibilityInfo GetHealthCarePlanElegibility(HealthCardInfo healthCardInfo, string hospital, string medicalExam)
        {
            return new EligibilityInfo();
        }

        public override HealthCardInfo ReadCardInfo(string json)
        {
            var cardInfoPosition = (ComputerVisionOCR)Newtonsoft.Json.JsonConvert.DeserializeObject(json, typeof(ComputerVisionOCR));
            var nameAux = string.Empty;
            var cardNumberAux = GetHealthCardInsuranceNumber(cardInfoPosition);
            var validdateAux = string.Empty;
            var logoAux = string.Empty;
            var companyAux = string.Empty;
            var healthinsuranceAux = string.Empty;

            //Filling 'nameAux'
            if (cardInfoPosition.regions.Count >= 3 &&
                cardInfoPosition.regions[2].lines.Count >= 1 &&
                cardInfoPosition.regions[2].lines[0].words != null)
            {
                for (int i = 0; i < cardInfoPosition.regions[2].lines[0].words.Count; i++)
                    nameAux += cardInfoPosition.regions[2].lines[0].words[i].text + " ";

            }

            //Filling 'validdateAux'
            if (cardInfoPosition.regions.Count >= 4 &&
                 cardInfoPosition.regions[3].lines.Count >= 4 &&
                 cardInfoPosition.regions[3].lines[3].words.Count >= 1)
            {
                validdateAux = cardInfoPosition.regions[3].lines[3].words[0].text;
            }

            //Filling 'logoAux'
            if (cardInfoPosition.regions.Count >= 4 &&
                cardInfoPosition.regions[3].lines.Count >= 1 &&
                cardInfoPosition.regions[3].lines[0].words.Count >= 1)
            {
                logoAux = cardInfoPosition.regions[3].lines[0].words[0].text;
            }

            //Filling 'companyAux'
            if (cardInfoPosition.regions.Count >= 4 &&
                cardInfoPosition.regions[3].lines.Count >= 1 &&
                cardInfoPosition.regions[3].lines[0].words.Count >= 1)
            {
                companyAux = cardInfoPosition.regions[3].lines[0].words[0].text;
            }

            //Filling 'healthinsuranceAux'
            if (cardInfoPosition.regions.Count >= 1 &&
               cardInfoPosition.regions[0].lines.Count >= 1 &&
               cardInfoPosition.regions[0].lines[0].words.Count >= 1)
            {
                healthinsuranceAux = cardInfoPosition.regions[0].lines[0].words[0].text;
            }

            return new HealthCardInfo
            {
                Name = nameAux,
                CardNumber = cardNumberAux,
                ValidDate = GetValidDate(validdateAux),
                HealthInsurance = healthinsuranceAux,
                Logo = logoAux,
                Company = companyAux,

            };
        }

        #endregion Public Methods

        #region Internal Methods

        /// <summary>
        /// Gets the health card insurance-number given
        /// </summary>
        /// <returns>The health card insurance number</returns>
        internal override string GetHealthCardInsuranceNumber(ComputerVisionOCR ocrData)
        {
            //Validations
            if (Configuration is null) return string.Empty;
            if (Configuration.CardInsuranceNumberLengthSequence is null) return string.Empty;
            if (Configuration.CardInsuranceNumberLengthSequence.Count == 0) return string.Empty;
            if (ocrData is null) return string.Empty;

            //Filling 'cardnumberAux'
            var cardNumber = string.Empty;
            var numAux = 0;
            var wordAux = string.Empty;
            var indexToRemove = new List<int>();
            var reindex = false;

            for (int countRegions = 0; countRegions < ocrData.regions.Count; countRegions++)
            {
                for (int countLines = 0; countLines < ocrData.regions[countRegions].lines.Count; countLines++)
                {
                    for (int countSeq = 0; countSeq < Configuration.CardInsuranceNumberLengthSequence.Count; countSeq++)
                    {
                        for (int countWords = 0; countWords < ocrData.regions[countRegions].lines[countLines].words.Count; countWords++)
                        {
                            wordAux = ocrData.regions[countRegions].lines[countLines].words[countWords].text;

                            //This item can not repeat through the iteration... Foward only!
                            ocrData.regions[countRegions].lines[countLines].words[countWords].text = string.Empty;

                            if (wordAux.Length == Configuration.CardInsuranceNumberLengthSequence[countSeq])
                            {
                                if (int.TryParse(wordAux, out numAux))
                                {
                                    indexToRemove.Add(countSeq);
                                    cardNumber += wordAux;
                                    break;
                                }
                            }
                        }

                        //Algorithm optimization -- Removing all empty words
                        ocrData.regions[countRegions].lines[countLines].words.RemoveAll(word => string.Empty.Equals(word.text));

                        //Goes to the next line if there's any word...
                        if (ocrData.regions[countRegions].lines[countLines].words.Count == 0) break;
                    }

                    if (indexToRemove.Count > 0)
                    {
                        for (int countIndexRemove = 0; countIndexRemove < indexToRemove.Count; countIndexRemove++)
                            Configuration.CardInsuranceNumberLengthSequence[countIndexRemove] = -1;

                        indexToRemove = new List<int>();
                    }
                }
            }

            return cardNumber;
        }

        #endregion Internal Methods
    }
}