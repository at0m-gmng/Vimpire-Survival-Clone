namespace GameResources.Scripts.InputSystem
{
    using System;
    using UniRx;
    using UnityEngine;
    using UnityEngine.InputSystem;

    public sealed class InputSystem : IInputSystem
    {
        public InputSystem(InputConfig config)
        {
            _movementAction = config.Movement.action;

            if (_movementAction == null)
            {
#if UNITY_EDITOR
                Debug.LogError("InputActionReference in config is null!");
#endif
                return;
            }

            _movementAction.performed += ctx => _drag.Execute(ctx.ReadValue<Vector2>());
            _movementAction.canceled += _ => _drag.Execute(Vector2.zero);
        }

        private readonly InputAction _movementAction;

        public IObservable<Vector2> Movement => _drag;

        private readonly ReactiveCommand<Vector2> _drag = new();

        public void Enable() => _movementAction?.Enable();

        public void Disable()
        {
            _movementAction?.Disable();
            _movementAction.performed -= _ => _drag.Execute(Vector2.zero);
            _movementAction.canceled -= _ => _drag.Execute(Vector2.zero);
        }
    }

    public interface IInputEvents
    {
        public IObservable<Vector2> Movement { get; }
    }

    public interface IInputSystem : IInputEvents
    {
        public void Enable();
        public void Disable();
    }
}