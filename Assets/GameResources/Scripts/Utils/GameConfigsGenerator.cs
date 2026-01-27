#if UNITY_EDITOR
namespace GameResources.Scripts.Utils
{
    using UnityEditor;
    using System.IO;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Data;
    using Data.Entities;
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
            JsonSerializerSettings settings = GetSerializerSettings();
            GameConfigs configs = LoadOrCreateConfigs(settings);
            
            configs = UpdateConfigsStructure(configs, settings);
            
            string json = JsonConvert.SerializeObject(configs, Formatting.Indented, settings);
            
            string folderPath = Path.Combine(Application.streamingAssetsPath, FOLDER_NAME);
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);
            
            string filePath = Path.Combine(folderPath, FILE_NAME);
            File.WriteAllText(filePath, json);
            
            AssetDatabase.Refresh();
        }

        private static JsonSerializerSettings GetSerializerSettings()
        {
            return new JsonSerializerSettings
            {
                Converters = new List<JsonConverter> { new StringEnumConverter() },
                Formatting = Formatting.Indented
            };
        }

        private static GameConfigs LoadOrCreateConfigs(JsonSerializerSettings settings)
        {
            string folderPath = Path.Combine(Application.streamingAssetsPath, FOLDER_NAME);
            string filePath = Path.Combine(folderPath, FILE_NAME);
            
            if (File.Exists(filePath))
            {
                try
                {
                    string json = File.ReadAllText(filePath);
                    return JsonConvert.DeserializeObject<GameConfigs>(json, settings) ?? new GameConfigs();
                }
                catch
                {
                    return new GameConfigs();
                }
            }
            
            return new GameConfigs();
        }

        private static GameConfigs UpdateConfigsStructure(GameConfigs configs, JsonSerializerSettings settings)
        {
            if (configs == null)
                configs = new GameConfigs();

            configs.EnemiesConfig = UpdateEnemiesConfig(configs.EnemiesConfig, settings);
            configs.CollectablesConfig = UpdateCollectablesConfig(configs.CollectablesConfig, settings);
            configs.AbilitiesConfig = UpdateAbilitiesConfig(configs.AbilitiesConfig, settings);
            configs.RewardConfig = UpdateRewardConfig(configs.RewardConfig, settings);
            
            return configs;
        }

        private static EnemiesConfig UpdateEnemiesConfig(EnemiesConfig enemiesConfig, JsonSerializerSettings settings)
        {
            if (enemiesConfig == null)
                enemiesConfig = new EnemiesConfig();
                
            if (enemiesConfig.EnemiesDescription == null)
                enemiesConfig.EnemiesDescription = new List<EnemyDescription>();
            
            List<EntityType> existingEnemyTypes = enemiesConfig.EnemiesDescription
                .Select(ed => ed.EntityType)
                .ToList();
            
            List<EntityType> allEnemyTypes = Enum.GetValues(typeof(EntityType))
                .Cast<EntityType>()
                .Where(et => et.ToString().ToLower().Contains("enemy"))
                .ToList();
            
            foreach (var enemyDesc in enemiesConfig.EnemiesDescription)
            {
                if (enemyDesc.EnemyConfig != null)
                {
                    string oldJson = JsonConvert.SerializeObject(enemyDesc.EnemyConfig, settings);
                    enemyDesc.EnemyConfig = JsonConvert.DeserializeObject<EnemyConfig>(oldJson, settings);
                }
            }
            
            foreach (EntityType enemyType in allEnemyTypes)
            {
                if (!existingEnemyTypes.Contains(enemyType))
                {
                    enemiesConfig.EnemiesDescription.Add(new EnemyDescription
                    {
                        EntityType = enemyType,
                        EnemyConfig = new EnemyConfig()
                    });
                }
            }
            
            return enemiesConfig;
        }

        private static CollectablesConfig UpdateCollectablesConfig(CollectablesConfig collectablesConfig, JsonSerializerSettings settings)
        {
            if (collectablesConfig == null)
                collectablesConfig = new CollectablesConfig();
                
            if (collectablesConfig.CollectablesDescription == null)
                collectablesConfig.CollectablesDescription = new List<CollectableDescription>();
            
            List<EntityType> existingCollectableTypes = collectablesConfig.CollectablesDescription
                .Select(cd => cd.EntityType)
                .ToList();
            
            List<EntityType> allCollectableTypes = Enum.GetValues(typeof(EntityType))
                .Cast<EntityType>()
                .Where(et => et.ToString().ToLower().Contains("collectable"))
                .ToList();
            
            foreach (var collectableDesc in collectablesConfig.CollectablesDescription)
            {
                if (collectableDesc.CollectableConfig != null)
                {
                    string oldJson = JsonConvert.SerializeObject(collectableDesc.CollectableConfig, settings);
                    collectableDesc.CollectableConfig = JsonConvert.DeserializeObject<CollectableConfig>(oldJson, settings);
                }
            }
            
            foreach (EntityType collectableType in allCollectableTypes)
            {
                if (!existingCollectableTypes.Contains(collectableType))
                {
                    collectablesConfig.CollectablesDescription.Add(new CollectableDescription
                    {
                        EntityType = collectableType,
                        CollectableConfig = new CollectableConfig()
                    });
                }
            }
            
            return collectablesConfig;
        }

        private static RewardConfig UpdateRewardConfig(RewardConfig rewardConfig, JsonSerializerSettings settings)
        {
            if (rewardConfig == null)
                rewardConfig = new RewardConfig();
                
            if (rewardConfig.RewardDescriptions == null)
                rewardConfig.RewardDescriptions = new List<RewardDescription>();

            List<EntityType> allAbilityTypes = Enum.GetValues(typeof(EntityType))
                .Cast<EntityType>()
                .Where(et => !et.ToString().ToLower().Contains("enemy") 
                          && !et.ToString().ToLower().Contains("collectable")
                          && !et.ToString().ToLower().Contains("player"))
                .ToList();

            if (allAbilityTypes.Count == 0)
            {
                return rewardConfig;
            }

            for (int level = 2; level <= 100; level++)
            {
                if (!rewardConfig.RewardDescriptions.Any(r => r.PlayerLevel == level))
                {
                    rewardConfig.RewardDescriptions.Add(new RewardDescription
                    {
                        EntityTypes = new List<EntityType>(allAbilityTypes),
                        PlayerLevel = level
                    });
                }
            }

            rewardConfig.RewardDescriptions = rewardConfig.RewardDescriptions
                .OrderBy(r => r.PlayerLevel)
                .ToList();
            
            return rewardConfig;
        }

        private static AbilitiesConfig UpdateAbilitiesConfig(AbilitiesConfig abilitiesConfig, JsonSerializerSettings settings)
        {
            if (abilitiesConfig == null)
                abilitiesConfig = new AbilitiesConfig();
                
            if (abilitiesConfig.AbilitiesDescription == null)
                abilitiesConfig.AbilitiesDescription = new List<AbilityDescription>();
            
            List<EntityType> existingAbilityTypes = abilitiesConfig.AbilitiesDescription
                .Select(ad => ad.EntityType)
                .ToList();
            
            List<EntityType> allAbilityTypes = Enum.GetValues(typeof(EntityType))
                .Cast<EntityType>()
                .Where(et => !et.ToString().ToLower().Contains("enemy") 
                          && !et.ToString().ToLower().Contains("collectable")
                          && !et.ToString().ToLower().Contains("player"))
                .ToList();
            
            foreach (var abilityDesc in abilitiesConfig.AbilitiesDescription)
            {
                if (abilityDesc.AbilityConfig != null)
                {
                    string oldJson = JsonConvert.SerializeObject(abilityDesc.AbilityConfig, settings);
                    abilityDesc.AbilityConfig = JsonConvert.DeserializeObject<AbilityConfig>(oldJson, settings);
                }
            }
            
            foreach (EntityType abilityType in allAbilityTypes)
            {
                if (!existingAbilityTypes.Contains(abilityType))
                {
                    abilitiesConfig.AbilitiesDescription.Add(new AbilityDescription
                    {
                        EntityType = abilityType,
                        AbilityConfig = new AbilityConfig()
                    });
                }
            }
            
            return abilitiesConfig;
        }
    }
}
#endif