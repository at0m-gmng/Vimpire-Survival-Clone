namespace GameResources.Scripts.Facades
{
    using System;
    using Data.Entities;
    using Factories;
    using HealthSystem;
    using MovementSystem;
    using Signals;
    using UI;
    using UniRx;
    using UnityEngine;
    using Zenject;

    public sealed class EnemyFacade : AbstractFacade<EnemyConfig>,
        IPoolable<EnemySpawnData, IMemoryPool>
    {
        [SerializeField] private EnemyHealthController _damageableComponent = default;
        [SerializeField] private HealthProgressBar _healthProgressBar = default;
        [SerializeField] private Vector3 _offset = default;

        private IDisposable _updateSubscription;
        private IMemoryPool _pool;
        private Transform _targetPlayer;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _updateSubscription?.Dispose();
        }

        #region POOL

        public void OnSpawned(EnemySpawnData enemySpawnData, IMemoryPool pool)
        {
            _pool = pool;
            transform.position = enemySpawnData.TargetPosition + _offset;
            EntityType = enemySpawnData.EnemiesDescription.EntityType;
            _config = enemySpawnData.EnemiesDescription.EnemyConfig;
            _targetPlayer = enemySpawnData.TargetPlayer;
            
            _movementController = new EnemyMovementController(transform, _targetPlayer, _config);
            
            _damageableComponent.Initialize(_config);
            _damageableComponent.EntityDamaged += OnEntityDamaged;
            _damageableComponent.EntityDestroyed += OnEntityDestroyed;
            
            if (_healthProgressBar != null)
            {
                _healthProgressBar.UpdateHealth(_damageableComponent.Health, _damageableComponent.MaxHealth);
                _healthProgressBar.SetVisible(true);
            }
            
            _updateSubscription = Observable.EveryUpdate()
                .Subscribe(_ => UpdateMovement());
        }

        private void UpdateMovement()
        {
            _movementController?.UpdateMovement();
        }

        public void OnDespawned()
        {
            if (_damageableComponent != null)
            {
                _damageableComponent.EntityDamaged -= OnEntityDamaged;
                _damageableComponent.EntityDestroyed -= OnEntityDestroyed;
            }
            _updateSubscription?.Dispose();
            _pool = null;
            
            if (_healthProgressBar != null)
            {
                _healthProgressBar.SetVisible(false);
            }
        }

        public void ReturnToPool()
        {
            if (_pool != null)
            {
                _pool.Despawn(this);
            }
        }

        #endregion

        private void OnEntityDamaged(float currentHealth) 
            => _healthProgressBar.UpdateHealth(_damageableComponent.Health, _damageableComponent.MaxHealth);

        private void OnEntityDestroyed()
        {
            _updateSubscription?.Dispose();
            _signalBus?.Fire(new EntityKilledSignal(EntityType, _config.ExperienceType, transform.position));
            ReturnToPool();
        }
    }
}
