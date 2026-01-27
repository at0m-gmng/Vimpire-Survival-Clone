namespace GameResources.Scripts.Facades
{
    using System.Collections.Generic;
    using AbilitySystem;
    using Data;
    using Data.Entities;
    using ExperienceSystem;
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
        protected ExperienceController _experienceController;
        protected List<Ability> _abilities = new();
        protected T _config;

        protected virtual void OnDestroy()
        {
            _movementController = null;
            
            foreach (var ability in _abilities)
            {
                ability?.Dispose();
            }
            _abilities.Clear();
        }
    }
}
