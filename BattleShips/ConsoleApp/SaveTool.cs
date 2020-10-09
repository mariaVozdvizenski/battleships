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
        private const string TextFile = "savegames.txt";

        public static void SaveGameToFile(BattleShipsSave battleShipSave)
        {
            using StreamWriter file = new StreamWriter(TextFile, true);
            string jsonString = JsonSerializer.Serialize(battleShipSave);
            file.WriteLine(jsonString);
        }

        public static ICollection<BattleShipsSave> LoadGamesFromFile()
        {
            ICollection<string> jsonStrings = new List<string>();
            ICollection<BattleShipsSave> battleShipsSaves = new List<BattleShipsSave>();

            ExtractLinesFromTextFile(jsonStrings);
            DeserializeJsonFromJsonList(jsonStrings, battleShipsSaves);
            
            return battleShipsSaves;
        }

        public static void DeleteGameFromFile(string saveGameName)
        {
            ICollection<string> jsonStrings = new List<string>();
            ICollection<BattleShipsSave> battleShipsSaves = new List<BattleShipsSave>();
            
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

        private static void DeserializeJsonFromJsonList(ICollection<string> jsonStrings, ICollection<BattleShipsSave> battleShipsSaves)
        {
            if (jsonStrings.Count <= 0 || jsonStrings.First().Equals("") ) return;
            foreach (var jsonString in jsonStrings)
            {
                BattleShipsSave battleShipsSave = JsonSerializer.Deserialize<BattleShipsSave>(jsonString);
                battleShipsSaves.Add(battleShipsSave);
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
            ICollection<BattleShipsSave> battleShipsSaves = new List<BattleShipsSave>();

            ExtractLinesFromTextFile(jsonStrings);
            DeserializeJsonFromJsonList(jsonStrings, battleShipsSaves);

            return battleShipsSaves.Count > 0 && battleShipsSaves.Any(e => e.SaveName == userInput);
        }
    }
}