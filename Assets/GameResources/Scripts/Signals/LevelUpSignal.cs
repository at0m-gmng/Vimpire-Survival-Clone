namespace GameResources.Scripts.Signals
{
    using Data.Entities;

    public sealed class LevelUpSignal
    {
        public LevelUpSignal(RewardDescription rewardDescription)
        {
            RewardDescription = rewardDescription;
        }
        public readonly RewardDescription RewardDescription;
    }
}
