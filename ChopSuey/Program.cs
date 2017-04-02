using System;
using Microsoft.Owin.Hosting;
using Owin;

namespace ChopSuey
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //const string init = "() => new List<string>()";
            //const string aggregate = @"(e, d, s) => {if (e.Type != ""secret"") return; s.Add((string)d.Text);}";

            //var query = new AggregateQuery(@"C:\Users\george\Downloads\temp\Streak Sample", init, aggregate);

            //query.Run().Wait();

            using (WebApp.Start("http://+:9999", app => app.UseNancy()))
            {
                Console.ReadLine();
            }
        }

        //TODO: Show aggregate query on UI (add pretty printing JSON)
        // > Add panel to house the query widgets
        // > Create widget template with header (description, hits, errors, etc) and pretty print state
        // > Poll every 1 second and gather all running aggregate queries
        // > Show the queries in the panel by binding to template
        // > Add "i" (info) icon with Init and Aggregate strings as tooltips
        //TODO: Add support for Text and Binary query types
        //TODO: Parameterize start/stop/continuous
        //TODO: Add persistence
        //TODO: Add imports/assemblies? (make sure init can new-up Expandos)
        // > Expando
        // > RegExp
        // > LINQ
        // > MoreLinq
        //TODO: Add filter logs
    }
}
