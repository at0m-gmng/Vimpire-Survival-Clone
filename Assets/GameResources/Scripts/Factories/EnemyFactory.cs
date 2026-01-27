namespace GameResources.Scripts.Factories
{
    using Data.Entities;
    using Facades;
    using UnityEngine;
    using Zenject;

    public sealed class EnemyFactory : PlaceholderFactory<EnemySpawnData, EnemyFacade>
    {
        
    }

    public class EnemySpawnData
    {
        public EnemySpawnData(Vector3 targetPosition, Transform targetPlayer, EnemyDescription enemiesDescription)
        {
            TargetPosition = targetPosition;
            TargetPlayer = targetPlayer;
            EnemiesDescription = enemiesDescription;
        }

        public readonly Vector3 TargetPosition;
        public readonly Transform TargetPlayer;
        public readonly EnemyDescription EnemiesDescription;
    }
}