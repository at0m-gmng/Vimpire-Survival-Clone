namespace GameResources.Scripts.Signals
{
    using Data;

    public sealed class RewardSelectedSignal
    {
        public RewardSelectedSignal(EntityType selectedEntityType)
        {
            SelectedEntityType = selectedEntityType;
        }
        public readonly EntityType SelectedEntityType;
    }
}
