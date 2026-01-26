namespace GameResources.Scripts.Facades
{
    using System;
    using Configs.Entities;
    using Factories;
    using HealthSystem;
    using MovementSystem;
    using Signals;
    using UniRx;
    using UnityEngine;
    using Zenject;

    public class EnemyFacade : AbstractFacade<EnemyConfig>,
        IPoolable<EnemySpawnData, IMemoryPool>
    {
        [SerializeField] private EnemyHealthController _damageableComponent = default;
        [SerializeField] private Vector3 _offset = default;

        private IDisposable _updateSubscription;
        private IMemoryPool _pool;
        private Transform _targetPlayer;

        private void Start()
        {
            _damageableComponent.EntityDamaged += OnEntityDamaged;
            _damageableComponent.EntityDestroyed += OnEntityDestroyed;
        
            _movementController = new EnemyMovementController(transform, _targetPlayer, _config);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (_damageableComponent != null)
            {
                _damageableComponent.EntityDamaged -= OnEntityDamaged;
                _damageableComponent.EntityDestroyed -= OnEntityDestroyed;
            }

            _updateSubscription?.Dispose();
        }

        #region POOL

        public void OnSpawned(EnemySpawnData enemySpawnData, IMemoryPool pool)
        {
            _pool = pool;
            transform.position = enemySpawnData.TargetPosition + _offset;
            _config = enemySpawnData.EnemyConfig;
            _targetPlayer = enemySpawnData.TargetPlayer;
            _damageableComponent.Initialize(_config);
            
            _updateSubscription = Observable.EveryUpdate()
                .Subscribe(_ => _movementController?.UpdateMovement());
        }

        public void OnDespawned()
        {
            _damageableComponent.Reset();
            _updateSubscription?.Dispose();
            _pool = null;
        }

        public void ReturnToPool()
        {
            if (_pool != null)
            {
                _pool.Despawn(this);
            }
        }

        #endregion

        private void OnEntityDamaged(float obj)
        {
            
        }

        private void OnEntityDestroyed()
        {
            _signalBus?.Fire(new EntityKilledSignal(_entityType));
            ReturnToPool();
        }
    }
}
