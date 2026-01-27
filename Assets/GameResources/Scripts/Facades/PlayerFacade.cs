namespace GameResources.Scripts.Facades
{
    using System;
    using AttackSystem;
    using Data.Entities;
    using ExperienceSystem;
    using Factories;
    using InputSystem;
    using MovementSystem;
    using UniRx;
    using UnityEngine;
    using Zenject;

    public sealed class PlayerFacade : AbstractFacade<PlayerConfig>,
        IPoolable<PlayerSpawnData, IMemoryPool>
    {
        [Inject]
        private void Construct(IInputSystem inputSystem) => _inputSystem = inputSystem;
        private IInputSystem _inputSystem;

        private IDisposable _updateSubscription;
        private IMemoryPool _pool;
        private Transform _poolParent;

        [SerializeField] private LayerMask _targetMask;
        [SerializeField] private GameObject _damageEffect;
        [SerializeField] private Collider _initializeCollectTrigger;
        [SerializeField] private Collider _collectTrigger;
        
        #region POOL

        public void OnSpawned(PlayerSpawnData playerSpawnData, IMemoryPool pool)
        {
            _pool = pool;
            _config = playerSpawnData.PlayerConfig;
            transform.SetPositionAndRotation(playerSpawnData.TargetTransform.position, playerSpawnData.TargetTransform.rotation);
            
            _movementController = new PlayerMovementController(EntityTransform, _config, _inputSystem);
            _attackController = new PlayerAttackController(EntityTransform, _damageEffect, _targetMask,
                _config.AttackRange, _config.AttackDamage, _config.AttackCooldown);
            _experienceController = new ExperienceController(_initializeCollectTrigger,_collectTrigger);

            _attackController.Start();
            _experienceController?.Start();

            _updateSubscription = Observable.EveryUpdate()
                .Subscribe(_ =>
                {
                    _movementController?.UpdateMovement();
                });
        }

        public void OnDespawned()
        {
            _pool = null;
            _updateSubscription?.Dispose();
            _attackController?.Dispose();
            _experienceController?.Dispose();
        }

        public void ReturnToPool()
        {
            if (_pool != null)
            {
                _pool.Despawn(this);
            }
        }

        #endregion
    }
}
