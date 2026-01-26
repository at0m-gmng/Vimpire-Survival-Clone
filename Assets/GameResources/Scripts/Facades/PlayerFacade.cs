namespace GameResources.Scripts.Facades
{
    using System;
    using AttackSystem;
    using Configs.Entities;
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
        
        #region POOL

        public void OnSpawned(PlayerSpawnData playerSpawnData, IMemoryPool pool)
        {
            _pool = pool;
            _config = playerSpawnData.PlayerConfig;
            transform.SetPositionAndRotation(playerSpawnData.TargetTransform.position, playerSpawnData.TargetTransform.rotation);
            
            _movementController = new PlayerMovementController(EntityTransform, _config, _inputSystem);
            _attackController = new PlayerAttackController(EntityTransform, _damageEffect, _targetMask,
                _config.AttackRange, _config.AttackDamage, _config.AttackCooldown);
            // _experienceController = new PlayerExperienceController(_signalBus);
            // _levelController = new PlayerLevelController(_signalBus);

            _attackController.Start();
            // _experienceController?.Start();
            // _levelController?.Start();

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
