namespace GameResources.Scripts.Configs.PrefabConfigs
{
    using System;
    using System.Collections.Generic;
    using Data;
    using Facades;
    using UnityEngine;

    [CreateAssetMenu(fileName = nameof(CollectablePrefabsConfig), menuName = "Configs/PrefabConfigs/CollectablesPrefabConfig")]
    public class CollectablePrefabsConfig : ScriptableObject
    {
        [field: SerializeField] public List<CollectablePrefab> CollectablePrefabs { get; private set; } = new();
    }
    
    [Serializable]
    public class CollectablePrefab
    {
        public EntityType EntityType;
        public CollectablesFacade CollectableFacade;
    }
}