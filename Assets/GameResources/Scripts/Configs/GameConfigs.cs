namespace GameResources.Scripts.Configs
{
    using Entities;

    public class GameConfigs
    {
        public GameConfigs()
        {
            PlayerConfig = new PlayerConfig();
            EnemyConfig = new EnemyConfig();
        }
        
        public PlayerConfig PlayerConfig;
        public EnemyConfig EnemyConfig;
    }
}