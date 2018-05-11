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

        internal override string GetHealthCardInsuranceNumber(ComputerVisionOCR ocrData)
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

            return new HealthCardInfo
            {
                Name = nameAux,
                CardNumber = cardnumberAux,
                HealthInsurance = healthinsuranceAux,
                Logo = logoAux,
                Company = companyAux,

            };
        }

        public override string GetInsuredName(ComputerVisionOCR ocr)
        {
            throw new NotImplementedException();
        }

        public override string GetCompanyName(ComputerVisionOCR ocr, int startIndex)
        {
            throw new NotImplementedException();
        }

        public override string GetHealthInsurancePlan(ComputerVisionOCR ocr)
        {
            throw new NotImplementedException();
        }

        public SulAmerica() : base()
        {

        }
    }
}