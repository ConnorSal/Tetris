using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Sir.tech
{
    public class PlayerData
    {
        public int Coins { get; set; }
        public Dictionary<string, List<string>> OwnedSkins { get; set; } = new();

        public Dictionary<string, string> EquippedSkins { get; set; } = new();


        private static string filePath = "playerdata.json";

        public static PlayerData Load()
        {
            if (!File.Exists(filePath))
                return new PlayerData();

            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<PlayerData>(json) ?? new PlayerData();
        }

        public void Save()
        {
            string json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
        }
    }
}
