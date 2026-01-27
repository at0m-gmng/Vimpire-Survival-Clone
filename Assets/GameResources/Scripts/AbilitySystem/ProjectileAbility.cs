namespace GameResources.Scripts.AbilitySystem
{
    using System.Collections.Generic;
    using Factories;
    using HealthSystem;
    using UniRx;
    using UniRx.Triggers;
    using UnityEngine;

    public class ProjectileAbility : Ability
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

                Debug.Log($"[ProjectileAbility] First initialization. Cooldown: {Config.Cooldown}, Damage: {Config.Damage}, Radius: {Config.Radius}, Trigger: {_detectionTrigger.name}, TriggerGameObject Active: {_detectionTrigger.gameObject.activeInHierarchy}");
                
                _isInitialized = true;
                StartUpdate();
            }
            else
            {
                Debug.Log($"[ProjectileAbility] Upgrade! New Cooldown: {Config.Cooldown}, Damage: {Config.Damage}, Radius: {Config.Radius}");
            }

            _detectionTrigger.transform.localScale = Vector3.one * Config.Radius;
        }

        private void OnTargetEnter(Collider other)
        {
            Debug.Log($"[ProjectileAbility] OnTriggerEnter called! Collider: {other.name}, Layer: {LayerMask.LayerToName(other.gameObject.layer)}");
            
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                _detectedTargets.Add(other);
                Debug.Log($"[ProjectileAbility] Target detected: {other.name}, Total targets: {_detectedTargets.Count}");
            }
            else
            {
                Debug.LogWarning($"[ProjectileAbility] Collider {other.name} has no IDamageable component");
            }
        }
        
        private void TryShoot()
        {
            Debug.Log($"[ProjectileAbility] TryShoot called. Detected targets: {_detectedTargets.Count}");
            
            if (_detectedTargets.Count > 0)
            {
                Transform nearest = GetNearestTarget();
                if (nearest != null)
                {
                    Debug.Log($"[ProjectileAbility] Shooting at: {nearest.name}");
                    ShootAt(nearest);
                }
                else
                {
                    Debug.LogWarning("[ProjectileAbility] No valid nearest target found");
                }
            }

            _detectedTargets.Clear();
            _detectionTrigger.enabled = false;
            _isDetecting = false;
            _currentCooldown = Config.Cooldown;
            Debug.Log($"[ProjectileAbility] Reset. Next shot in {_currentCooldown}s");
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
            Debug.Log($"[ProjectileAbility] Creating projectile at {_shootingPoint.position} targeting {target.position}");
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
                Debug.Log($"[ProjectileAbility] Cooldown ready! Enabling detection trigger. Trigger enabled: {_detectionTrigger.enabled}, GameObject active: {_detectionTrigger.gameObject.activeInHierarchy}");
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