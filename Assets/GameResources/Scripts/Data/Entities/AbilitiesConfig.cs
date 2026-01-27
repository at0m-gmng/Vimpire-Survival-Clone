namespace GameResources.Scripts.Data.Entities
{
    using System;
    using System.Collections.Generic;
    using UnityEngine.Serialization;

    [Serializable]
    public class AbilitiesConfig
    {
        public List<AbilityDescription> AbilitiesDescription = new List<AbilityDescription>();
    }

    [Serializable]
    public class AbilityDescription
    {
        public EntityType EntityType;
        public AbilityConfig AbilityConfig;
    }

    [Serializable]
    public class AbilityConfig : EntityConfig
    {
        public float Damage = 1f;
        public float Cooldown = 1f;
        public float Radius = 3f;
    }
}