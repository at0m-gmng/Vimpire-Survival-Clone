namespace GameResources.Scripts.AbilitySystem
{
    using System;
    using HealthSystem;
    using UniRx;
    using UniRx.Triggers;
    using UnityEngine;

    public sealed class TriggerDamageAbility : Ability
    {
        public TriggerDamageAbility(Collider triggerCollider, LayerMask targetLayerMask)
        {
            _triggerCollider = triggerCollider;
            _targetLayerMask = targetLayerMask;
        }
        private readonly Collider _triggerCollider;
        private readonly LayerMask _targetLayerMask;
        private CompositeDisposable _disposables;

        public event Action EntityDamaged;

        protected override void OnInitialize()
        {
            _disposables = new CompositeDisposable();
        
            _triggerCollider.OnTriggerEnterAsObservable()
                .Subscribe(OnTriggerEnter)
                .AddTo(_disposables);
        }

        private void OnTriggerEnter(Collider other)
        {
            int layerMask = 1 << other.gameObject.layer;
            if ((_targetLayerMask.value & layerMask) != 0)
            {
                IDamageable damageable = other.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(Config.Damage);
                    EntityDamaged?.Invoke();
                }
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            _disposables?.Dispose();
        }
    }
}