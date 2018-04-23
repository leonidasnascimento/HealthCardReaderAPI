using System;

namespace API.Models
{
    public class Bradesco : HealthCardReader
    {

        public override EligibilityInfo GetHealthCarePlanElegibility(HealthCardInfo healthCardInfo, string hospital, string medicalExam)
        {
            return new EligibilityInfo();
        }

        
        public override HealthCardInfo ReadCardInfo(string json)
        {
            //if (!(json is null))
            //    return Newtonsoft.Json.JsonConvert.DeserializeObject();

            return new HealthCardInfo
            {
                Name = "Giovanna",
                CardNumber = "774 247 009695 001",
                //ValidDate = "", 
                HealthInsurance = "Saude Top Enfermaria Nacional Flex",
                //Logo = "",
                Company = "Bradesco Saúde"

                               
        };
        }
    }
}