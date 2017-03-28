using System;

namespace ChopSuey
{
    public class Program
    {
        public static void Main(string[] args)
        {
            const string init = "() => new List<string>()";
            const string aggregate = @"(e, d, s) => {if (e.Type != ""secret"") return; s.Add((string)d.Text);}";

            var query = new AggregateQuery(@"C:\Users\george\Downloads\temp\Streak Sample", init, aggregate);

            query.Run().Wait();

            Console.ReadLine();
        }

        //TODO: Show aggregate query on UI (add pretty printing JSON)
        //TODO: Add queries from UI
        //TODO: Poll to update queries
        //TODO: Parameterize start/stop/continuous
        //TODO: Add persistence
        //TODO: Add imports/assemblies? (make sure init can new-up Expandos)
    }
}
