﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace API.Models
{
    public class Bradesco : HealthCardReader
    {
        #region Public Methods

        public Bradesco(string json) : base(json)
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
        public override string GetInsuredName()
        {
            return GetInsuredName(-1);
        }

        /// <summary>
        /// Gets the insured name
        /// </summary>
        /// <param name="ocr">OCR Object</param>
        /// <returns>A name containing the insured name</returns>
        public string GetInsuredName(int expirationDataIndex)
        {
            //Validation
            if (OCR is null) return string.Empty;
            if (OCR.RecognitionResult is null) return string.Empty;
            if (OCR.RecognitionResult.Lines is null) return string.Empty;

            var foundName = string.Empty;
            var index = -1;

            //Replancing "oo" by "00" & "o2" by "02"
            if (OCR.RecognitionResult.Lines.Any(line => "oo".Equals(line.Text.Trim())))
                OCR.RecognitionResult.Lines.Where(line => "oo".Equals(line.Text.Trim())).All(line => { line.Text = "00"; return true; });
            if (OCR.RecognitionResult.Lines.Any(line => "o2".Equals(line.Text.Trim())))
                OCR.RecognitionResult.Lines.Where(line => "o2".Equals(line.Text.Trim())).All(line => { line.Text = "02"; return true; }); ;

            //Check if there's a dependent-insered
            if (OCR.RecognitionResult.Lines.Any(line => "02".Equals(line.Text.Trim())))
                index = OCR.RecognitionResult.Lines.FindIndex(line => "02".Equals(line.Text.Trim()));
            else if (OCR.RecognitionResult.Lines.Any(line => "00".Equals(line.Text.Trim()))) //Main insured
                index = OCR.RecognitionResult.Lines.FindIndex(line => "00".Equals(line.Text.Trim()));

            if (index > 0)
            {
                foundName = OCR.RecognitionResult.Lines[index - 1].Text;
            }
            else if (expirationDataIndex > 0 && OCR.RecognitionResult.Lines.Count > (expirationDataIndex + 1) && HasOnlyCharacters(OCR.RecognitionResult.Lines[expirationDataIndex + 1].Text))
            {
                foundName = OCR.RecognitionResult.Lines[expirationDataIndex + 1].Text;
            }
            else
            {
                //Looking for a name starting from the end of the OCR
                for (var countLines = OCR.RecognitionResult.Lines.Count - 1; countLines > 0; countLines--)
                {
                    if (HasOnlyCharacters(OCR.RecognitionResult.Lines[countLines].Text))
                    {
                        foundName = OCR.RecognitionResult.Lines[countLines].Text;
                        break;
                    }
                }
            }

            return foundName;
        }

        public override HealthCardInfo ReadCardInfo(string json)
        {
            var expDateIndex = -1;

            //Removing all words that should be ignored
            RemoveWordsToIgnore();

            var expirationDateAux = GetExpirationDate(out expDateIndex);
            var nameAux = GetInsuredName(expDateIndex);
            var cardNumberAux = GetHealthCardInsuranceNumber();
            var logoAux = "Bradesco Saúde";
            var companyAux = GetCompanyName(expDateIndex);
            var healthinsuranceAux = GetHealthInsurancePlan();

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
        public string GetCompanyName(int startIndex)
        {
            //Validation
            if (OCR is null) return string.Empty;
            if (OCR.RecognitionResult is null) return string.Empty;
            if (OCR.RecognitionResult.Lines is null) return string.Empty;
            if (startIndex <= 0) return string.Empty;

            var companyName = OCR.RecognitionResult.Lines[startIndex - 1].Text;

            return companyName;
        }

        /// <summary>
        /// Get the 'Valid Date' field value given a string
        /// </summary>
        /// <param name="healthInsuranceDate">Date</param>
        /// <returns>Valid Date</returns>
        protected DateTime? GetExpirationDate(out int expDateIndex)
        {
            expDateIndex = -1;

            //Validation
            if (OCR is null) return null;
            if (OCR.RecognitionResult is null) return null;
            if (OCR.RecognitionResult.Lines is null) return null;

            DateTime? returnDate = null;
            var index = -1;
            var foundDate = string.Empty;

            if (OCR.RecognitionResult.Lines.Any(line => !string.IsNullOrWhiteSpace(line.Text) && line.Text.Contains("/") && line.Text.Split('/').Length == 2))
                index = OCR.RecognitionResult.Lines.FindIndex(line => !string.IsNullOrWhiteSpace(line.Text) && line.Text.Contains("/") && line.Text.Split('/').Length == 2);

            if (index > 0)
            {
                foundDate = OCR.RecognitionResult.Lines[index].Text;

                if (!string.IsNullOrWhiteSpace(foundDate))
                {
                    expDateIndex = index;

                    if (foundDate.Split('/').Length == 2)
                        returnDate = new DateTime(int.Parse(string.Concat("20", foundDate.Split('/')[1].Trim())), int.Parse(foundDate.Split('/')[0].Trim()), GetLastDayOfMonth(int.Parse(foundDate.Split('/')[1].Trim()), int.Parse(foundDate.Split('/')[0].Trim())));

                    if (foundDate.Split('/').Length == 3)
                        returnDate = new DateTime(int.Parse(string.Concat("20", foundDate.Split('/')[2].Trim())), int.Parse(foundDate.Split('/')[1].Trim()), int.Parse(foundDate.Split('/')[0].Trim()));
                }
            }

            return returnDate;
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

                return string.Concat(OCR.RecognitionResult.Lines[index].Text, " ", OCR.RecognitionResult.Lines[index - 1].Text);
            }

            return string.Empty;
        }

        #endregion Public Methods

        #region Internal Methods

        #endregion Internal Methods
    }
}