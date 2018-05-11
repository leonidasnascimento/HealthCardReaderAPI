using System;
using System.Collections.Generic;
using System.Linq;

namespace API.Models
{
    public class SulAmerica : HealthCardReader
    {
        public SulAmerica() : base()
        {

        }
        public override EligibilityInfo GetHealthCarePlanElegibility(HealthCardInfo healthCardInfo, string hospital, string medicalExam)
        {
            return new EligibilityInfo();
        }

    

        public override HealthCardInfo ReadCardInfo(string json)
        {
            
            var cardInfoPosition = (ComputerVisionOCR)Newtonsoft.Json.JsonConvert.DeserializeObject(json, typeof(ComputerVisionOCR));
            var nameAux = GetInsuredName(cardInfoPosition);
            var cardNumberAux = GetHealthCardInsuranceNumber(cardInfoPosition);
            var logoAux = "SulAmérica";
            var companyAux = GetCompanyName1(cardInfoPosition);
            var healthinsuranceAux = GetHealthInsurancePlan(cardInfoPosition);

            return new HealthCardInfo
            {
                Name = nameAux,
                CardNumber = cardNumberAux,
                HealthInsurance = healthinsuranceAux,
                Logo = logoAux,
                Company = companyAux,
            };
        }

        public override string GetInsuredName(ComputerVisionOCR ocr)
        {
            //Validation
            if (ocr is null) return string.Empty;
            if (ocr.RecognitionResult is null) return string.Empty;
            if (ocr.RecognitionResult.Lines is null) return string.Empty;

            var foundName = string.Empty;

            if(ocr.RecognitionResult.Lines.Count >= 1)
            {
                foundName = ocr.RecognitionResult.Lines[0].Text;
                return foundName;
            }return null;
            
        }
        /// <summary>
        /// Gets the company name from the OCR object
        /// </summary>
        /// <param name="ocr">OCR Position</param>
        /// <param name="startIndex">Start Index</param>
        /// <returns>Company's name</returns>
        /// 


        public override string GetCompanyName1(ComputerVisionOCR ocr)
        {
            var startIndex = 0;
            var companyName = string.Empty;
            //Validation
            if (ocr is null) return string.Empty;
            if (ocr.RecognitionResult is null) return string.Empty;
            if (ocr.RecognitionResult.Lines is null) return string.Empty;
            
            
            if (ocr.RecognitionResult.Lines.Any(line => "EMPRESA:".Equals(line.Text.Trim())))
                startIndex = ocr.RecognitionResult.Lines.FindIndex(line => "EMPRESA:".Equals(line.Text.Trim()));

            if(startIndex >0 )
            companyName = ocr.RecognitionResult.Lines[startIndex + 1].Text;

            return companyName;
        }

        public override string GetHealthInsurancePlan(ComputerVisionOCR ocr)
        {
            //Validation
            if (ocr is null) return null;
            if (ocr.RecognitionResult is null) return null;
            if (ocr.RecognitionResult.Lines is null) return null;
            if (Configuration is null) return string.Empty;
            if (string.IsNullOrWhiteSpace(Configuration.AcceptedPlan)) return string.Empty;

            var acceptedPlans = Configuration.AcceptedPlan.Split(',');
            var lstPlansAux = new List<string>();
            var index = -1;

            foreach (var plan in acceptedPlans)
            {
                lstPlansAux = ocr.RecognitionResult.Lines
                    .Where(line => !string.IsNullOrWhiteSpace(line.Text) && line.Text.ToLowerInvariant().Trim().Contains(plan.ToLowerInvariant()))
                    .Select(line => line.Text)
                    .ToList();

                if (lstPlansAux is null) continue;
                if (lstPlansAux.Count > 1) continue;

                index = ocr.RecognitionResult.Lines.FindIndex(line => !string.IsNullOrWhiteSpace(line.Text) && line.Text.ToLowerInvariant().Trim().Contains(plan.ToLowerInvariant()));

                if (index <= 0) continue;

                return string.Concat(ocr.RecognitionResult.Lines[index].Text);
            }

            return string.Empty;
            }

        

        public override string GetCompanyName(ComputerVisionOCR ocr, int startIndex)
        {
            throw new NotImplementedException();
        }
    }
}