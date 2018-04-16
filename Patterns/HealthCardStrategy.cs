namespace API.Patterns
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class HealthCardStrategy : Models.HealthCard
    {
        public HealthCardStrategy(string ModelCard)
        {
#pragma warning disable CS0184 // 'is' expression's given expression is never of the provided type
            if (ModelCard is Models.Bradesco)
#pragma warning restore CS0184 // 'is' expression's given expression is never of the provided type
            {
                System.Console.WriteLine("É Bradesco!");
            }
            else
            { 
                System.Console.WriteLine("Não é Bradesco");
            }
        }

        //public override Models.HealthCard ReadCardInfo(string json)
        //{
        //    if(json is null)
        //    {
        //        System.Console.WriteLine("Json é nulo");
        //    }
        //}

        private Models.HealthCard _healthCard { get; set; }

    }
 
}