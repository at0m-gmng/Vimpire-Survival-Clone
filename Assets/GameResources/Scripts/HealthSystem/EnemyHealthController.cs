namespace GameResources.Scripts.HealthSystem
{
    using System;
    using Data.Entities;
    using UnityEngine;

    public sealed class EnemyHealthController : MonoBehaviour, IDamageable
    {
        public event Action EntityDestroyed;
        public event Action<float> EntityDamaged;

        public float Health
        {
            get => _health;
            private set
            {
                if (_health > 0 && _health >= value)
                {
                    _health = value;
                }
                else
                {
                    _health = 0;
                }
                EntityDamaged?.Invoke(_health);
                if (_health <= 0)
                {
                    EntityDestroyed?.Invoke();
                }
            }
        }

        public float MaxHealth { get; private set; } = default;

        private EnemyConfig _config;
        private float _health = default;

        public void Initialize(EnemyConfig config)
        {
            _config = config;
            Reset();
        }

        public void TakeDamage(float damage) => Health -= damage;

        public void Reset()
        {
            _health = _config.Health;
            MaxHealth = _config.Health;
        }
    }
}
