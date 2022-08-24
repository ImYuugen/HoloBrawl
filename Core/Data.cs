using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HoloBrawl.Entities;
using HoloBrawl.Input.Player;
using HoloBrawl.Terrain;
using Newtonsoft.Json;

namespace HoloBrawl.Core
{
    public static class Data
    {
        private static readonly string DataPath
            = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\HoloBrawl\";

        public static readonly string ContentPath =
            @"..\..\..\Content\";

        private static readonly string OptionsFile = DataPath + "options.txt";
        private static readonly string CharDataFile = DataPath + "characters.json";
        private static readonly string ProfilesDataPath = DataPath + @"Profiles\";
        private static readonly string MapDataPath = ContentPath + "Maps\\";


        public static int ScreenWidth { get; private set; } = 1280;
        public static int ScreenHeight { get; private set; } = 720;
        public static bool Fullscreen { get; private set; }

        public static Dictionary<string, Character> Characters { get; set; } = new();
        public static Terrain.Terrain LoadedTerrain { get; private set; }

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
            var matching = chars.FirstOrDefault(character => character.Name == name); // The character with the searched name, the first in the file is selected.
            if (matching == null)
            {
                Console.WriteLine("[WARNING] Could not find character: " + name);
                return;
            }
            if (Characters.TryAdd(matching.Name, matching))
            {
                Console.WriteLine($"[INFO] Loaded character: {matching.Name}");
                return;
            }
            Console.WriteLine("[WARNING] Could not add character: " + matching.Name);
        }

        #endregion

        #region MapData
        
        private static void CheckFolderMap(string name, out bool found)
        {
            if (!Directory.Exists(ContentPath))
            {
                Console.WriteLine("[INFO] Creating map directory");
                Directory.CreateDirectory(ContentPath);
            }
            found = File.Exists(MapDataPath + name + ".json");
        }

        public static void LoadAndSetMap(string mapName)
        {
            CheckFolderMap(mapName, out var found);
            Terrain.Terrain map;
            if (found)
                map = JsonConvert.DeserializeObject<Terrain.Terrain>(File.ReadAllText(MapDataPath + mapName + ".json"));
            else
            {
                Console.WriteLine("[WARNING] Could not find map: " + mapName);
                map = new Terrain.Terrain("null", Array.Empty<Floor>());
            }
            LoadedTerrain = map;
        }

        #endregion

        #region ControlProfiles

        private static void CheckFolderProfiles(string name, out bool found)
        {
            if (!Directory.Exists(ProfilesDataPath))
            {
                Console.WriteLine("[INFO] Creating profiles directory");
                Directory.CreateDirectory(ProfilesDataPath);
            }
            found = File.Exists(ProfilesDataPath + name + ".json");
        }
        
        public static Controller LoadProfile(string profileName)
        {
            CheckFolderProfiles(profileName, out var found);
            if (found)
                return JsonConvert.DeserializeObject<Controller>(
                    File.ReadAllText(ProfilesDataPath + profileName + ".json"));
            
            Console.WriteLine("[WARNING] Could not find profile: " + profileName);
            return new Controller();
        }

        #endregion
    }
}