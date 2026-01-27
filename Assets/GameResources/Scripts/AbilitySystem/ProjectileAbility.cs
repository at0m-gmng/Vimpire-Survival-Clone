namespace GameResources.Scripts.AbilitySystem
{
    using System.Collections.Generic;
    using Factories;
    using HealthSystem;
    using UniRx;
    using UniRx.Triggers;
    using UnityEngine;

    public sealed class ProjectileAbility : Ability
    {
        public ProjectileAbility(IProjectileFactoryManager projectileFactoryManager, Collider detectionTrigger, Transform shootingPoint)
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
        private bool _isDetecting;
        private bool _isInitialized;
        private readonly HashSet<Collider> _detectedTargets = new();

        protected override void OnInitialize()
        {
            if (!_isInitialized)
            {
                _disposables = new CompositeDisposable();
                _currentCooldown = 0f;
                _isDetecting = false;

                _detectionTrigger.enabled = false;
                _detectionTrigger.isTrigger = true;
        
                _detectionTrigger.OnTriggerEnterAsObservable()
                    .Subscribe(OnTargetEnter)
                    .AddTo(_disposables);

                _isInitialized = true;
                StartUpdate();
            }

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
                Transform nearest = GetNearestTarget();
                if (nearest != null)
                {
                    ShootAt(nearest);
                }
            }

            _detectedTargets.Clear();
            _detectionTrigger.enabled = false;
            _isDetecting = false;
            _currentCooldown = Config.Cooldown;
        }

        private Transform GetNearestTarget()
        {
            Transform nearest = null;
            float minDistance = float.MaxValue;
        
            foreach (Collider target in _detectedTargets)
            {
                if (target == null) continue;
            
                float distance = Vector3.Distance(_shootingPoint.position, target.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearest = target.transform;
                }
            }
            return nearest;
        }

        private void ShootAt(Transform target)
        {
            _projectileFactoryManager.GetFactory(_configDescription.EntityType).Create(new ProjectileSpawnData
            (
                _shootingPoint.position,
                target,
                _configDescription
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
                    .Subscribe(_ => TryShoot())
                    .AddTo(_disposables);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            _disposables?.Dispose();
            _detectedTargets.Clear();
        }
    }}