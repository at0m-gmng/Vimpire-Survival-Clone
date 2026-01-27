namespace GameResources.Scripts.Data.Entities
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class CollectablesConfig
    {
        public List<CollectableDescription> CollectablesDescription = new List<CollectableDescription>();
    }

    [Serializable]
    public class CollectableDescription
    {
        public EntityType EntityType;
        public CollectableConfig CollectableConfig;
    }

    [Serializable]
    public class CollectableConfig : EntityConfig
    {
        public float Experience = 10f;
        public float CollectSpeed = 20f;
    }
}