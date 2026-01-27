namespace GameResources.Scripts.Data.Entities
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class RewardConfig
    {
        public List<RewardDescription> RewardDescriptions = new List<RewardDescription>();
    }

    [Serializable]
    public class RewardDescription
    {
        public List<EntityType> EntityTypes = new List<EntityType>();
        public int PlayerLevel;
    }
}
