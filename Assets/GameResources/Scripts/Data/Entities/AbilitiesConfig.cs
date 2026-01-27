namespace GameResources.Scripts.Data.Entities
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class AbilitiesConfig
    {
        public List<AbilityDescription> AbilitiesDescription = new List<AbilityDescription>();
    }

    [Serializable]
    public class AbilityDescription
    {
        public EntityType EntityType = EntityType.RangeAttackAbility;
        public AbilityConfig AbilityConfig = new();
    }

    [Serializable]
    public class AbilityConfig : EntityConfig
    {
        public float Damage = 5f;
        public float Cooldown = 1f;
        public float Radius = 3f;
        public float Speed = 0f;
        public int EntitiesCount = 0;
    }
}