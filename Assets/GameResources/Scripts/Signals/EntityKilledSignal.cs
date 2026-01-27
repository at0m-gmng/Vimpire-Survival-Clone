namespace GameResources.Scripts.Signals
{
    using Data;
    using Facades;

    public class EntityKilledSignal
    {
        public EntityType EntityType { get; }

        public EntityKilledSignal(EntityType entityType) => EntityType = entityType;
    }
}
