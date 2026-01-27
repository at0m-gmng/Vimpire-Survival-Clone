namespace GameResources.Scripts.Signals
{
    using Data.Entities;

    public class LevelUpSignal
    {
        public readonly RewardDescription RewardDescription;

        public LevelUpSignal(RewardDescription rewardDescription)
        {
            RewardDescription = rewardDescription;
        }
    }
}
