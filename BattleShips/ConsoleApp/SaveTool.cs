using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Domain;

namespace ConsoleApp
{
    public class SaveTool
    {
        private const string TextFile = 
            "savegames.txt";

        public static void SaveGameToFile(BattleShipsSaveJson battleShipSaveJson)
        {
            using StreamWriter file = new StreamWriter(TextFile, true);
            string jsonString = JsonSerializer.Serialize(battleShipSaveJson);
            file.WriteLine(jsonString);
        }

        public static ICollection<BattleShipsSaveJson> LoadGamesFromFile()
        {
            ICollection<string> jsonStrings = new List<string>();
            ICollection<BattleShipsSaveJson> battleShipsSaves = new List<BattleShipsSaveJson>();

            ExtractLinesFromTextFile(jsonStrings);
            DeserializeJsonFromJsonList(jsonStrings, battleShipsSaves);
            
            return battleShipsSaves;
        }

        public static void DeleteGameFromFile(string saveGameName)
        {
            ICollection<string> jsonStrings = new List<string>();
            ICollection<BattleShipsSaveJson> battleShipsSaves = new List<BattleShipsSaveJson>();
            
            ExtractLinesFromTextFile(jsonStrings);
            DeserializeJsonFromJsonList(jsonStrings, battleShipsSaves);

            var save = battleShipsSaves.FirstOrDefault(e => e.SaveName == saveGameName);
            battleShipsSaves.Remove(save!);
            
            File.WriteAllText(TextFile, string.Empty);

            foreach (var saveGame in battleShipsSaves)
            {
                SaveGameToFile(saveGame);
            }
        }

        private static void DeserializeJsonFromJsonList(ICollection<string> jsonStrings, ICollection<BattleShipsSaveJson> battleShipsSaves)
        {
            if (jsonStrings.Count <= 0 || jsonStrings.First().Equals("") ) return;
            foreach (var jsonString in jsonStrings)
            {
                BattleShipsSaveJson battleShipsSaveJson = JsonSerializer.Deserialize<BattleShipsSaveJson>(jsonString);
                battleShipsSaves.Add(battleShipsSaveJson);
            }
        }

        private static void ExtractLinesFromTextFile(ICollection<string> jsonStrings)
        {
            if (!File.Exists(TextFile)) return;
            using StreamReader file = new StreamReader(TextFile);
            string? line;
            while ((line = file.ReadLine()) != null)
            {
                jsonStrings.Add(line);
            }
        }

        public static bool SaveGameExists(string userInput)
        {
            ICollection<string> jsonStrings = new List<string>();
            ICollection<BattleShipsSaveJson> battleShipsSaves = new List<BattleShipsSaveJson>();

            ExtractLinesFromTextFile(jsonStrings);
            DeserializeJsonFromJsonList(jsonStrings, battleShipsSaves);

            return battleShipsSaves.Count > 0 && battleShipsSaves.Any(e => e.SaveName == userInput);
        }
    }
}