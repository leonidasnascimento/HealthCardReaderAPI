using System;
using System.Collections.Generic;
using System.Linq;

namespace API.Models
{
    public class SulAmerica : HealthCardReader
    {
        public SulAmerica(string json) : base(json)
        {

        }

        public override EligibilityInfo GetHealthCarePlanElegibility(HealthCardInfo healthCardInfo, string hospital, string medicalExam)
        {
            return new EligibilityInfo();
        }

        public override HealthCardInfo ReadCardInfo(string json)
        {
            var nameAux = GetInsuredName();
            var cardNumberAux = GetHealthCardInsuranceNumber();
            var logoAux = "SulAmérica";
            var companyAux = GetCompanyName();
            var healthinsuranceAux = GetHealthInsurancePlan();

            return new HealthCardInfo
            {
                Name = nameAux,
                CardNumber = cardNumberAux,
                HealthInsurance = healthinsuranceAux,
                Logo = logoAux,
                Company = companyAux,
            };
        }

        public override string GetInsuredName()
        {
            //Validation
            if (OCR is null) return string.Empty;
            if (OCR.RecognitionResult is null) return string.Empty;
            if (OCR.RecognitionResult.Lines is null) return string.Empty;

            var foundName = string.Empty;

            if (OCR.RecognitionResult.Lines.Count >= 1)
            {
                foundName = OCR.RecognitionResult.Lines[0].Text;
                return foundName;

            }
            return null;

        }

        /// <summary>
        /// Gets the company name from the OCR object
        /// </summary>
        /// <param name="ocr">OCR Position</param>
        /// <param name="startIndex">Start Index</param>
        /// <returns>Company's name</returns>
        public string GetCompanyName()
        {
            var startIndex = 0;
            var companyName = string.Empty;
            //Validation
            if (OCR is null) return string.Empty;
            if (OCR.RecognitionResult is null) return string.Empty;
            if (OCR.RecognitionResult.Lines is null) return string.Empty;


            if (OCR.RecognitionResult.Lines.Any(line => "empresa:".Equals(line.Text.ToLowerInvariant().Trim())))
                startIndex = OCR.RecognitionResult.Lines.FindIndex(line => "empresa:".Equals(line.Text.ToLowerInvariant().Trim()));

            if (startIndex > 0)
                companyName = OCR.RecognitionResult.Lines[startIndex + 1].Text;

            return companyName;
        }

        public override string GetHealthInsurancePlan()
        {
            //Validation
            if (OCR is null) return null;
            if (OCR.RecognitionResult is null) return null;
            if (OCR.RecognitionResult.Lines is null) return null;
            if (Configuration is null) return string.Empty;
            if (string.IsNullOrWhiteSpace(Configuration.AcceptedPlan)) return string.Empty;

            var acceptedPlans = Configuration.AcceptedPlan.Split(',');
            var lstPlansAux = new List<string>();
            var index = -1;

            foreach (var plan in acceptedPlans)
            {
                lstPlansAux = OCR.RecognitionResult.Lines
                    .Where(line => !string.IsNullOrWhiteSpace(line.Text) && line.Text.ToLowerInvariant().Trim().Contains(plan.ToLowerInvariant()))
                    .Select(line => line.Text)
                    .ToList();

                if (lstPlansAux is null) continue;
                if (lstPlansAux.Count > 1) continue;

                index = OCR.RecognitionResult.Lines.FindIndex(line => !string.IsNullOrWhiteSpace(line.Text) && line.Text.ToLowerInvariant().Trim().Contains(plan.ToLowerInvariant()));

                if (index <= 0) continue;

                return string.Concat(OCR.RecognitionResult.Lines[index].Text);
            }

            return string.Empty;
        }
    }
}