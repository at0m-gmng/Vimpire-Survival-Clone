namespace GameResources.Scripts.Signals
{
    using Facades;

    public class EntityKilledSignal
    {
        public EntityType EntityType { get; }

        public EntityKilledSignal(EntityType entityType) => EntityType = entityType;
    }
}
