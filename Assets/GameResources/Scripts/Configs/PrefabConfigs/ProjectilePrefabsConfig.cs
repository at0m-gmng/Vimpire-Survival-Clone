namespace GameResources.Scripts.Configs.PrefabConfigs
{
    using System;
    using System.Collections.Generic;
    using Data;
    using Facades;
    using UnityEngine;

    [CreateAssetMenu(fileName = nameof(ProjectilePrefabsConfig), menuName = "Configs/PrefabConfigs/ProjectilePrefabsConfig")]
    public class ProjectilePrefabsConfig : ScriptableObject
    {
        [field: SerializeField] public List<ProjectilePrefab> ProjectilePrefabs { get; private set; } = new();
    }
    
    [Serializable]
    public class ProjectilePrefab
    {
        public EntityType EntityType;
        public ProjectileFacade ProjectileFacade;
    }
}