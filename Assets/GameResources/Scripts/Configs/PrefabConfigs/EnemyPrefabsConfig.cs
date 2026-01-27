namespace GameResources.Scripts.Configs.PrefabConfigs
{
    using System;
    using System.Collections.Generic;
    using Data;
    using Facades;
    using UnityEngine;

    [CreateAssetMenu(fileName = nameof(EnemyPrefabsConfig), menuName = "Configs/PrefabConfigs/EnemyPrefabsConfig")]
    public class EnemyPrefabsConfig : ScriptableObject
    {
        [field: SerializeField] public List<EnemyPrefab> EnemyPrefabs { get; private set; } = new();
    }

    [Serializable]
    public class EnemyPrefab
    {
        public EntityType EntityType;
        public EnemyFacade EnemyFacade;
    }
}