namespace GameResources.Scripts.Factories.CustomFactories
{
    using System;
    using System.Collections.Generic;
    using Configs.PrefabConfigs;
    using Data;
    using Facades;
    using UnityEngine;
    using Zenject;

    public class EnemyPool : MonoPoolableMemoryPool<EnemySpawnData, IMemoryPool, EnemyFacade>
    {
        private readonly DiContainer _container;
        private readonly EnemyPrefabsConfig _config;
        private readonly Transform _poolParent;
        private readonly Dictionary<EntityType, EnemyFacade> _prefabCache = new();
        private readonly Dictionary<EntityType, Queue<EnemyFacade>> _availableEnemies = new();

        public EnemyPool(
            DiContainer container,
            EnemyPrefabsConfig config,
            Transform poolParent)
        {
            _container = container;
            _config = config;
            _poolParent = poolParent;
        
            BuildPrefabCache();
        }

        private void BuildPrefabCache()
        {
            foreach (EnemyPrefab entry in _config.EnemyPrefabs)
            {
                _prefabCache[entry.EntityType] = entry.EnemyFacade;
            }
        }

        // Новый метод для спавна врага с данными
        public EnemyFacade SpawnEnemy(EnemySpawnData data)
        {
            EntityType enemyType = data.EnemiesDescription.EntityType;
        
            EnemyFacade enemy;
        
            // Пытаемся взять врага из пула соответствующего типа
            if (_availableEnemies.TryGetValue(enemyType, out Queue<EnemyFacade> queue) && queue.Count > 0)
            {
                enemy = queue.Dequeue();
            }
            else
            {
                // Создаем нового врага
                if (!_prefabCache.TryGetValue(enemyType, out EnemyFacade prefab))
                    throw new Exception($"Prefab for {enemyType} not found in config!");

                enemy = _container.InstantiatePrefabForComponent<EnemyFacade>(
                    prefab.gameObject, _poolParent);
                enemy.gameObject.SetActive(false);
            }
        
            // Используем стандартный Spawn Zenject пула (2 параметра)
            return Spawn(data, this);
        }

        // Переопределяем Reinitialize для инициализации конкретного врага
        protected override void Reinitialize(EnemySpawnData data, IMemoryPool pool, EnemyFacade enemy)
        {
            base.Reinitialize(data, pool, enemy);
        
            // Устанавливаем данные врага
            if (enemy is IPoolable<EnemySpawnData, IMemoryPool> poolable)
            {
                poolable.OnSpawned(data, pool);
            }
        }

        // Переопределяем OnDespawned для возврата врага в пул соответствующего типа
        protected override void OnDespawned(EnemyFacade enemy)
        {
            base.OnDespawned(enemy);
        
            enemy.gameObject.SetActive(false);
        
            // Возвращаем в пул соответствующего типа
            EntityType enemyType = enemy.EntityType;
            if (!_availableEnemies.TryGetValue(enemyType, out Queue<EnemyFacade> queue))
            {
                queue = new Queue<EnemyFacade>();
                _availableEnemies[enemyType] = queue;
            }
        
            queue.Enqueue(enemy);
            enemy.transform.SetParent(_poolParent, false);
        }

        // Дополнительный метод для предварительного заполнения пула
        public void PrewarmPool(int countPerType)
        {
            foreach (KeyValuePair<EntityType, EnemyFacade> kvp in _prefabCache)
            {
                Queue<EnemyFacade> queue = new Queue<EnemyFacade>();
            
                for (int i = 0; i < countPerType; i++)
                {
                    EnemyFacade enemy = _container.InstantiatePrefabForComponent<EnemyFacade>(
                        kvp.Value.gameObject, _poolParent);
                    enemy.gameObject.SetActive(false);
                    queue.Enqueue(enemy);
                }
            
                _availableEnemies[kvp.Key] = queue;
            }
        }
    }
}