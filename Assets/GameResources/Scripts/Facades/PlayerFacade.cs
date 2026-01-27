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
        private void Construct(IInputSystem inputSystem) => _inputSystem = inputSystem;
        private IInputSystem _inputSystem;

        private IDisposable _updateSubscription;
        private IMemoryPool _pool;
        private Transform _poolParent;
        private AbilitiesConfig _abilitiesConfig;

        [SerializeField] private LayerMask _targetMask;
        [SerializeField] private GameObject _damageEffect;
        [SerializeField] private Collider _initializeCollectTrigger;
        [SerializeField] private Collider _collectTrigger;
        [SerializeField] private Collider _auraDamageTrigger;
        [SerializeField] private GameObject _auraEffect;
        
        #region POOL

        public void OnSpawned(PlayerSpawnData playerSpawnData, IMemoryPool pool)
        {
            _pool = pool;
            _config = playerSpawnData.PlayerConfig;
            _abilitiesConfig = playerSpawnData.AbilitiesConfig;
            transform.SetPositionAndRotation(playerSpawnData.TargetTransform.position, playerSpawnData.TargetTransform.rotation);
            
            _movementController = new PlayerMovementController(EntityTransform, _config, _inputSystem);
            _experienceController = new ExperienceController(_initializeCollectTrigger,_collectTrigger);

            // CreateRangeAttackAbility();
            CreateTestAuraAbility();

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
            RangeAttackAbility rangeAttack = new RangeAttackAbility(EntityTransform, _damageEffect, _targetMask);
            
            AbilityConfig attackConfig = new AbilityConfig
            {
                Damage = _config.AttackDamage,
                Radius = _config.AttackRange,
                Cooldown = _config.AttackCooldown
            };
            
            rangeAttack.Initialize(attackConfig);
            _abilities.Add(rangeAttack);
        }

        private void CreateTestAuraAbility()
        {
            AuraDamageAbility auraAbility = new AuraDamageAbility(_auraDamageTrigger, _auraEffect, _targetMask);
            
            AbilityConfig auraConfig = new AbilityConfig
            {
                Damage = 1f,
                Radius = 3f,
                Cooldown = 0.2f
            };
            
            auraAbility.Initialize(auraConfig);
            _abilities.Add(auraAbility);
        }

        private void OnRewardSelected(RewardSelectedSignal signal)
        {
            CreateOrUpgradeAbility(signal.SelectedEntityType);
        }

        private void CreateOrUpgradeAbility(EntityType entityType)
        {
            AbilityDescription abilityDescription = _abilitiesConfig.AbilitiesDescription
                .FirstOrDefault(desc => desc.EntityType == entityType);

            if (abilityDescription == null)
            {
                return;
            }

            Ability existingAbility = FindAbilityByEntityType(entityType);

            if (existingAbility != null)
            {
                existingAbility.Initialize(abilityDescription.AbilityConfig);
            }
            else
            {
                Ability newAbility = CreateAbility(entityType);
                if (newAbility != null)
                {
                    newAbility.Initialize(abilityDescription.AbilityConfig);
                    _abilities.Add(newAbility);
                }
            }
        }

        private Ability FindAbilityByEntityType(EntityType entityType)
        {
            switch (entityType)
            {
                case EntityType.AuraAbility:
                    return _abilities.OfType<AuraDamageAbility>().FirstOrDefault();
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
