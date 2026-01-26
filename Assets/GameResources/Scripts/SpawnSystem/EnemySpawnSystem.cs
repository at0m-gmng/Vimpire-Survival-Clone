namespace GameResources.Scripts.SpawnSystem
{
    using System;
    using System.Collections.Generic;
    using Configs.Entities;
    using Facades;
    using Factories;
    using Signals;
    using UniRx;
    using UnityEngine;
    using Zenject;
    using Random = UnityEngine.Random;

    public class EnemySpawnSystem : IDisposable
    {
        private readonly SignalBus _signalBus;
        private readonly EnemyFactory _enemyFactory;
    
        private Transform _playerTarget;
        private int _currentEnemyCount = 0;
        private int _maxEnemies;
        private float _spawnInterval;
        private List<Vector3> _spawnPoints;
        private EnemyConfig _enemyConfig;
        private IDisposable _spawnTimer;

        public EnemySpawnSystem(SignalBus signalBus, EnemyFactory enemyFactory)
        {
            _enemyFactory = enemyFactory;
            _signalBus = signalBus;
        
            _signalBus.Subscribe<PlayerCreatedSignal>(OnPlayerCreated);
            _signalBus.Subscribe<EntityKilledSignal>(OnEntityKilled);
        }

        public void StartSystem(List<Vector3> spawnPoints, EnemyConfig enemyConfig, int maxEnemies, float interval)
        {
            _spawnPoints = spawnPoints;
            _enemyConfig = enemyConfig;
            _spawnInterval = interval;
            _maxEnemies = maxEnemies;        
            StartSpawning();
        }

        private void OnPlayerCreated(PlayerCreatedSignal signal) => _playerTarget = signal.Transform;

        private void OnEntityKilled(EntityKilledSignal signal)
        {
            if (signal.EntityType == EntityType.Enemy && _currentEnemyCount > 0)
            {
                _currentEnemyCount--;
                // TrySpawnEnemy();
            }
        }

        private void StartSpawning()
        {
            _spawnTimer?.Dispose();
            _spawnTimer = Observable
                .Interval(TimeSpan.FromSeconds(_spawnInterval))
                .Subscribe(_ => TrySpawnEnemy());
        }

        private void TrySpawnEnemy()
        {
            if (_currentEnemyCount < _maxEnemies && _playerTarget != null && _spawnPoints.Count != 0)
            {
                Vector3 randomSpawnPoint = _spawnPoints[Random.Range(0, _spawnPoints.Count)];

                _enemyFactory.Create(new EnemySpawnData
                (
                    randomSpawnPoint,
                    _playerTarget,
                    _enemyConfig
                ));

                _currentEnemyCount++;
            }
        }

        public void Dispose() => _spawnTimer?.Dispose();
    }
}