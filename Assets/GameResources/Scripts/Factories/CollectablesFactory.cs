namespace GameResources.Scripts.Factories
{
    using Data.Entities;
    using Facades;
    using UnityEngine;
    using Zenject;

    public sealed class CollectablesFactory : PlaceholderFactory<CollectableSpawnData, CollectablesFacade>
    {
        
    }
    
    public class CollectableSpawnData
    {
        public CollectableSpawnData(Vector3 targetPosition, Transform targetPlayer, CollectableDescription collectableDescription)
        {
            TargetPosition = targetPosition;
            TargetPlayer = targetPlayer;
            CollectableDescription = collectableDescription;
        }

        public readonly Vector3 TargetPosition;
        public readonly Transform TargetPlayer;
        public readonly CollectableDescription CollectableDescription;
    }
}