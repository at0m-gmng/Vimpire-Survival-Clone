namespace GameResources.Scripts.AbilitySystem
{
    using System;
    using Data.Entities;
    using UniRx;
    using UnityEngine;

    public abstract class Ability : IDisposable
    {
        protected AbilityConfig Config => _configDescription.AbilityConfig;
        protected AbilityDescription _configDescription;
        protected IDisposable _updateSubscription;

        public void Initialize(AbilityDescription config)
        {
            _configDescription = config;
            OnInitialize();
        }

        protected void StartUpdate()
        {
            if (_updateSubscription == null)
            {
                _updateSubscription = Observable.EveryUpdate()
                    .Subscribe(_ => OnUpdate(Time.deltaTime));
            }
        }

        protected virtual void OnInitialize() { }
        protected virtual void OnUpdate(float deltaTime) { }
    
        public virtual void Dispose() => _updateSubscription?.Dispose();
    }
}