namespace GameResources.Scripts.AbilitySystem
{
    using System.Collections.Generic;
    using System.Linq;
    using HealthSystem;
    using UniRx;
    using UniRx.Triggers;
    using UnityEngine;

    public sealed class AuraDamageAbility : Ability
    {
        public AuraDamageAbility(Collider damageTrigger, GameObject attackEffect, LayerMask targetLayerMask)
        {
            _damageTrigger = damageTrigger;
            _attackEffect = attackEffect;
            _targetLayerMask = targetLayerMask;
        }
        private readonly Collider _damageTrigger;
        private readonly GameObject _attackEffect;
        private readonly LayerMask _targetLayerMask;
        
        private readonly HashSet<Collider> _detectedEnemies = new();
        private readonly CompositeDisposable _disposables = new();
        private float _currentDamage;
        private float _currentRadius;
        private float _damageInterval;
        private bool _isInitialized;

        protected override void OnInitialize()
        {
            if (!_isInitialized)
            {
                _currentDamage = Config.Damage;
                _currentRadius = Config.Radius;
                _damageInterval = Config.Cooldown;

                _attackEffect.gameObject.SetActive(true);

                _damageTrigger.OnTriggerEnterAsObservable()
                    .Subscribe(collider =>
                    {
                        if (collider != null && collider.gameObject.activeInHierarchy)
                        {
                            int colliderLayer = 1 << collider.gameObject.layer;
                            if ((_targetLayerMask.value & colliderLayer) != 0)
                            {
                                IDamageable damageable = collider.GetComponent<IDamageable>();
                                if (damageable != null && damageable.Health > 0)
                                {
                                    _detectedEnemies.Add(collider);
                                }
                            }
                        }
                    })
                    .AddTo(_disposables);

                _damageTrigger.OnTriggerExitAsObservable()
                    .Subscribe(collider =>
                    {
                        _detectedEnemies.Remove(collider);
                    })
                    .AddTo(_disposables);

                Observable.Interval(System.TimeSpan.FromSeconds(_damageInterval))
                    .Subscribe(_ => DamageTargets())
                    .AddTo(_disposables);

                _isInitialized = true;
            }
            else
            {
                _currentDamage += Config.Damage;
                _currentRadius += Config.Radius;
                _damageInterval *= 0.9f;
            }

            _attackEffect.transform.localScale = Vector3.one * _currentRadius;
        }

        private void DamageTargets()
        {
            foreach (Collider collider in _detectedEnemies.ToList())
            {
                if (collider != null && collider.gameObject.activeInHierarchy)
                {
                    IDamageable damageable = collider.GetComponent<IDamageable>();
                    if (damageable != null && damageable.Health > 0)
                    {
                        damageable.TakeDamage((int)_currentDamage);
                    }

                    if (damageable == null || damageable.Health <= 0)
                    {
                        _detectedEnemies.Remove(collider);
                    }
                }
                else
                {
                    _detectedEnemies.Remove(collider);
                }
            }


            foreach (Collider detectedEnemy in _detectedEnemies)
            {
                if (detectedEnemy == null)
                {
                    _detectedEnemies.Remove(detectedEnemy);
                }
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            _disposables.Dispose();
            _detectedEnemies.Clear();
        }
    }
}