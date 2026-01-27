namespace GameResources.Scripts.Configs
{
    using System;
    using Data.Entities;

    [Serializable]
    public class GameConfigs
    {
        public GameConfigs()
        {
            PlayerConfig = new PlayerConfig();
            EnemiesConfig = new EnemiesConfig();
        }
        
        public PlayerConfig PlayerConfig;
        public EnemiesConfig EnemiesConfig;
    }
}