using System;
using System.IO;

namespace HoloBrawl.Core
{
    public static class Data
    {
        private static readonly string DataPath 
            = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\HoloBrawl\";
        private static readonly string OptionsFile = DataPath + "options.txt";

        public static readonly float Gravity = -9.8f;

        public static int ScreenWidth { get; private set; } = 1280;
        public static int ScreenHeight { get; private set; } = 720;
        public static bool Fullscreen { get; private set; }


        private static void CheckFolders()
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
        }
        private static void CheckOptions(string arg, string option)
        {
            switch (arg)
            {
                case "ScreenWidth":
                    ScreenWidth = int.Parse(option);
                    break;
                case "ScreenHeight":
                    ScreenHeight = int.Parse(option);
                    break;
                case "Fullscreen":
                    if (bool.TryParse(option, out var temp))
                        Fullscreen = temp;
                    else
                        Console.WriteLine("[WARNING] Could not parse fullscreen option, using default");
                    break;
                default:
                    Console.WriteLine("[WARNING] Unknown option: " + arg);
                    break;
            }
        }
        public static void LoadData()
        {
            CheckFolders();
            var options = File.ReadLines(OptionsFile);
            foreach (var option in options)
            {
                Console.WriteLine("[INFO] Loading option: " + option);
                var split = option.Split(':');
                if (split.Length < 2)
                    continue;
                CheckOptions(split[0], split[1]);
                
            }
        }
    }
}