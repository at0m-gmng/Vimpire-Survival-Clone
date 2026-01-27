namespace GameResources.Scripts.Factories
{
    using System;
    using System.Collections.Generic;
    using Configs.PrefabConfigs;
    using Data;
    using Zenject;

    public sealed class ProjectileFactoryManager : IProjectileFactoryManager
    {
        public ProjectileFactoryManager(DiContainer container, ProjectilePrefabsConfig config)
        {
            _container = container;
            _config = config;
        
            BuildFactoryCache();
        }
        private readonly DiContainer _container;
        private readonly ProjectilePrefabsConfig _config;
        private readonly Dictionary<EntityType, ProjectileFactory> _factories = new();

        public ProjectileFactory GetFactory(EntityType entityType)
        {
            if (_factories.TryGetValue(entityType, out ProjectileFactory factory))
                return factory;
        
            throw new Exception($"Factory for {entityType} not found!");
        }

        private void BuildFactoryCache()
        {
            foreach (ProjectilePrefab prefab in _config.ProjectilePrefabs)
            {
                string id = prefab.EntityType.ToString();
            
                ProjectileFactory factory = _container.ResolveId<ProjectileFactory>(id);
                _factories[prefab.EntityType] = factory;
            }
        }
    }

    public interface IProjectileFactoryManager
    {
        public ProjectileFactory GetFactory(EntityType entityType);
    }
}