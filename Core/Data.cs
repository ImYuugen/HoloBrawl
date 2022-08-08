using System;
using System.IO;

namespace HoloBrawl.Core
{
    public static class Data
    {
        private static readonly string DataPath 
            = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\HoloBrawl\";
        private static readonly string OptionsFile = DataPath + "options.txt";

        public static int ScreenWidth { get; private set; } = 1280;
        public static int ScreenHeight { get; private set; } = 720;
        public static bool Fullscreen { get; private set; } = false;

        public static void LoadData()
        {
            if (!Directory.Exists(DataPath))
            {
                Console.WriteLine("Creating data directory");
                Directory.CreateDirectory(DataPath);
            }
            if (!File.Exists(OptionsFile))
            {
                Console.WriteLine("Creating options file");
                File.Create(OptionsFile);
            }
            
            var options = File.ReadLines(OptionsFile);
            foreach (var option in options)
            {
                Console.WriteLine("[INFO] Loading option: " + option);
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
                    case "Fullscreen":
                        if (bool.TryParse(split[1], out var temp))
                            Fullscreen = temp;
                        else
                            Console.WriteLine("[WARNING] Could not parse fullscreen option, using default");
                        break;
                    default:
                        Console.WriteLine("[WARNING] Unknown option: " + split[0]);
                        break;
                }
            }
        }
    }
}