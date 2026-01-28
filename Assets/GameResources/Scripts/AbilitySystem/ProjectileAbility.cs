namespace GameResources.Scripts.AbilitySystem
{
    using System.Collections.Generic;
    using Data.Entities;
    using Factories;
    using HealthSystem;
    using UniRx;
    using UniRx.Triggers;
    using UnityEngine;

    public sealed class ProjectileAbility : Ability
    {
        public ProjectileAbility(IProjectileFactoryManager projectileFactoryManager, Collider detectionTrigger, 
            Transform shootingPoint)
        {
            _projectileFactoryManager = projectileFactoryManager;
            _detectionTrigger = detectionTrigger;
            _shootingPoint = shootingPoint;
        }
        private IProjectileFactoryManager _projectileFactoryManager;
        private Collider _detectionTrigger;
        private Transform _shootingPoint;
        
        private CompositeDisposable _disposables;
        private float _currentCooldown;
        private float _currentCooldownInterval;
        private float _currentDamage;
        private int _currentEntitiesCount;
        private bool _isDetecting;
        private bool _isInitialized;
        private readonly HashSet<Collider> _detectedTargets = new();
        private readonly List<Transform> _targetList = new();
        private readonly List<(Collider collider, float distance)> _sortedTargets = new();
        
        private static readonly DistanceComparer _distanceComparer = new();

        protected override void OnInitialize()
        {
            if (!_isInitialized)
            {
                _disposables = new CompositeDisposable();
                _currentCooldown = 0f;
                _currentCooldownInterval = Config.Cooldown;
                _currentDamage = Config.Damage;
                _currentEntitiesCount = Mathf.Max(1, Config.EntitiesCount);
                _isDetecting = false;

                _detectionTrigger.enabled = false;
                _detectionTrigger.isTrigger = true;
        
                _detectionTrigger.OnTriggerEnterAsObservable()
                    .Subscribe(OnTargetEnter)
                    .AddTo(_disposables);

                _isInitialized = true;
                StartUpdate();
            }
            else
            {
                _currentDamage += Config.Damage;
                _currentEntitiesCount += Config.EntitiesCount;
                _currentCooldownInterval *= 0.9f;
            }

            _detectionTrigger.gameObject.SetActive(true);
            _detectionTrigger.transform.localScale = Vector3.one * Config.Radius;
        }

        private void OnTargetEnter(Collider other)
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                _detectedTargets.Add(other);
            }
        }
        
        private void TryShoot()
        {
            if (_detectedTargets.Count > 0)
            {
                GetPrioritizedTargets(_currentEntitiesCount);
                foreach (Transform target in _targetList)
                {
                    if (target != null)
                    {
                        ShootAt(target);
                    }
                }
            }

            _detectedTargets.Clear();
            _detectionTrigger.enabled = false;
            _isDetecting = false;
        }

        private void GetPrioritizedTargets(int count)
        {
            _targetList.Clear();
            _sortedTargets.Clear();
        
            foreach (Collider target in _detectedTargets)
            {
                if (target == null) continue;
            
                float distance = Vector3.Distance(_shootingPoint.position, target.transform.position);
                _sortedTargets.Add((target, distance));
            }

            _sortedTargets.Sort(_distanceComparer);

            int targetCount = Mathf.Min(count, _sortedTargets.Count);
            for (int i = 0; i < targetCount; i++)
            {
                _targetList.Add(_sortedTargets[i].collider.transform);
            }
        }

        private void ShootAt(Transform target)
        {
            AbilityDescription projectileConfig = new AbilityDescription
            {
                EntityType = _configDescription.EntityType,
                AbilityConfig = new AbilityConfig
                {
                    Damage = _currentDamage,
                    Cooldown = Config.Cooldown,
                    Radius = Config.Radius,
                    Speed = Config.Speed,
                    EntitiesCount = Config.EntitiesCount
                }
            };

            _projectileFactoryManager.GetFactory(_configDescription.EntityType).Create(new ProjectileSpawnData
            (
                _shootingPoint.position,
                target,
                projectileConfig
            ));
        }

        protected override void OnUpdate(float deltaTime)
        {
            _currentCooldown -= deltaTime;
        
            if (_currentCooldown <= 0 && !_isDetecting)
            {
                _isDetecting = true;
                _detectionTrigger.enabled = true;
            
                Observable.TimerFrame(2)
                    .Subscribe(_ =>
                    {
                        TryShoot();
                        _currentCooldown = _currentCooldownInterval;
                    })
                    .AddTo(_disposables);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            _disposables?.Dispose();
            _detectedTargets.Clear();
            _targetList.Clear();
            _sortedTargets.Clear();
        }
        
        private class DistanceComparer : IComparer<(Collider collider, float distance)>
        {
            public int Compare((Collider collider, float distance) a, (Collider collider, float distance) b)
            {
                return a.distance.CompareTo(b.distance);
            }
        }
    }
}