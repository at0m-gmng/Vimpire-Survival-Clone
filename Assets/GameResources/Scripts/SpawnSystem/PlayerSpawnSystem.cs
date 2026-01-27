namespace GameResources.Scripts.SpawnSystem
{
    using Data.Entities;
    using Facades;
    using Factories;
    using Signals;
    using UnityEngine;
    using Zenject;

    public sealed class PlayerSpawnSystem 
    {
        public PlayerSpawnSystem(SignalBus signalBus, PlayerFactory playerFactory)
        {
            _playerFactory = playerFactory;
            _signalBus = signalBus;
        }
        private readonly PlayerFactory _playerFactory;
        private readonly SignalBus _signalBus;

        public void StartSystem(Transform playerSpawnPoint, PlayerConfig playerConfig, AbilitiesConfig abilitiesConfig)
        {
            PlayerFacade facade = _playerFactory.Create(new PlayerSpawnData(playerSpawnPoint, playerConfig, abilitiesConfig));
            _signalBus.Fire(new PlayerCreatedSignal(facade.EntityTransform));
        }
    }
}