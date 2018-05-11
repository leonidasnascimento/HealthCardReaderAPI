using System;
using System.Configuration;
using System.Linq;

namespace API.Models
{
    /// <summary>
    /// the class HealthCare is responsible for establishing the atributes that are commom for all the health cards
    /// </summary>
    public abstract class HealthCardReader
    {
        #region Internal Properties

        /// <summary>
        /// Set of configuration for the reading process
        /// </summary>
        internal HealthCardReaderConfiguration Configuration { get; set; }

        #endregion Internal Properties

        #region Abstract Methods

        /// <summary>
        /// Returns all the information of a given JSON from the Health Card OCR Process
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public abstract HealthCardInfo ReadCardInfo(string json);

        /// <summary>
        /// Checks the elegibity of a Health Card for a given medical exam into a given hospital
        /// </summary>
        /// <param name="healthCardInfo">Health Card Info</param>
        /// <param name="hospital">Hospital</param>
        /// <param name="medicalExam">Medical Exam to Check the elegibity</param>
        /// <returns>The information regarding the health card elegibility into a given hospital and medical exam</returns>
        public abstract EligibilityInfo GetHealthCarePlanElegibility(HealthCardInfo healthCardInfo, string hospital, string medicalExam);

        /// <summary>
        /// Gets the health care plan
        /// </summary>
        /// <param name="ocr">OCR Object</param>
        /// <returns>Plan name</returns>
        public abstract string GetHealthInsurancePlan(ComputerVisionOCR ocr);

        /// <summary>
        /// Gets the insured name
        /// </summary>
        /// <param name="ocr">OCR Object</param>
        /// <returns>A name containing the insured name</returns>
        public abstract string GetInsuredName(ComputerVisionOCR ocr);
        public abstract string GetCompanyName1(ComputerVisionOCR ocr);

        /// <summary>
        /// Gets the company name from the OCR object
        /// </summary>
        /// <param name="ocr">OCR Position</param>
        /// <param name="startIndex">Start Index</param>
        /// <returns>Company's name</returns>
        public abstract string GetCompanyName(ComputerVisionOCR ocr, int startIndex);

        #endregion Abstract Methods

        #region Protected & Private Methods

        /// <summary>
        /// Get the last day in a given month
        /// </summary>
        /// <param name="year">Year</param>
        /// <param name="month">Month</param>
        /// <returns>Last day of the month</returns>
        protected int GetLastDayOfMonth(int year, int month)
        {
            var returnDay = DateTime.DaysInMonth(year, month);

            return returnDay;
        }

        /// <summary>
        /// Validates if a given string has only numbers
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="removeWhiteSpace">Remove white space</param>
        private bool HasOnlyNumbers(string value)
        {
            //Validations
            if (string.IsNullOrWhiteSpace(value)) return false;

            var numberAux = 0;

            foreach (var word in value.ToCharArray())
            {
                if (char.IsWhiteSpace(word)) continue;
                if (int.TryParse(word.ToString(), out numberAux)) continue;

                return false;
            }

            return true;
        }

        #endregion Protected & Private Methods

        #region Internal Methods

        /// <summary>
        /// Gets the health card insurance-number given
        /// </summary>
        /// <returns>The health card insurance number</returns>
        internal virtual string GetHealthCardInsuranceNumber(ComputerVisionOCR ocrData)
        {
            //Validations
            if (Configuration is null) return string.Empty;
            if (Configuration.CardInsuranceNumberLengthSequence is null) return string.Empty;
            if (Configuration.CardInsuranceNumberLengthSequence.Count == 0) return string.Empty;
            if (ocrData is null) return string.Empty;
            if (ocrData.RecognitionResult is null) return string.Empty;
            if (ocrData.RecognitionResult.Lines is null) return string.Empty;

            //Optimization -- Only lines with length higher than the sum of card number length
            var lines = ocrData.RecognitionResult.Lines
                .Where(line => line.Text.Length >= Configuration.CardInsuranceNumberLengthSequence.Sum())
                .ToList();
            var cardNumber = string.Empty;

            if (lines is null) return string.Empty;
            if (lines.Count == 0) return string.Empty;

            foreach (var line in lines)
            {
                if (HasOnlyNumbers(line.Text))
                {
                    if (line.Text.Replace(" ", "").Length == Configuration.CardInsuranceNumberLengthSequence.Sum())
                    {
                        cardNumber = line.Text.Replace(" ", "");
                        break;
                    }
                }
            }

            return cardNumber;
        }

        #endregion Internal Methods

        #region Public Methods

        /// <summary>
        /// Load the configurations for a given reader
        /// </summary>
        public void LoadConfiguration(Type derivedType)
        {
            try
            {
                Configuration = new HealthCardReaderConfiguration();

                if (derivedType.Equals(typeof(Bradesco)))
                {
                    //Getting a list of int from a list of string
                    if (ConfigurationManager.AppSettings.AllKeys.Contains("CARD_INSURANCE_NUMBER_LENGTH_SEQUENCE_BRADESCO"))
                    {
                        Configuration.CardInsuranceNumberLengthSequence = ConfigurationManager.AppSettings["CARD_INSURANCE_NUMBER_LENGTH_SEQUENCE_BRADESCO"]
                            .Split(',')
                            .Select(x => int.Parse(x))
                            .ToList();
                    }

                    //Getting the accepted plan
                    if (ConfigurationManager.AppSettings.AllKeys.Contains("CARD_INSURE_PLAN_BRADESCO"))
                    {
                        Configuration.AcceptedPlan = ConfigurationManager.AppSettings["CARD_INSURE_PLAN_BRADESCO"];
                    }

                }
                else if (derivedType.Equals(typeof(SulAmerica)))
                {
                    if (!ConfigurationManager.AppSettings.AllKeys.Contains("CARD_INSURANCE_NUMBER_LENGTH_SEQUENCE_SULAMERICA")) return;

                    //Getting a list of int from a list of string
                    Configuration.CardInsuranceNumberLengthSequence = ConfigurationManager.AppSettings["CARD_INSURANCE_NUMBER_LENGTH_SEQUENCE_SULAMERICA"]
                        .Split(',')
                        .Select(x => int.Parse(x))
                        .ToList();
                    //Getting the accepted plan
                    if (ConfigurationManager.AppSettings.AllKeys.Contains("CARD_INSURE_PLAN_SULAMERICA"))
                    {
                        Configuration.AcceptedPlan = ConfigurationManager.AppSettings["CARD_INSURE_PLAN_SULAMERICA"];
                    }
                }

            }
            catch (Exception)
            {
                Configuration = null;
            }
        }

        /// <summary>
        /// Class instance method
        /// </summary>
        public HealthCardReader()
        {
            LoadConfiguration(GetType());
        }

        #endregion Public Methods
    }
}