namespace GameResources.Scripts.AbilitySystem
{
    using System;
    using Data.Entities;
    using UniRx;
    using UnityEngine;

    public abstract class Ability : IDisposable
    {
        protected AbilityConfig Config { get; private set; }
        protected IDisposable UpdateSubscription;

        public void Initialize(AbilityConfig config)
        {
            Config = config;
            OnInitialize();
        }

        protected void StartUpdate()
        {
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