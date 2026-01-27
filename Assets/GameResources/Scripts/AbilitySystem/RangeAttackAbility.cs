namespace GameResources.Scripts.AbilitySystem
{
    using System;
    using HealthSystem;
    using UniRx;
    using UnityEngine;

    public class RangeAttackAbility : Ability
    {
        private readonly Transform _playerTransform;
        private readonly GameObject _attackEffect;
        private readonly LayerMask _targetMask;
        
        private float _attackRange;
        private int _attackDamage;
        private float _attackCooldown;
        private float _lastAttackTime;
        
        private IDisposable _attackSubscription;
        private IDisposable _effectTimer;

        public RangeAttackAbility(Transform playerTransform, GameObject attackEffect, LayerMask targetMask)
        {
            _playerTransform = playerTransform;
            _attackEffect = attackEffect;
            _targetMask = targetMask;
        }

        protected override void OnInitialize()
        {
            _attackRange = Config.Radius;
            _attackDamage = (int)Config.Damage;
            _attackCooldown = Config.Cooldown;
            _lastAttackTime = -_attackCooldown;

            _attackEffect.transform.localScale = Vector3.one * _attackRange;

            _attackSubscription?.Dispose();
            _attackSubscription = Observable.EveryUpdate()
                .Where(_ => Time.time - _lastAttackTime >= _attackCooldown)
                .Subscribe(_ => PerformAttack());
        }

        private void PerformAttack()
        {
            DamageTargets();
            _lastAttackTime = Time.time;

            _effectTimer?.Dispose();
            _attackEffect.SetActive(true);
            _effectTimer = Observable.Timer(TimeSpan.FromSeconds(_attackCooldown * 0.1f))
                .Subscribe(_ => _attackEffect.SetActive(false));
        }

        private void DamageTargets()
        {
            Collider[] colliders = Physics.OverlapSphere(_playerTransform.position, _attackRange, _targetMask);
            
            foreach (Collider collider in colliders)
            {
                IDamageable damageable = collider.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(_attackDamage);
                }
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            _attackSubscription?.Dispose();
            _effectTimer?.Dispose();
        }
    }
}
