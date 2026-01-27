namespace GameResources.Scripts.SpawnSystem
{
    using System;
    using System.Collections.Generic;
    using Data.Entities;
    using Factories;
    using Signals;
    using UniRx;
    using UnityEngine;
    using Zenject;
    using Random = UnityEngine.Random;

    public sealed class EnemySpawnSystem : IDisposable
    {
        public EnemySpawnSystem(SignalBus signalBus, IEnemyFactoryManager enemyEnemyFactory)
        {
            _enemyEnemyFactory = enemyEnemyFactory;
            _signalBus = signalBus;
        
            _signalBus.Subscribe<PlayerCreatedSignal>(OnPlayerCreated);
            _signalBus.Subscribe<EntityKilledSignal>(OnEntityKilled);
        }
        private readonly SignalBus _signalBus;
        private readonly IEnemyFactoryManager _enemyEnemyFactory;

        private const string ENEMY_ENTITY = "Enemy";
        
        private Transform _playerTarget;
        private int _currentEnemyCount = 0;
        private int _maxEnemies;
        private float _spawnInterval;
        private List<Vector3> _spawnPoints;
        private EnemiesConfig _enemiesConfig;
        private IDisposable _spawnTimer;
        
        public void StartSystem(List<Vector3> spawnPoints, EnemiesConfig enemiesConfig, int maxEnemies, float interval)
        {
            _spawnPoints = spawnPoints;
            _enemiesConfig = enemiesConfig;
            _spawnInterval = interval;
            _maxEnemies = maxEnemies;        
            StartSpawning();
        }

        private void OnPlayerCreated(PlayerCreatedSignal signal) => _playerTarget = signal.Transform;

        private void OnEntityKilled(EntityKilledSignal signal)
        {
            if (signal.EntityType.ToString().Contains(ENEMY_ENTITY) && _currentEnemyCount > 0)
            {
                _currentEnemyCount--;
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
                if (_enemiesConfig != null && _enemiesConfig.EnemiesDescription != null &&
                    _enemiesConfig.EnemiesDescription.Count != 0)
                {
                    Vector3 randomSpawnPoint = _spawnPoints[Random.Range(0, _spawnPoints.Count)];
                    EnemyDescription enemyDescription =
                        _enemiesConfig.EnemiesDescription[Random.Range(0, _enemiesConfig.EnemiesDescription.Count)];

                    _enemyEnemyFactory.GetFactory(enemyDescription.EntityType).Create(new EnemySpawnData
                    (
                        randomSpawnPoint,
                        _playerTarget,
                        enemyDescription
                    ));

                    _currentEnemyCount++;
                }
            }
        }

        public void Dispose() => _spawnTimer?.Dispose();
    }
}