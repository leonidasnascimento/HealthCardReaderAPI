namespace API.Models
{
    /// <summary>
    /// the class HealthCare is responsible for establishing the atributes that are commom for all the health cards
    /// </summary>
    public abstract class HealthCardReader
    {
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
    }
}