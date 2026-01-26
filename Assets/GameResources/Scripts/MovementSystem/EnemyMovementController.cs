namespace GameResources.Scripts.MovementSystem
{
    using Configs.Entities;
    using UnityEngine;

    public class EnemyMovementController : AbstractMovementController
    {
        public EnemyMovementController(Transform transform, Transform targetPlayer, EnemyConfig enemyConfig)
        {
            _transform = transform;
            _target = targetPlayer;
            _moveSpeed = enemyConfig.MoveSpeed;
        }
        private readonly Transform _transform;
        private readonly Transform _target;
        private readonly float _moveSpeed;

        public override void UpdateMovement()
        {
            if (_target != null)
            {
                Vector3 direction = (_target.position - _transform.position).normalized;
                _transform.position += direction * _moveSpeed * Time.deltaTime;
            }
        }
    }
}