namespace GameResources.Scripts.AbilitySystem
{
    using System;
    using HealthSystem;
    using UniRx;
    using UnityEngine;

    public sealed class RangeAttackAbility : Ability
    {
        private const int MAX_TARGETS = 50;
        
        public RangeAttackAbility(Transform playerTransform, GameObject attackEffect, LayerMask targetMask)
        {
            _playerTransform = playerTransform;
            _attackEffect = attackEffect;
            _targetMask = targetMask;
        }
        private readonly Transform _playerTransform;
        private readonly GameObject _attackEffect;
        private readonly LayerMask _targetMask;
        
        private float _attackRange;
        private int _attackDamage;
        private float _attackCooldown;
        private float _timeSinceLastAttack;
        private IDisposable _effectTimer;
        private readonly Collider[] _colliderBuffer = new Collider[MAX_TARGETS];

        protected override void OnInitialize()
        {
            _attackRange = Config.Radius;
            _attackDamage = (int)Config.Damage;
            _attackCooldown = Config.Cooldown;
            _timeSinceLastAttack = 0f;

            _attackEffect.transform.localScale = Vector3.one * _attackRange;
            
            StartUpdate();
        }

        protected override void OnUpdate(float deltaTime)
        {
            _timeSinceLastAttack += deltaTime;
            
            if (_timeSinceLastAttack >= _attackCooldown)
            {
                PerformAttack();
                _timeSinceLastAttack = 0f;
            }
        }

        private void PerformAttack()
        {
            DamageTargets();

            _effectTimer?.Dispose();
            _attackEffect.SetActive(true);
            _effectTimer = Observable.Timer(TimeSpan.FromSeconds(_attackCooldown * 0.1f))
                .Subscribe(_ => _attackEffect.SetActive(false));
        }

        private void DamageTargets()
        {
            int hitCount = Physics.OverlapSphereNonAlloc(
                _playerTransform.position, 
                _attackRange, 
                _colliderBuffer, 
                _targetMask
            );
            
            for (int i = 0; i < hitCount; i++)
            {
                IDamageable damageable = _colliderBuffer[i].GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(_attackDamage);
                }
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            _effectTimer?.Dispose();
        }
    }
}
