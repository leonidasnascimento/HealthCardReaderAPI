using System.Collections.Generic;

namespace API.Models
{
    /// <summary>
    /// Set of configuration required for our reading process
    /// </summary>
    public class HealthCardReaderConfiguration
    {
        /// <summary>
        /// Gets or sets the length of each set of number which compose the health card insurance number
        /// </summary>
        public List<int> CardInsuranceNumberLengthSequence { get; set; }

        /// <summary>
        /// Gets or sets the accepted plans
        /// </summary>
        public string AcceptedPlan { get; set; }

        /// <summary>
        /// Gets or sets the list of words to ignore
        /// </summary>
        public List<string> WordsToIgnore { get; set; }
    }
}