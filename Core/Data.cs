using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HoloBrawl.Entities;
using Newtonsoft.Json;

namespace HoloBrawl.Core
{
    public static class Data
    {
        private static readonly string DataPath
            = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\HoloBrawl\";

        private static readonly string OptionsFile = DataPath + "options.txt";
        private static readonly string CharDataFile = DataPath + "characters.json";


        public static int ScreenWidth { get; private set; } = 1280;
        public static int ScreenHeight { get; private set; } = 720;
        public static bool Fullscreen { get; private set; }

        public static Dictionary<string, Character> Characters { get; set; } = new();

        #region Options

        private static void CheckFoldersOption()
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
            CheckFoldersOption();
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

        #endregion

        #region CharData

        private static void CheckFolderChar()
        {
            if (!Directory.Exists(DataPath))
            {
                Console.WriteLine("[INFO] Creating data directory");
                Directory.CreateDirectory(DataPath);
            }

            if (!File.Exists(CharDataFile))
            {
                Console.WriteLine("[INFO] Creating character data file");
                File.Create(CharDataFile);
            }
        }

        public static void LoadCharacter(string name)
        {
            CheckFolderChar();
            var chars = JsonConvert.DeserializeObject<Character[]>(File.ReadAllText(CharDataFile))
                        ?? Array.Empty<Character>();
            foreach (var character in chars.Where(character => character.Name == name))
            {
                if (Characters.TryAdd(character.Name, character))
                {
                    Console.WriteLine($"[INFO] Loaded character: {character.Name}");
                    continue;
                }
                
                Console.WriteLine("[WARNING] Could not add character: " + character.Name);
                return;
            }
        }

        #endregion
    }
}