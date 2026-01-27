namespace GameResources.Scripts.Data
{
    using System;
    using Entities;

    [Serializable]
    public class GameConfigs
    {
        public GameConfigs()
        {
            PlayerConfig = new PlayerConfig();
            EnemiesConfig = new EnemiesConfig();
            CollectablesConfig = new CollectablesConfig();
            AbilitiesConfig = new AbilitiesConfig();
            RewardConfig = new RewardConfig();
        }
        
        public PlayerConfig PlayerConfig;
        public EnemiesConfig EnemiesConfig;
        public CollectablesConfig CollectablesConfig;
        public AbilitiesConfig AbilitiesConfig;
        public RewardConfig RewardConfig;
    }
}