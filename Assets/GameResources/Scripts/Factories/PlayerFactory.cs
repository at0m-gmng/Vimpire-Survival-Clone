namespace GameResources.Scripts.Factories
{
    using Data.Entities;
    using Facades;
    using UnityEngine;
    using Zenject;

    public sealed class PlayerFactory : PlaceholderFactory<PlayerSpawnData, PlayerFacade>
    {
        
    }
    
    public class PlayerSpawnData
    {
        public PlayerSpawnData(Transform targetTransform, PlayerConfig playerConfig, AbilitiesConfig abilitiesConfig)
        {
            TargetTransform = targetTransform;
            PlayerConfig = playerConfig;
            AbilitiesConfig = abilitiesConfig;
        }

        public readonly Transform TargetTransform;
        public readonly PlayerConfig PlayerConfig;
        public readonly AbilitiesConfig AbilitiesConfig;
    }
}