namespace GameResources.Scripts.MovementSystem
{
    using UnityEngine;

    public class BaseFollowingController : AbstractMovementController
    {
        public BaseFollowingController(FollowingData followingData)
        {
            _transform = followingData.Transform;
            _target = followingData.TargetTransform;
            _moveSpeed = followingData.Speed;
        }
        protected readonly Transform _transform;
        protected readonly Transform _target;
        protected readonly float _moveSpeed;

        public override void UpdateMovement()
        {
            if (_target != null)
            {
                Vector3 direction = (_target.position - _transform.position).normalized;
                _transform.position += direction * _moveSpeed * Time.deltaTime;
            }
        }
    }

    public class FollowingData
    {
        public FollowingData(Transform transform, Transform targetTransform, float speed)
        {
            Transform = transform;
            TargetTransform = targetTransform;
            Speed = speed;
        }
        public readonly Transform Transform;
        public readonly Transform TargetTransform;
        public readonly float Speed;
    }
}