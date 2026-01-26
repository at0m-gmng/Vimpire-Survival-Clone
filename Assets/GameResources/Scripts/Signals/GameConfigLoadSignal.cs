namespace GameResources.Scripts.Signals
{
    using Configs;

    public class GameConfigLoadSignal
    {
        public GameConfigs GameConfigs { get; }

        public GameConfigLoadSignal(GameConfigs gameConfigs) => GameConfigs = gameConfigs;
    }
}