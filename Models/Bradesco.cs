using System;
using System.Collections.Generic;

namespace API.Models
{
    public class Bradesco : HealthCardReader
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
            var validdateAux = string.Empty;
            var logoAux = string.Empty;
            var companyAux = string.Empty;
            var healthinsuranceAux = string.Empty;

            //Filling 'nameAux'
            if (CardsInfoPosition.regions.Count >= 3 &&
                CardsInfoPosition.regions[2].lines.Count >= 1 &&
                CardsInfoPosition.regions[2].lines[0].words != null)
            {
                for (int i = 0; i < CardsInfoPosition.regions[2].lines[0].words.Count; i++)
                    nameAux += CardsInfoPosition.regions[2].lines[0].words[i].text + " ";

            }

            //Filling 'cardnumberAux'
            var strNum3 = "776";
            var strNum6 = "354689";
            var number3digits = string.Empty;
            var number6digits = string.Empty;
            var cardNumber = string.Concat(number3digits);
            var cardNumber6 = string.Empty;
            int valorAux;

            for (int i = 0; i < CardsInfoPosition.regions.Count; i++) {
                for (int y = 0; y < CardsInfoPosition.regions[i].lines.Count; y++)
                {
                    for (int t = 0; t< CardsInfoPosition.regions[i].lines[y].words.Count; t++)
                    {
                        if (CardsInfoPosition.regions[i].lines[y].words[t].text.Length == strNum3.Length)
                        {
                            number3digits = CardsInfoPosition.regions[i].lines[y].words[t].text;
                            if(int.TryParse(number3digits, out valorAux))
                            cardNumber += number3digits;
                           
                        }
                    }
                   
                }
                
            }
            for (int i = 0; i < CardsInfoPosition.regions.Count; i++)
            {
                for (int y = 0; y < CardsInfoPosition.regions[i].lines.Count; y++)
                {
                    for (int t = 0; t < CardsInfoPosition.regions[i].lines[y].words.Count; t++)
                    {
                        if (CardsInfoPosition.regions[i].lines[y].words[t].text.Length == strNum6.Length)
                        {
                            number6digits = CardsInfoPosition.regions[i].lines[y].words[t].text;
                            if (int.TryParse(number6digits, out valorAux))
                                cardNumber6 = number6digits;
                        }
                    }

                }

            }
            var teste = cardNumber.Substring(0, 6);
            var teste2 = cardNumber.Substring(6, 3);
            cardnumberAux = teste + cardNumber6 + teste2;
            
           
            //Filling 'validdateAux'
            if ( CardsInfoPosition.regions.Count >= 4 &&
                 CardsInfoPosition.regions[3].lines.Count >= 4 &&
                 CardsInfoPosition.regions[3].lines[3].words.Count >= 1)
            {
                validdateAux = CardsInfoPosition.regions[3].lines[3].words[0].text;
            }

            //Filling 'logoAux'
            if ( CardsInfoPosition.regions.Count >= 4 &&
                CardsInfoPosition.regions[3].lines.Count >= 1 &&
                CardsInfoPosition.regions[3].lines[0].words.Count >= 1)
            {
                logoAux = CardsInfoPosition.regions[3].lines[0].words[0].text;
            }

            //Filling 'companyAux'
            if (CardsInfoPosition.regions.Count >= 4 &&
                CardsInfoPosition.regions[3].lines.Count >= 1 &&
                CardsInfoPosition.regions[3].lines[0].words.Count >= 1)
            {
                companyAux = CardsInfoPosition.regions[3].lines[0].words[0].text;
            }

            //Filling 'healthinsuranceAux'
            if (CardsInfoPosition.regions.Count >= 1 &&
               CardsInfoPosition.regions[0].lines.Count >= 1 &&
               CardsInfoPosition.regions[0].lines[0].words.Count >= 1)
            {
                healthinsuranceAux = CardsInfoPosition.regions[0].lines[0].words[0].text;
            }

            return new HealthCardInfo
            {
                Name = nameAux,
                CardNumber = cardnumberAux,
                ValidDate = GetValidDate(validdateAux),
                HealthInsurance = healthinsuranceAux,
                Logo = logoAux,
                Company = companyAux,

            };
        }

    }
}