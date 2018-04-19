using API.Models;
using System.Diagnostics;

namespace API.Patterns
{
    /// <summary>
    /// 
    /// </summary>
    public class HealthCardStrategy
    {
        public HealthCard GetHealthCardInstance(string operadora)
        {
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

        public string operadora;

        public string Operadora
        {
            get { return operadora; }
            set { operadora = value; }
        }
    }


}