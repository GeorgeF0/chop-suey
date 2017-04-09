using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace ChopSuey.Config
{
    public static class ConfigReader
    {
        private static Configuration _configuration;

        public static Configuration Configuration => _configuration ?? (_configuration = ReadConfig());

        private static Configuration ReadConfig()
        {
            Console.WriteLine("Reading configuration...");

            var file = Directory.GetFiles("./", "config.json", SearchOption.AllDirectories).Single();

            var content = File.ReadAllText(file);

            return JsonConvert.DeserializeObject<Configuration>(content);
        }
    }
}
