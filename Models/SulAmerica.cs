using System;

namespace API.Models
{
    public class SulAmerica : HealthCardReader
    {
        public override EligibilityInfo GetHealthCarePlanElegibility(HealthCardInfo healthCardInfo, string hospital, string medicalExam)
        {
            return null;
        }

        public override HealthCardInfo ReadCardInfo(string json)
        {
            throw new NotImplementedException();
        }
    }
}