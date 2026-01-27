namespace GameResources.Scripts.MovementSystem
{
    using Data.Entities;
    using UnityEngine;

    public sealed class EnemyMovementController : BaseFollowingController
    {
        public EnemyMovementController(Transform transform, Transform targetPlayer, EnemyConfig enemiesConfig)
            : base(new FollowingData(transform: transform, targetTransform: targetPlayer, speed: enemiesConfig.MoveSpeed)) { }
    }
}