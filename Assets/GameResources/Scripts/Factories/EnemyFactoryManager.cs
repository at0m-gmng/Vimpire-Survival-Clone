namespace GameResources.Scripts.Factories
{
    using System;
    using System.Collections.Generic;
    using Configs.PrefabConfigs;
    using Data;
    using Zenject;

    public sealed class EnemyEnemyFactoryManager : IEnemyFactoryManager
    {
        public EnemyEnemyFactoryManager(DiContainer container, EnemyPrefabsConfig config)
        {
            _container = container;
            _config = config;
        
            BuildFactoryCache();
        }
        private readonly DiContainer _container;
        private readonly EnemyPrefabsConfig _config;
        private readonly Dictionary<EntityType, EnemyFactory> _factories = new();

        public EnemyFactory GetFactory(EntityType entityType)
        {
            if (_factories.TryGetValue(entityType, out EnemyFactory factory))
                return factory;
        
            throw new Exception($"Factory for {entityType} not found!");
        }

        private void BuildFactoryCache()
        {
            foreach (EnemyPrefab prefab in _config.EnemyPrefabs)
            {
                string id = prefab.EntityType.ToString();
            
                EnemyFactory factory = _container.ResolveId<EnemyFactory>(id);
                _factories[prefab.EntityType] = factory;
            }
        }
    }

    public interface IEnemyFactoryManager
    {
        public EnemyFactory GetFactory(EntityType entityType);
    }
}