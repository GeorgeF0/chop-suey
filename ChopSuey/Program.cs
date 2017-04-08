using System;
using Microsoft.Owin.Hosting;
using Owin;

namespace ChopSuey
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (WebApp.Start("http://+:9999", app => app.UseNancy()))
            {
                Console.WriteLine("Chop Suey started...");
                Console.ReadLine();
            }
        }

        //TODO: Show aggregate query on UI
        // > Add "i" (info) icon with Init and Aggregate strings as tooltips
        //TODO: Add support for Text and Binary query types
        //TODO: Add syntax Highlighting to query input
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
