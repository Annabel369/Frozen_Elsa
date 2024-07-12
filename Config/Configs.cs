using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.VisualBasic;

namespace Frozen_music.Config
{
    public static class Configs
    {
        public static class Shared {
            public static string? CookiesFolderPath { get; set; }
        }

        

        private static readonly string ConfigDirectoryName = "config";
        private static readonly string ConfigFileName = "config.json";
        private static readonly string jsonFilePath = "Kill_Settings.json";
        private static string? _configFilePath;
        private static string? _jsonFilePath;
        private static ConfigData? _configData;



    //[JsonPropertyName("gif_display_duration")]
    //public float GifDisplayDuration { get; set; } = 4.0f;

        

        private static readonly JsonSerializerOptions SerializationOptions = new()
        {
            Converters =
            {
                new JsonStringEnumConverter()
            },
            WriteIndented = true,
            AllowTrailingCommas = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
        };

        public static bool IsLoaded()
        {
            return _configData is not null;
        }

        public static ConfigData GetConfigData()
        {
            if (_configData is null)
            {
                throw new Exception("Config not yet loaded.");
            }
            
            return _configData;
        }

        

        public static ConfigData Load(string modulePath)
        {
            var configFileDirectory = Path.Combine(modulePath, ConfigDirectoryName);
            if(!Directory.Exists(configFileDirectory))
            {
                Directory.CreateDirectory(configFileDirectory);
            }
            _jsonFilePath = Path.Combine(configFileDirectory, jsonFilePath);

            _configFilePath = Path.Combine(configFileDirectory, ConfigFileName);
            if (File.Exists(_configFilePath))
            {
                _configData = JsonSerializer.Deserialize<ConfigData>(File.ReadAllText(_configFilePath), SerializationOptions);
            }
            else
            {
                _configData = new ConfigData();
            }

            if (_configData is null)
            {
                throw new Exception("Failed to load configs.");
            }

            SaveConfigData(_configData);
            
            return _configData;
        }

        private static void SaveConfigData(ConfigData configData)
        {
            if (_configFilePath is null)
            {
                throw new Exception("Config not yet loaded.");
            }
            string json = JsonSerializer.Serialize(configData, SerializationOptions);

            json = "// Note: To Use Modify Version And Lower Volume \n// Download https://github.com/Source2ZE/MultiAddonManager  With Frozen Elsa WorkShop \n// https://steamcommunity.com/sharedfiles/filedetails/?id=3241525034\n// mm_extra_addons 3230015783\n// OtherWise Use Normal Sounds https://github.com/astral3693/Frozen_Elsa/tree/main/csgo_addons/frozen_elsa/soundevents.txt \n\n" + json;

            File.WriteAllText(_configFilePath, json);
        }

        public class ConfigData
        {
            public bool KS_EnableQuakeSounds { get; set; }
            
            public string empty { get; set; }
            public string explode { get; set; }
            public string freeze_hit { get; set; }
            public string frozengo { get; set; }
            public string frozenice { get; set; }
            public string punishment1 { get; set; }
            public string unfreeze { get; set; }

            public string amarillo { get; set; }

            public string empty2 { get; set; }


            public string Information_For_You_Dont_Delete_it { get; set; }

            [JsonPropertyName("FreezeTime")]
            public int FreezeTime { get; set; } = 5;

            public ConfigData()
            {
                KS_EnableQuakeSounds = true;
                empty = "-----------------------------------------------------------------------------------"; 
                explode = "sounds/frozen_music2/explode.vsnd_c";
                freeze_hit = "sounds/frozen_music2/freeze_hit.vsnd_c";
                frozengo = "sounds/frozen_music2/frozen-go.vsnd_c";
                frozenice = "sounds/frozen_music2/frozen-ice.vsnd_c";
                punishment1 = "sounds/frozen_music2/punishment1.vsnd_c";
                unfreeze = "sounds/frozen_music2/unfreeze.vsnd_c";
                amarillo = "sounds/marius_music/amarillo.vsnd_c";
                empty2 = "-----------------------------------------------------------------------------------";
                Information_For_You_Dont_Delete_it = " Vist  [https://github.com/astral3693/Frozen_Elsa] To Understand All Above";
            }
        }
    }
}
