namespace GameResources.Scripts.Signals
{
    using Data;

    public sealed class GameConfigLoadSignal
    {
        public GameConfigLoadSignal(GameConfigs gameConfigs)
        {
            GameConfigs = gameConfigs;
        }
        public readonly GameConfigs GameConfigs;
    }
}