namespace GameResources.Scripts.AbilitySystem
{
    using System.Collections.Generic;
    using Data;
    using Data.Entities;
    using Factories;
    using Facades;
    using UnityEngine;

    public sealed class OrbitProjectileAbility : Ability
    {
        public OrbitProjectileAbility(IProjectileFactoryManager projectileFactoryManager, Transform playerTransform)
        {
            _projectileFactoryManager = projectileFactoryManager;
            _playerTransform = playerTransform;
        }

        private IProjectileFactoryManager _projectileFactoryManager;
        private Transform _playerTransform;
        private List<ProjectileFacade> _activeProjectiles = new();
        private float _currentAngle;
        private float _currentCooldown;
        private bool _isInitialized;
        private int _targetProjectileCount;
        private float _currentDamage;
        private float _currentSpeed;

        protected override void OnInitialize()
        {
            if (!_isInitialized)
            {
                _currentCooldown = 0f;
                _currentAngle = 0f;
                _isInitialized = true;
                _targetProjectileCount = Config.EntitiesCount;
                _currentDamage = Config.Damage;
                _currentSpeed = Config.Speed;
                CreateMissingProjectiles();
                StartUpdate();
            }
            else
            {
                _targetProjectileCount += 1;
                _currentDamage += Config.Damage;
                _currentSpeed *= 1.1f;
                _currentCooldown = 0f;
                CreateMissingProjectiles();
            }
        }

        protected override void OnUpdate(float deltaTime)
        {
            _currentCooldown -= deltaTime;

            if (_currentCooldown <= 0)
            {
                int currentCount = CountActiveProjectiles();
                
                if (currentCount < _targetProjectileCount)
                {
                    CreateProjectile();
                }
                
                _currentCooldown = Config.Cooldown;
            }

            _activeProjectiles.RemoveAll(p => p == null || !p.gameObject.activeInHierarchy);

            _currentAngle += _currentSpeed * deltaTime;

            for (int i = 0; i < _activeProjectiles.Count; i++)
            {
                if (_activeProjectiles[i] != null && _activeProjectiles[i].gameObject.activeInHierarchy)
                {
                    float angleStep = 360f / _activeProjectiles.Count;
                    float angle = _currentAngle + (i * angleStep);

                    Vector3 offset = new Vector3(
                        Mathf.Cos(angle * Mathf.Deg2Rad) * Config.Radius,
                        0,
                        Mathf.Sin(angle * Mathf.Deg2Rad) * Config.Radius
                    );

                    Vector3 targetPosition = _playerTransform.position + offset;
                    _activeProjectiles[i].SetPosition(targetPosition);
                }
            }
        }

        private int CountActiveProjectiles()
        {
            int count = 0;
            foreach (ProjectileFacade projectile in _activeProjectiles)
            {
                if (projectile != null && projectile.gameObject.activeInHierarchy)
                {
                    count++;
                }
            }
            return count;
        }

        private void CreateProjectile()
        {
            AbilityDescription projectileConfig = new AbilityDescription
            {
                EntityType = EntityType.OrbitAbility,
                AbilityConfig = new AbilityConfig
                {
                    Damage = _currentDamage,
                    Cooldown = Config.Cooldown,
                    Radius = Config.Radius,
                    Speed = _currentSpeed,
                    EntitiesCount = _targetProjectileCount
                }
            };

            ProjectileFacade projectile = _projectileFactoryManager.GetFactory(EntityType.OrbitAbility).Create(new ProjectileSpawnData
            (
                _playerTransform.position,
                _playerTransform,
                projectileConfig
            ));

            _activeProjectiles.Add(projectile);
        }

        private void CreateMissingProjectiles()
        {
            int currentCount = CountActiveProjectiles();
            int missingCount = _targetProjectileCount - currentCount;
            
            if (missingCount <= 0)
            {
                return;
            }
            
            for (int i = 0; i < missingCount; i++)
            {
                CreateProjectile();
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            foreach (ProjectileFacade projectile in _activeProjectiles)
            {
                if (projectile != null)
                {
                    projectile.ReturnToPool();
                }
            }

            _activeProjectiles.Clear();
        }
    }
}
