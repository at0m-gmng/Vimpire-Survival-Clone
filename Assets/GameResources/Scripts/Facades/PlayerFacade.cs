namespace GameResources.Scripts.Facades
{
    using System;
    using System.Linq;
    using AbilitySystem;
    using Data;
    using Data.Entities;
    using ExperienceSystem;
    using Factories;
    using InputSystem;
    using MovementSystem;
    using Signals;
    using UniRx;
    using UnityEngine;
    using Zenject;

    public sealed class PlayerFacade : AbstractFacade<PlayerConfig>,
        IPoolable<PlayerSpawnData, IMemoryPool>
    {
        [Inject]
        private void Construct(IInputSystem inputSystem, IProjectileFactoryManager projectileFactoryManager)
        {
            _inputSystem = inputSystem;
            _projectileFactoryManager = projectileFactoryManager;
        }
        private IInputSystem _inputSystem;
        private IProjectileFactoryManager _projectileFactoryManager;

        [SerializeField] private LayerMask _targetMask;
        [SerializeField] private GameObject _damageEffect;
        [SerializeField] private Collider _initializeCollectTrigger;
        [SerializeField] private Collider _collectTrigger;
        [SerializeField] private Collider _auraDamageTrigger;
        [SerializeField] private GameObject _auraEffect;
        [SerializeField] private Collider _projectileAbilityTrigger;
        
        private IDisposable _updateSubscription;
        private IMemoryPool _pool;
        private Transform _poolParent;
        private AbilitiesConfig _abilitiesConfig;

        #region POOL

        public void OnSpawned(PlayerSpawnData playerSpawnData, IMemoryPool pool)
        {
            _pool = pool;
            _config = playerSpawnData.PlayerConfig;
            _abilitiesConfig = playerSpawnData.AbilitiesConfig;
            transform.SetPositionAndRotation(playerSpawnData.TargetTransform.position, playerSpawnData.TargetTransform.rotation);
            
            _movementController = new PlayerMovementController(EntityTransform, _config, _inputSystem);
            _experienceController = new ExperienceController(_initializeCollectTrigger,_collectTrigger);

            CreateRangeAttackAbility();

            _experienceController?.Start();

            _signalBus.Subscribe<RewardSelectedSignal>(OnRewardSelected);

            _updateSubscription = Observable.EveryUpdate()
                .Subscribe(_ =>
                {
                    _movementController?.UpdateMovement();
                });
        }

        private void CreateRangeAttackAbility()
        {
            AbilityDescription abilityDescription = new();
            RangeAttackAbility rangeAttack = new(EntityTransform, _damageEffect, _targetMask);
            rangeAttack.Initialize(abilityDescription);
            _abilities.Add(rangeAttack);
        }

        private void OnRewardSelected(RewardSelectedSignal signal) => CreateOrUpgradeAbility(signal.SelectedEntityType);

        private void CreateOrUpgradeAbility(EntityType entityType)
        {
            AbilityDescription abilityDescription = _abilitiesConfig.AbilitiesDescription
                .FirstOrDefault(desc => desc.EntityType == entityType);

            if (abilityDescription != null)
            {
                Ability existingAbility = FindAbilityByEntityType(entityType);

                if (existingAbility == null)
                {
                    existingAbility = CreateAbility(entityType);
                    _abilities.Add(existingAbility);

                }
                existingAbility.Initialize(abilityDescription);
            }
        }

        private Ability FindAbilityByEntityType(EntityType entityType)
        {
            switch (entityType)
            {
                case EntityType.AuraAbility:
                    return _abilities.OfType<AuraDamageAbility>().FirstOrDefault();
                case EntityType.ProjectileAbility:
                    return _abilities.OfType<ProjectileAbility>().FirstOrDefault();
                default:
                    return null;
            }
        }

        private Ability CreateAbility(EntityType entityType)
        {
            switch (entityType)
            {
                case EntityType.AuraAbility:
                    return new AuraDamageAbility(_auraDamageTrigger, _auraEffect, _targetMask);
                case EntityType.ProjectileAbility:
                    return new ProjectileAbility(_projectileFactoryManager, _projectileAbilityTrigger, EntityTransform);
                default:
                    return null;
            }
        }

        public void OnDespawned()
        {
            _pool = null;
            _updateSubscription?.Dispose();
            _experienceController?.Dispose();
            
            _signalBus.TryUnsubscribe<RewardSelectedSignal>(OnRewardSelected);

            foreach (var ability in _abilities)
            {
                ability?.Dispose();
            }
            _abilities.Clear();
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
