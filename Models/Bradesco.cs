using System;
using System.Collections.Generic;
using System.Linq;

namespace API.Models
{
    public class Bradesco : HealthCardReader
    {
        #region Public Methods

        public Bradesco() : base()
        {

        }

        public override EligibilityInfo GetHealthCarePlanElegibility(HealthCardInfo healthCardInfo, string hospital, string medicalExam)
        {
            return new EligibilityInfo();
        }

        /// <summary>
        /// Gets the insured name
        /// </summary>
        /// <param name="ocr">OCR Object</param>
        /// <returns>A name containing the insured name</returns>
        public override string GetInsuredName(ComputerVisionOCR ocr)
        {
            //Validation
            if (ocr is null) return string.Empty;
            if (ocr.RecognitionResult is null) return string.Empty;
            if (ocr.RecognitionResult.Lines is null) return string.Empty;

            var foundName = string.Empty;
            var index = -1;

            //Check if there's a dependent-insered
            if (ocr.RecognitionResult.Lines.Any(line => "02".Equals(line.Text.Trim())))
                index = ocr.RecognitionResult.Lines.FindIndex(line => "02".Equals(line.Text.Trim()));
            else if (ocr.RecognitionResult.Lines.Any(line => "00".Equals(line.Text.Trim()))) //Main insured
                index = ocr.RecognitionResult.Lines.FindIndex(line => "00".Equals(line.Text.Trim()));

            if (index > 0)
                foundName = ocr.RecognitionResult.Lines[index - 1].Text;

            return foundName;
        }

        public override HealthCardInfo ReadCardInfo(string json)
        {
            var expDateIndex = -1;
            var cardInfoPosition = (ComputerVisionOCR)Newtonsoft.Json.JsonConvert.DeserializeObject(json, typeof(ComputerVisionOCR));
            var nameAux = GetInsuredName(cardInfoPosition);
            var cardNumberAux = GetHealthCardInsuranceNumber(cardInfoPosition);
            var expirationDateAux = GetExpirationDate(cardInfoPosition, out expDateIndex);
            var logoAux = "Bradesco Saúde";
            var companyAux = GetCompanyName(cardInfoPosition, expDateIndex);
            var healthinsuranceAux = GetHealthInsurancePlan(cardInfoPosition);

            return new HealthCardInfo
            {
                Name = nameAux,
                CardNumber = cardNumberAux,
                ExpirationDate = expirationDateAux,
                HealthInsurance = healthinsuranceAux,
                Logo = logoAux,
                Company = companyAux,

            };
        }

        /// <summary>
        /// Gets the company name from the OCR object
        /// </summary>
        /// <param name="ocr">OCR Position</param>
        /// <param name="startIndex">Start Index</param>
        /// <returns>Company's name</returns>
        public override string GetCompanyName(ComputerVisionOCR ocr, int startIndex)
        {
            //Validation
            if (ocr is null) return string.Empty;
            if (ocr.RecognitionResult is null) return string.Empty;
            if (ocr.RecognitionResult.Lines is null) return string.Empty;
            if (startIndex <= 0) return string.Empty;

            var companyName = ocr.RecognitionResult.Lines[startIndex - 1].Text;

            return companyName;
        }

        /// <summary>
        /// Get the 'Valid Date' field value given a string
        /// </summary>
        /// <param name="healthInsuranceDate">Date</param>
        /// <returns>Valid Date</returns>
        protected DateTime? GetExpirationDate(ComputerVisionOCR ocr, out int expDateIndex)
        {
            expDateIndex = -1;

            //Validation
            if (ocr is null) return null;
            if (ocr.RecognitionResult is null) return null;
            if (ocr.RecognitionResult.Lines is null) return null;

            DateTime? returnDate = null;
            var index = -1;
            var foundDate = string.Empty;

            if (ocr.RecognitionResult.Lines.Any(line => !string.IsNullOrWhiteSpace(line.Text) && line.Text.Contains("/")))
                index = ocr.RecognitionResult.Lines.FindIndex(line => !string.IsNullOrWhiteSpace(line.Text) && line.Text.Contains("/"));

            if (index > 0)
            {
                foundDate = ocr.RecognitionResult.Lines[index].Text;

                if (!string.IsNullOrWhiteSpace(foundDate))
                {
                    expDateIndex = index;

                    if (foundDate.Split('/').Length == 2)
                        returnDate = new DateTime(int.Parse(string.Concat("20", foundDate.Split('/')[1])), int.Parse(foundDate.Split('/')[0]), GetLastDayOfMonth(int.Parse(foundDate.Split('/')[1]), int.Parse(foundDate.Split('/')[0])));

                    if (foundDate.Split('/').Length == 3)
                        returnDate = new DateTime(int.Parse(string.Concat("20", foundDate.Split('/')[2])), int.Parse(foundDate.Split('/')[1]), int.Parse(foundDate.Split('/')[0]));
                }
            }

            return returnDate;
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

                return string.Concat(ocr.RecognitionResult.Lines[index].Text, " ", ocr.RecognitionResult.Lines[index - 1].Text);
            }

            return string.Empty;
        }

        #endregion Public Methods

        #region Internal Methods

        #endregion Internal Methods
    }
}