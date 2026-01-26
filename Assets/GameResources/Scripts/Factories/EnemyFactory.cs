namespace GameResources.Scripts.Factories
{
    using Configs.Entities;
    using Facades;
    using UnityEngine;
    using Zenject;

    public class EnemyFactory : PlaceholderFactory<EnemySpawnData, EnemyFacade>
    {
        
    }

    public class EnemySpawnData
    {
        public EnemySpawnData(Vector3 targetPosition, Transform targetPlayer, EnemyConfig enemyConfig)
        {
            TargetPosition = targetPosition;
            TargetPlayer = targetPlayer;
            EnemyConfig = enemyConfig;
        }

        public readonly Vector3 TargetPosition;
        public readonly Transform TargetPlayer;
        public readonly EnemyConfig EnemyConfig;
    }
}