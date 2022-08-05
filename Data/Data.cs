using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace HoloBrawl.Data
{
    public static class Data
    {
        private static readonly string _dataPath 
            = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\HoloBrawl\";
        private static readonly string _optionsFile = _dataPath + "options.xml";

        public static int ScreenWidth = 1280;
        public static int ScreenHeight = 720;

        public static void Load()
        {
            if (!Directory.Exists(_dataPath))
            {
                Console.WriteLine("Creating data directory");
                Directory.CreateDirectory(_dataPath);
            }
            if (!File.Exists(_optionsFile))
            {
                Console.WriteLine("Creating options file");
                File.Create(_optionsFile);
            }
            var options = File.ReadLines(_optionsFile);
            foreach (var option in options)
            {
                Console.WriteLine("Loading option: " + option);
                var split = option.Split(':');
                if (split.Length < 2)
                    continue;
                switch (split[0])
                {
                    case "ScreenWidth":
                        ScreenWidth = int.Parse(split[1]);
                        break;
                    case "ScreenHeight":
                        ScreenHeight = int.Parse(split[1]);
                        break;
                    default:
                        Console.WriteLine("Unknown option: " + split[0]);
                        break;
                }
            }
        }
    }
}