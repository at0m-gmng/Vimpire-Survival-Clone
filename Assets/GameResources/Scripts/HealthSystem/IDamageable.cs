namespace GameResources.Scripts.HealthSystem
{
    using System;
    using Configs.Entities;

    public interface IDamageable
    {
        public event Action EntityDestroyed;
        public event Action<float> EntityDamaged;
        
        public float MaxHealth { get; }
        public float Health { get; }
        public void Initialize(EnemyConfig config);
        public void TakeDamage(int damage);
        public void Reset();
    }
}
