namespace GameResources.Scripts.MovementSystem
{
    using System;
    using Data.Entities;
    using InputSystem;
    using UniRx;
    using UnityEngine;

    public sealed class PlayerMovementController : AbstractMovementController, IDisposable
    {
        public PlayerMovementController(Transform transform, PlayerConfig playerConfig, IInputSystem inputSystem)
        {
            _transform = transform;
            _playerConfig = playerConfig;
            inputSystem.Movement.Subscribe(value => _movementInput = value).AddTo(_disposables);
        }
        private readonly Transform _transform;
        private readonly PlayerConfig _playerConfig;
        
        private Vector2 _movementInput;
        private readonly CompositeDisposable _disposables = new();

        public override void UpdateMovement()
        {
            Vector3 movement = new Vector3(_movementInput.x, 0f, _movementInput.y).normalized;
            Vector3 moveDirection = _transform.TransformDirection(movement);
            _transform.position += moveDirection * _playerConfig.MoveSpeed * Time.deltaTime;
        }

        public void Dispose() => _disposables.Clear();
    }
}
