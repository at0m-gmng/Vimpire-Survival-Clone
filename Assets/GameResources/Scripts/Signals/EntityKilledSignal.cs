namespace GameResources.Scripts.Signals
{
    using Data;
    using UnityEngine;

    public sealed class EntityKilledSignal
    {
        public EntityKilledSignal(EntityType entityType, EntityType rewardType, Vector3 position)
        {
            EntityType = entityType;
            RewardType = rewardType;
            Position = position;
        }

        public readonly EntityType EntityType;
        public readonly EntityType RewardType;
        public readonly Vector3 Position;
    }
}
