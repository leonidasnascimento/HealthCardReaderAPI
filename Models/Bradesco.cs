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
            var teste = (ComputerVisionOCR) Newtonsoft.Json.JsonConvert.DeserializeObject(json, typeof(ComputerVisionOCR));
            
        
            return new HealthCardInfo
            {
                Name = teste.regions[0].lines[1].words[0].text + " " +
                       teste.regions[0].lines[1].words[1].text + " " +
                       teste.regions[1].lines[3].words[0].text,
                CardNumber = teste.regions[0].lines[2].words[0].text + " " +
                             teste.regions[1].lines[4].words[0].text + " " +
                             teste.regions[1].lines[4].words[1].text,
                //ValidDate
                //HealthInsurance
                //Logo
                Company = teste.regions[1].lines[0].words[1].text + " " +
                          teste.regions[1].lines[1].words[0].text,


                    //SUBSTRING
                //Name = json.Substring(273, 8) + " " +
                //       json.Substring(321, 1) + " " +
                //       json.Substring(880, 4),
                //CardNumber =
                //    json.Substring(404, 3) + " " +
                //    json.Substring(448, 3) + " " +
                //    json.Substring(969, 6) + " " +
                //    json.Substring(1017, 3),

                ////ValidDate = json.Substring(),
                ////HealthInsurance = "Saude Top Enfermaria Nacional Flex",
                ////Logo = "",
                //Company =
                //    json.Substring(616, 8) + " " +
                //    json.Substring(704, 5),



            };
        }
    }
}