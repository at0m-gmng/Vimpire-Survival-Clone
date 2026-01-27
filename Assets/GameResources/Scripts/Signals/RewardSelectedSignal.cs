namespace GameResources.Scripts.Signals
{
    using Data;

    public class RewardSelectedSignal
    {
        public readonly EntityType SelectedEntityType;

        public RewardSelectedSignal(EntityType selectedEntityType)
        {
            SelectedEntityType = selectedEntityType;
        }
    }
}
