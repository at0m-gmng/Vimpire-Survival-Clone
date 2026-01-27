namespace GameResources.Scripts.Factories
{
    using Data.Entities;
    using Facades;
    using UnityEngine;
    using Zenject;

    public class PlayerFactory : PlaceholderFactory<PlayerSpawnData, PlayerFacade>
    {
        
    }
    
    public class PlayerSpawnData
    {
        public PlayerSpawnData(Transform targetTransform, PlayerConfig playerConfig)
        {
            TargetTransform = targetTransform;
            PlayerConfig = playerConfig;
        }

        public readonly Transform TargetTransform;
        public readonly PlayerConfig PlayerConfig;
    }
}