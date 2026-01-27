namespace GameResources.Scripts.SpawnSystem
{
    using Data.Entities;
    using Factories;
    using Signals;
    using UnityEngine;
    using Zenject;

    public sealed class CollectablesSpawnSystem
    {
        public CollectablesSpawnSystem(SignalBus signalBus, ICollectableFactoryManager collectableFactory)
        {
            _collectableFactory = collectableFactory;
            _signalBus = signalBus;

            _signalBus.Subscribe<PlayerCreatedSignal>(OnPlayerCreated);
            _signalBus.Subscribe<EntityKilledSignal>(OnEntityKilled);
        }
        private readonly SignalBus _signalBus;
        private readonly ICollectableFactoryManager _collectableFactory;

        private Transform _playerTarget;
        private CollectablesConfig _collectablesConfig;

        public void StartSystem(CollectablesConfig collectablesConfig) => _collectablesConfig = collectablesConfig;

        private void OnPlayerCreated(PlayerCreatedSignal signal) => _playerTarget = signal.Transform;

        private void OnEntityKilled(EntityKilledSignal entityKilledSignal)
        {
            CollectableDescription collectableDescription =
                _collectablesConfig.CollectablesDescription.Find(x => x.EntityType == entityKilledSignal.RewardType);

            if (collectableDescription != null)
            {
                _collectableFactory.GetFactory(collectableDescription.EntityType).Create(
                    new CollectableSpawnData(
                        entityKilledSignal.Position,
                        _playerTarget,
                        collectableDescription
                    ));
            }
        }

    }
}