namespace GameResources.Scripts.CameraSystem
{
    using UniRx;
    using UnityEngine;
    using Zenject;
    using System;
    using Signals;

    public sealed class CameraSystem : IInitializable, IDisposable
    {
        public CameraSystem(SignalBus signalBus, CameraConfig config)
        {
            _signalBus = signalBus;
            _config = config;
            _camera = Camera.main;
        }
        private readonly SignalBus _signalBus;
        private readonly CameraConfig _config;
        private readonly Camera _camera;
        
        private Transform _target;
        private readonly CompositeDisposable _disposables = new();
        
        public void Initialize()
        {
            _signalBus.Subscribe<PlayerCreatedSignal>(s => _target = s.Transform);
            _signalBus.Subscribe<PlayerDestroyedSignal>(_ => _target = null);
                
            Observable.EveryLateUpdate()
                .Where(_ => _target != null)
                .Subscribe(_ => UpdateCameraPosition())
                .AddTo(_disposables);
        }
        
        private void UpdateCameraPosition()
        {
            Vector3 position = _target.position + _config.Offset;
            _camera.transform.position = position;
            _camera.transform.rotation = Quaternion.Euler(_config.Tilt, _config.Rotation, 0);
        }
        
        public void Dispose() => _disposables.Clear();
    }
}