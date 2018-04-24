using System;
using System.Collections.Generic;
using System.Drawing;

namespace API.Models
{
    public class HealthCardInfo
    {
        /// <summary>
        /// Name represents the healthcard's owner
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// CardNumber representes the number of the card
        /// </summary>
        public string CardNumber { get; set; }

        /// <summary>
        /// ValidDate represents the last date the card will be valid
        /// </summary>

        public DateTime ValidDate { get; set; }
        
        /// <summary>
        /// HealthInsurance represents the type of the Health Insurance
        /// </summary>
        public string HealthInsurance { get; set; }

        /// <summary>
        /// Logo represents which is the medical agreement of the card
        /// </summary>
        public Image Logo { get; set; }
        /// <summary>
        /// Company represents the company in which the owner of the card works
        /// </summary>
        public string Company { get; set; }

        /// <summary>
        /// List of Eligibities
        /// </summary>
        public List<EligibilityInfo> Eligibilities { get; set; }

        #region Methods
            
        /// <summary>
        /// Adds instance of Eligibility to the list
        /// </summary>
        /// <param name="eligibility">Eligibility</param>
        public void AddEligibility(EligibilityInfo eligibility)
        {
            if (Eligibilities is null)
                Eligibilities = new List<EligibilityInfo>();

            Eligibilities.Add(eligibility);
        }

        #endregion Methods
    }
}
