namespace GameResources.Scripts.Facades
{
    using AttackSystem;
    using Configs.Entities;
    using ExperienceSystem;
    using LevelSystem;
    using MovementSystem;
    using UnityEngine;
    using Zenject;

    public abstract class AbstractFacade<T>: MonoBehaviour
        where T : EntityConfig
    {
        public Transform EntityTransform => _entityGameObject.transform;
        
        [SerializeField] protected EntityType _entityType;
        [SerializeField] protected GameObject _entityGameObject;

        protected AbstractMovementController _movementController;
        protected AbstractAttackController _attackController;
        protected AbstractExperienceController _experienceController;
        protected AbstractLevelController _levelController;
    
        protected SignalBus _signalBus;
        protected T _config;

        public EntityType EntityType => _entityType;

        [Inject]
        private void Construct(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        protected virtual void OnDestroy()
        {
            _movementController = null;
            _attackController?.Dispose();
            _experienceController?.Dispose();
            _levelController?.Dispose();
        }
    }
}
