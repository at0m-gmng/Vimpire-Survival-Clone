namespace GameResources.Scripts.Data.Entities
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class EnemiesConfig
    {
        public List<EnemyDescription> EnemiesDescription = new List<EnemyDescription>();
    }

    [Serializable]
    public class EnemyDescription
    {
        public EntityType EntityType;
        public EnemyConfig EnemyConfig;
    }

    [Serializable]
    public class EnemyConfig : EntityConfig
    {
        public EntityType ExperienceType = EntityType.CollectableSmallExperience;
        public float MoveSpeed = 4f;
        public float Health = 10;
    }
}