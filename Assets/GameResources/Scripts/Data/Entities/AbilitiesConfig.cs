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
        public EntityType EntityType;
        public AbilityConfig AbilityConfig;
    }

    [Serializable]
    public class AbilityConfig : EntityConfig
    {
        public float BaseDamage = 1f;
        public float BaseCooldown = 1f;
    
        public float BaseRadius = 3f;
        public int BaseProjectileCount = 0;
        public int BaseOrbitalCount = 0;
        public float BaseRotationSpeed = 10;
    
        public float DamagePerLevel;
        public float CooldownReductionPerLevel;
        public float RadiusIncreasePerLevel;
        public int ProjectileCountPerLevel;
        public int OrbitalCountPerLevel;
        public float RotationSpeedPerLevel;
    }
}