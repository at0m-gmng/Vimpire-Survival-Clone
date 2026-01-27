namespace GameResources.Scripts.HealthSystem
{
    using System;
    using Data.Entities;
    using UnityEngine;

    public sealed class EnemyHealthController : MonoBehaviour, IDamageable
    {
        public event Action EntityDestroyed;
        public event Action<float> EntityDamaged;
        
        private EnemyConfig _config;

        public bool IsAlive => Health > 0;

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
                EntityDamaged?.Invoke(value);
                if (_health <= 0)
                {
                    EntityDestroyed?.Invoke();
                }
            }
        }

        private float _health = default;

        public float MaxHealth { get; private set; } = default;
        
        public void Initialize(EnemyConfig config)
        {
            _config = config;
            Reset();
        }

        public void Initialize(EnemiesConfig config) => throw new NotImplementedException();

        public void TakeDamage(int damage) => Health -= damage;

        public void Reset()
        {
            _health = _config.Health;
            MaxHealth = _config.Health;
        }
    }
}
