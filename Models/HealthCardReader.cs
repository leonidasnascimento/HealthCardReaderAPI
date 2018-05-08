using System;
using System.Collections.Generic;
using System.Globalization;

namespace API.Models
{
    /// <summary>
    /// the class HealthCare is responsible for establishing the atributes that are commom for all the health cards
    /// </summary>
    public abstract class HealthCardReader
    {
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

        public abstract string GetHealthInsuranceNumber(List<int> logicNumericSequence);

        #endregion Abstract Methods

        #region Internal Methods

        /// <summary>
        /// Get the 'Valid Date' field value given a string
        /// </summary>
        /// <param name="healthInsuranceDate">Date</param>
        /// <returns>Valid Date</returns>
        protected DateTime? GetValidDate(string healthInsuranceDate)
        {
            //Validations
            if (string.IsNullOrWhiteSpace(healthInsuranceDate)) return default(DateTime);
            if (healthInsuranceDate.Split('/').Length <= 1) return default(DateTime);

            DateTime? returnDate = null;

            if (healthInsuranceDate.Split('/').Length == 2)
                returnDate = new DateTime(int.Parse(string.Concat("20", healthInsuranceDate.Split('/')[1])), int.Parse(healthInsuranceDate.Split('/')[0]), GetLastDayOfMonth(int.Parse(healthInsuranceDate.Split('/')[1]), int.Parse(healthInsuranceDate.Split('/')[0])));

            if(healthInsuranceDate.Split('/').Length == 3)
                returnDate = new DateTime(int.Parse(string.Concat("20", healthInsuranceDate.Split('/')[2])), int.Parse(healthInsuranceDate.Split('/')[1]), int.Parse(healthInsuranceDate.Split('/')[0]));

            return returnDate;
        }

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

        #endregion Internal Methods
    }
}