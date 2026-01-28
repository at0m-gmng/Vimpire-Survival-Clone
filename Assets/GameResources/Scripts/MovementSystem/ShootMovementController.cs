namespace GameResources.Scripts.MovementSystem
{
    using UnityEngine;

    public sealed class ShootMovementController : AbstractMovementController
    {
        public ShootMovementController(ShootMovementData shootData)
        {
            _transform = shootData.Transform;
            _direction = shootData.Direction;
            _moveSpeed = shootData.Speed;
        }

        private readonly Transform _transform;
        private readonly Vector3 _direction;
        private readonly float _moveSpeed;

        public override void UpdateMovement()
        {
            _transform.position += _direction * _moveSpeed * Time.deltaTime;
        }
    }

    public class ShootMovementData
    {
        public ShootMovementData(Transform transform, Vector3 direction, float speed)
        {
            Transform = transform;
            Direction = direction.normalized;
            Speed = speed;
        }

        public readonly Transform Transform;
        public readonly Vector3 Direction;
        public readonly float Speed;
    }
}
