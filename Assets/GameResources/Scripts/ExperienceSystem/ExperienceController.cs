namespace GameResources.Scripts.ExperienceSystem
{
    using System;
    using Facades;
    using UniRx;
    using UnityEngine;
    using UniRx.Triggers;

    public sealed class ExperienceController : IDisposable
    {
        public ExperienceController(Collider initTriggerCollider, Collider collectTriggerCollider)
        {
            _initTriggerCollider = initTriggerCollider;
            _collectTriggerCollider = collectTriggerCollider;
        }
        private readonly Collider _initTriggerCollider;
        private readonly Collider _collectTriggerCollider;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public void Start()
        {
            _initTriggerCollider.OnTriggerEnterAsObservable()
                .Subscribe(collider => 
                {
                    ICollectable collectable = collider.GetComponent<ICollectable>();
                    if (collectable != null)
                    {
                        collectable.InitializeCollect();
                    }
                })
                .AddTo(_disposables);

            _collectTriggerCollider.OnTriggerEnterAsObservable()
                .Subscribe(collider => 
                {
                    ICollectable collectable = collider.GetComponent<ICollectable>();
                    if (collectable != null)
                    {
                        collectable.Collect();
                    }
                })
                .AddTo(_disposables);
        }

        public void Dispose() => _disposables.Dispose();
    }
}