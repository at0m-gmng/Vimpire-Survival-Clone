namespace GameResources.Scripts.Facades
{
    using System;
    using AbilitySystem;
    using Factories;
    using Data;
    using Data.Entities;
    using MovementSystem;
    using UniRx;
    using UnityEngine;
    using Zenject;

    public sealed class ProjectileFacade : AbstractFacade<EntityConfig>, IPoolable<ProjectileSpawnData, IMemoryPool>
    {
        private const float DEFAULT_LIFETIME = 5f;
        
        [SerializeField] private LayerMask _targetMask;
        [SerializeField] private Collider _trigger;
        
        private IDisposable _updateSubscription = null;
        private IDisposable _lifetimeSubscription = null;
        private IMemoryPool _pool;
        private Transform _targetPlayer;
        private TriggerDamageAbility _rangeAttack;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _updateSubscription?.Dispose();
            _lifetimeSubscription?.Dispose();
        }
        
        #region POOL
        
        public void OnSpawned(ProjectileSpawnData spawnData, IMemoryPool pool)
        {
            _pool = pool;
            _entityType = spawnData.AbilityDescription.EntityType;
            transform.position = spawnData.TargetPosition;
            _targetPlayer = spawnData.TargetPlayer;
            Vector3 direction = spawnData.TargetPlayer.position - transform.position;
            direction.y = 0;
            transform.rotation = Quaternion.LookRotation(direction);
            
            AbilityDescription abilityDescription = spawnData.AbilityDescription;
            _rangeAttack = new TriggerDamageAbility(_trigger, _targetMask);
            _rangeAttack.EntityDamaged += OnEntityDamaged;
            _rangeAttack.Initialize(abilityDescription);
            _abilities.Add(_rangeAttack);

            if (_entityType == EntityType.ProjectileAbility)
            {
                _movementController = new ShootMovementController(new ShootMovementData
                (
                    EntityTransform,
                    direction,
                    abilityDescription.AbilityConfig.Speed
                ));
            
                _updateSubscription = Observable.EveryUpdate()
                    .Subscribe(_ => UpdateMovement());
                
                _lifetimeSubscription = Observable.Timer(TimeSpan.FromSeconds(DEFAULT_LIFETIME))
                    .Subscribe(_ => ReturnToPool());
            }
        }

        private void UpdateMovement() => _movementController?.UpdateMovement();

        private void OnEntityDamaged() => ReturnToPool();

        public void OnDespawned()
        {
            if (_rangeAttack != null)
            {
                _rangeAttack.EntityDamaged -= OnEntityDamaged;
                _rangeAttack.Dispose();
                _rangeAttack = null;
            }

            _abilities.Clear();
            
            _updateSubscription?.Dispose();
            _updateSubscription = null;
            
            _lifetimeSubscription?.Dispose();
            _lifetimeSubscription = null;
            
            _pool = null;
        }

        public void ReturnToPool()
        {
            if (_pool != null)
            {
                _pool.Despawn(this);
            }
        }

        public void SetPosition(Vector3 position) => transform.position = position;

        #endregion
    }
}