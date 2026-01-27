namespace GameResources.Scripts.Facades
{
    using AttackSystem;
    using Data;
    using Data.Entities;
    using ExperienceSystem;
    using LevelSystem;
    using MovementSystem;
    using UnityEngine;
    using Zenject;

    public abstract class AbstractFacade<T>: MonoBehaviour where T : EntityConfig
    {
        [Inject]
        private void Construct(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }
        protected SignalBus _signalBus;
        
        public Transform EntityTransform => _entityGameObject.transform;
        
        public EntityType EntityType
        {
            get => _entityType;
            protected set => _entityType = value;
        }
        
        [SerializeField] protected EntityType _entityType;
        [SerializeField] protected GameObject _entityGameObject;

        protected AbstractMovementController _movementController;
        protected AbstractAttackController _attackController;
        protected AbstractExperienceController _experienceController;
        protected AbstractLevelController _levelController;
        protected T _config;

        protected virtual void OnDestroy()
        {
            _movementController = null;
            _attackController?.Dispose();
            _experienceController?.Dispose();
            _levelController?.Dispose();
        }
    }
}
