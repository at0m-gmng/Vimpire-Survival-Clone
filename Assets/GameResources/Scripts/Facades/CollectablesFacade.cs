namespace GameResources.Scripts.Facades
{
    using System;
    using Data.Entities;
    using Factories;
    using MovementSystem;
    using Signals;
    using UniRx;
    using UnityEngine;
    using Zenject;

    public sealed class CollectablesFacade : AbstractFacade<CollectableConfig>, ICollectable,
        IPoolable<CollectableSpawnData, IMemoryPool>
    {
        private IDisposable _updateSubscription = null;
        private IMemoryPool _pool;
        private Transform _targetPlayer;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _updateSubscription?.Dispose();
        }
        
        #region POOL
        
        public void OnSpawned(CollectableSpawnData spawnData, IMemoryPool pool)
        {
            _pool = pool;
            transform.position = spawnData.TargetPosition;
            EntityType = spawnData.CollectableDescription.EntityType;
            _config = spawnData.CollectableDescription.CollectableConfig;
            _targetPlayer = spawnData.TargetPlayer;
            
            _movementController = new BaseFollowingController(
                new FollowingData(EntityTransform, _targetPlayer, _config.CollectSpeed));
        }

        public void OnDespawned()
        {
            _updateSubscription?.Dispose();
            _updateSubscription = null;
            _pool = null;
        }

        public void ReturnToPool()
        {
            if (_pool != null)
            {
                _pool.Despawn(this);
            }
        }
        
        #endregion

        public void InitializeCollect()
        {
            if (_updateSubscription == null)
            {
                _updateSubscription = Observable.EveryUpdate()
                    .Subscribe(_ => UpdateMovement());
            }
        }

        private void UpdateMovement() => _movementController?.UpdateMovement();

        public void Collect()
        {
            _signalBus.Fire(new ExperienceCollectedSignal(_config.Experience));
            ReturnToPool();
        }
    }

    public interface ICollectable
    {
        public void InitializeCollect();
        public void Collect();
    }
}