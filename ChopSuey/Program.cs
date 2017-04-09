using System;
using ChopSuey.Config;
using Microsoft.Owin.Hosting;
using Owin;

namespace ChopSuey
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (WebApp.Start(ConfigReader.Configuration.WebServerAddress, app => app.UseNancy()))
            {
                Console.WriteLine("Chop Suey started...");
                Console.ReadLine();
            }
        }

        //TODO: Add syntax Highlighting to query input
        //TODO: Parameterize start/stop/continuous
        //TODO: Add support for Text and Binary query types
        //TODO: Add filter logs
    }
}
