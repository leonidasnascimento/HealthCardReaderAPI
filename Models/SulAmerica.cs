using System;
using System.Collections.Generic;

namespace API.Models
{
    public class SulAmerica : HealthCardReader
    {
        public override EligibilityInfo GetHealthCarePlanElegibility(HealthCardInfo healthCardInfo, string hospital, string medicalExam)
        {
            return new EligibilityInfo();
        }

        public override string GetHealthInsuranceNumber(List<int> logicNumericSequence)
        {
            throw new NotImplementedException();
        }

        public override HealthCardInfo ReadCardInfo(string json)
        {
            var CardsInfoPosition = (ComputerVisionOCR)Newtonsoft.Json.JsonConvert.DeserializeObject(json, typeof(ComputerVisionOCR));
            var nameAux = string.Empty;
            var cardnumberAux = string.Empty;
            var logoAux = string.Empty;
            var companyAux = string.Empty;
            var healthinsuranceAux = string.Empty;

            //Filling 'nameAux'
            if (CardsInfoPosition.regions.Count >= 1 &&
                CardsInfoPosition.regions[0].lines.Count >= 1 &&
                CardsInfoPosition.regions[0].lines[0].words != null)
            

            //Filling 'cardnumberAux'
            if (CardsInfoPosition.regions.Count >= 1 &&
                CardsInfoPosition.regions[2].lines.Count >= 3 &&
                CardsInfoPosition.regions[2].lines[1].words != null)
            {
                for (int i = 0; i < CardsInfoPosition.regions[2].lines[1].words.Count; i++)
                    cardnumberAux += CardsInfoPosition.regions[2].lines[1].words[i].text + " ";
                
            }

            //Filling 'logoAux'
            if (CardsInfoPosition.regions.Count >= 5 &&
                CardsInfoPosition.regions[3].lines.Count >= 2 &&
                CardsInfoPosition.regions[3].lines[4].words.Count >= 4)
            {
                logoAux = CardsInfoPosition.regions[3].lines[4].words[3].text;

            }

            //Filling 'companyAux'
            if (CardsInfoPosition.regions.Count >= 5 &&
                CardsInfoPosition.regions[3].lines.Count >= 2 &&
                CardsInfoPosition.regions[3].lines[4].words.Count >= 4)
            {
                companyAux = CardsInfoPosition.regions[3].lines[4].words[3].text;

            }

            //Filling 'healthinsuranceAux'
            if (CardsInfoPosition.regions.Count >= 2 &&
               CardsInfoPosition.regions[1].lines.Count >= 2 &&
               CardsInfoPosition.regions[1].lines[3].words.Count >= 1)
            {
                healthinsuranceAux = CardsInfoPosition.regions[1].lines[3].words[0].text;
            }
                

            return new HealthCardInfo
            {
                Name = nameAux,
                CardNumber = cardnumberAux,
                HealthInsurance = healthinsuranceAux,
                Logo = logoAux,
                Company = companyAux,

            };
        }
    }
}