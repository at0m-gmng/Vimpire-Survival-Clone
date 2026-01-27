namespace GameResources.Scripts.Factories
{
    using System;
    using System.Collections.Generic;
    using Configs.PrefabConfigs;
    using Data;
    using Zenject;

    public sealed class CollectablesFactoryManager : ICollectableFactoryManager
    {
        public CollectablesFactoryManager(DiContainer container, CollectablePrefabsConfig config)
        {
            _container = container;
            _config = config;
        
            BuildFactoryCache();
        }
        private readonly DiContainer _container;
        private readonly CollectablePrefabsConfig _config;
        private readonly Dictionary<EntityType, CollectablesFactory> _factories = new();

        public CollectablesFactory GetFactory(EntityType entityType)
        {
            if (_factories.TryGetValue(entityType, out CollectablesFactory factory))
                return factory;
        
            throw new Exception($"Factory for {entityType} not found!");
        }

        private void BuildFactoryCache()
        {
            foreach (CollectablePrefab prefab in _config.CollectablePrefabs)
            {
                string id = prefab.EntityType.ToString();
            
                CollectablesFactory factory = _container.ResolveId<CollectablesFactory>(id);
                _factories[prefab.EntityType] = factory;
            }
        }
    }

    public interface ICollectableFactoryManager
    {
        public CollectablesFactory GetFactory(EntityType entityType);
    }
}