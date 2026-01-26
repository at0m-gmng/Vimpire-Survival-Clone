namespace GameResources.Scripts.AttackSystem
{
    using System;
    using HealthSystem;
    using UniRx;
    using UnityEngine;
    using Object = UnityEngine.Object;

    public class PlayerAttackController : AbstractAttackController
    {
        private readonly Transform _playerTransform;
        private readonly float _attackRange;
        private readonly int _attackDamage;
        private readonly float _attackCooldown;
    
        private IDisposable _attackSubscription;
        private float _lastAttackTime;
        private LayerMask _targetMask;
        
        private GameObject _attackEffect;
        private IDisposable _sphereTimer;

        public PlayerAttackController(Transform playerTransform, GameObject attackEffect, LayerMask targetMask,
            float attackRange, int attackDamage, float attackCooldown)
        {
            _playerTransform = playerTransform;
            _attackRange = attackRange;
            _attackDamage = attackDamage;
            _attackCooldown = attackCooldown;
            _lastAttackTime = -attackCooldown;
            _attackEffect = attackEffect;
            _targetMask = targetMask;

            _attackEffect.transform.localScale = new Vector3(_attackRange, _attackRange, _attackRange);
        }

        public override void Start()
        {
            _attackSubscription = Observable.EveryUpdate()
                .Where(_ => Time.time - _lastAttackTime >= _attackCooldown)
                .Subscribe(_ => PerformAttack());
        }

        public override void Dispose()
        {
            _attackSubscription?.Dispose();
            _sphereTimer?.Dispose();
            Object.DestroyImmediate(_attackEffect);
        }

        private void PerformAttack()
        {
            DamageTargets();
           
            _lastAttackTime = Time.time;

            _sphereTimer?.Dispose();
            _attackEffect.SetActive(true);
            _sphereTimer = Observable.Timer(TimeSpan.FromSeconds(_attackCooldown * 0.1f))
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
    }
}
