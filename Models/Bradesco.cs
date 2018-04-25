using System;

namespace API.Models
{
    public class Bradesco : HealthCardReader
    {
        public override EligibilityInfo GetHealthCarePlanElegibility(HealthCardInfo healthCardInfo, string hospital, string medicalExam)
        {
            return new EligibilityInfo();
        }

        public override HealthCardInfo ReadCardInfo(string json)
        {
            var CardsInfoPosition = (ComputerVisionOCR)Newtonsoft.Json.JsonConvert.DeserializeObject(json, typeof(ComputerVisionOCR));
            var nameAux = string.Empty;
            var cardnumberAux = string.Empty;
            var validdateAux = string.Empty;
            var logoAux = string.Empty;
            var companyAux = string.Empty;

            //Filling 'nameAux'
            if (CardsInfoPosition.regions.Count >= 2 &&
                CardsInfoPosition.regions[0].lines.Count >= 2 &&
                CardsInfoPosition.regions[1].lines.Count >= 4 &&
                CardsInfoPosition.regions[0].lines[1].words.Count >= 2 &&
                CardsInfoPosition.regions[1].lines[3].words.Count >= 1)
            {
                nameAux = CardsInfoPosition.regions[0].lines[1].words[0].text + " " +
                          CardsInfoPosition.regions[0].lines[1].words[1].text + " " +
                          CardsInfoPosition.regions[1].lines[3].words[0].text;
            }

            //Filling 'cardnumberAux'
            if ( CardsInfoPosition.regions.Count >= 2 &&
                CardsInfoPosition.regions[0].lines.Count >= 3 &&
                CardsInfoPosition.regions[1].lines.Count >= 5 &&
                CardsInfoPosition.regions[0].lines[2].words.Count >= 2 &&
                CardsInfoPosition.regions[1].lines[4].words.Count >= 2)
            {
                cardnumberAux = CardsInfoPosition.regions[0].lines[2].words[0].text + " " +
                                CardsInfoPosition.regions[0].lines[2].words[1].text + " " +
                                CardsInfoPosition.regions[1].lines[4].words[0].text + " " +
                                CardsInfoPosition.regions[1].lines[4].words[1].text;
            }

            //Filling 'validdateAux'
            if ( CardsInfoPosition.regions.Count >= 2 &&
                 CardsInfoPosition.regions[1].lines.Count >= 3 &&
                 CardsInfoPosition.regions[1].lines[2].words.Count >= 1)
            {
                validdateAux = CardsInfoPosition.regions[1].lines[2].words[0].text;
            }

            //Filling 'logoAux'
            if ( CardsInfoPosition.regions.Count >= 2 &&
                CardsInfoPosition.regions[1].lines.Count >= 1 &&
                CardsInfoPosition.regions[1].lines[0].words.Count >= 2)
            {
                logoAux = CardsInfoPosition.regions[1].lines[0].words[1].text;
            }

            //Filling 'companyAux'
            if ( CardsInfoPosition.regions.Count >= 2 &&
                 CardsInfoPosition.regions[1].lines.Count >= 2 &&
                 CardsInfoPosition.regions[1].lines[0].words.Count >= 2 &&
                 CardsInfoPosition.regions[1].lines[1].words.Count >= 1)
            {
                companyAux = CardsInfoPosition.regions[1].lines[0].words[1].text + " " +
                             CardsInfoPosition.regions[1].lines[1].words[0].text;
            }

            return new HealthCardInfo
            {
                Name = nameAux,
                CardNumber = cardnumberAux,
                ValidDate = GetValidDate(validdateAux),
                //HealthInsurance
                Logo = logoAux,
                Company = companyAux,

            };
        }
    }
}