#if UNITY_EDITOR
namespace GameResources.Scripts.Utils
{
    using UnityEditor;
    using System.IO;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Configs;
    using Data;
    using Data.Entities;
    using Facades;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using UnityEngine;

    public static class GameConfigsGenerator
    {
        private const string MENU_PATH = "Services/[Utils]Generate Game Config";
        private const string FOLDER_NAME = "Configs";
        private const string FILE_NAME = "GameConfigs.json";
    
        [MenuItem(MENU_PATH)]
        public static void Generate()
        {
            GameConfigs configs = new GameConfigs();
        
            List<EntityType> enemyTypes = Enum.GetValues(typeof(EntityType))
                .Cast<EntityType>()
                .Where(et => et.ToString().ToLower().Contains("enemy"))
                .ToList();
        
            foreach (EntityType enemyType in enemyTypes)
            {
                configs.EnemiesConfig.EnemiesDescription.Add(new EnemyDescription
                {
                    EntityType = enemyType,
                    EnemyConfig = new EnemyConfig()
                });
            }
            
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                Converters = new List<JsonConverter> { new StringEnumConverter() },
                Formatting = Formatting.Indented
            };
        
            string json = JsonConvert.SerializeObject(configs, Formatting.Indented, settings);
            
            string folderPath = Path.Combine(Application.streamingAssetsPath, FOLDER_NAME);
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);
            
            string filePath = Path.Combine(folderPath, FILE_NAME);
            File.WriteAllText(filePath, json);
            
            AssetDatabase.Refresh();
        }
    }
}
#endif