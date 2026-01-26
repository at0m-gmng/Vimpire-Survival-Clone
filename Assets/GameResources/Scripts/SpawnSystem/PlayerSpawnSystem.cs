namespace GameResources.Scripts.SpawnSystem
{
    using Configs.Entities;
    using Facades;
    using Factories;
    using Signals;
    using UnityEngine;
    using Zenject;

    public class PlayerSpawnSystem 
    {
        public PlayerSpawnSystem(SignalBus signalBus, PlayerFactory playerFactory)
        {
            _playerFactory = playerFactory;
            _signalBus = signalBus;
        }
        
        private readonly PlayerFactory _playerFactory;
        private readonly SignalBus _signalBus;

        public virtual void StartSystem(Transform playerSpawnPoint, PlayerConfig playerConfig)
        {
            PlayerFacade facade = _playerFactory.Create(new PlayerSpawnData(playerSpawnPoint, playerConfig));
            _signalBus.Fire(new PlayerCreatedSignal(facade.EntityTransform));
        }
    }
}