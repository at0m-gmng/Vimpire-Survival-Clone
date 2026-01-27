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

        protected IDisposable UpdateSubscription;

        public void Initialize(AbilityDescription config)
        {
            _configDescription = config;
            OnInitialize();
        }

        protected void StartUpdate()
        {
            UpdateSubscription?.Dispose();
            if (UpdateSubscription == null)
            {
                UpdateSubscription = Observable.EveryUpdate()
                    .Subscribe(_ => OnUpdate(Time.deltaTime));
            }
        }

        protected virtual void OnInitialize() { }
        protected virtual void OnUpdate(float deltaTime) { }
    
        public virtual void Dispose() => UpdateSubscription?.Dispose();
    }
}