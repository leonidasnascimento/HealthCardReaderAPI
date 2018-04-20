using API.Models;
using System.Diagnostics;

namespace API.Patterns
{
    /// <summary>
    /// 
    /// </summary>
    public class HealthCardStrategy
    {
        public HealthCardInfo GetHealthCardInstance(string operadora)
        {
            if (string.IsNullOrWhiteSpace(operadora))
            {
                Debug.WriteLine("operadora is null!");
                return null;
            }

            if ("bradesco".Equals(operadora.ToLowerInvariant()))
            {
                Debug.WriteLine("É Bradesco!");
                return new Bradesco();
            }
            else
            {
                Debug.WriteLine("Não é Bradesco");

                return null;
            }
        }               
    }    
}