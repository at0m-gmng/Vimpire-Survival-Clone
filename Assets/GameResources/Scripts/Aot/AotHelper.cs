namespace GameResources.Scripts.Aot
{
    using AbilitySystem;
    using CameraSystem;
    using Configs.PrefabConfigs;
    using Data;
    using Data.Entities;
    using ExperienceSystem;
    using Factories;
    using InputSystem;
    using LevelUpSystem;
    using MovementSystem;
    using SaveLoadSystem;
    using Signals;
    using SpawnSystem;
    using UnityEngine;
    using Zenject;

    public sealed class AotHelper : MonoBehaviour
    {
        [Header("Prefabs")]
        [SerializeField] private PlayerPrefabsConfig _playerPrefabsConfig;
        [SerializeField] private EnemyPrefabsConfig _enemyPrefabsConfig;
        [SerializeField] private CollectablePrefabsConfig _collectablePrefabsConfig;
        [SerializeField] private ProjectilePrefabsConfig _projectilePrefabsConfig;

        [Header("Configs")] 
        [SerializeField] private InputConfig _inputConfig = default;
        [SerializeField] private CameraConfig _cameraConfig = default;
        
        [Header("Pool Parents")] [SerializeField]
        private Transform _playerPool = default;
        [SerializeField] private Transform _enemiesPool = default;
        [SerializeField] private Transform _collectablesPool = default;
        [SerializeField] private Transform _projectilePool = default;

        private void Start()
        {
            DiContainer diContainer = new DiContainer();
            ZenjectManagersInstaller managerInstaller = new();
            SignalBusInstaller signalBusInstaller = new SignalBusInstaller();
            SignalDeclaration signalDeclaration = new SignalDeclaration(default, default);
            SignalSubscription signalSubscription = new SignalSubscription(new SignalSubscription.Pool());
            SceneContextRegistry sceneContextRegistry = new();
            GameObjectContext goContext = new();
            SignalDeclarationAsyncInitializer signalDeclarationAsyncInitializer = new SignalDeclarationAsyncInitializer(default, default);
            SceneContext sceneContext = new SceneContext();
            SceneContextRegistryAdderAndRemover sceneContextRegistryAdderAndRemover = new(sceneContext, sceneContextRegistry);

            SaveLoadSystem saveLoadSystem = new();
            ISaveLoadSystem saveLoadInterface = saveLoadSystem;

            CollectablesFactory collectablesFactory = new CollectablesFactory();
            EnemyFactory enemyFactory = new EnemyFactory();
            ProjectileFactory projectileFactory = new ProjectileFactory();
            
            LayerMask layerMask = new();
            Collider collider = new BoxCollider();
            GameObject gameObject = new();
            Transform transform = new GameObject().transform;
            SignalBus signalBus = new SignalBus(default, default, default, default, default, default);
            
            AuraDamageAbility auraDamageAbility = new(collider, gameObject, layerMask);
            OrbitProjectileAbility orbitProjectileAbility = new(null, transform);
            ProjectileAbility projectileAbility = new(null, collider, transform);
            RangeAttackAbility rangeAttackAbility = new(transform, gameObject, layerMask);
            TriggerDamageAbility triggerDamageAbility = new(collider, layerMask);
            
            CameraConfig cameraConfig = new();
            CameraSystem cameraSystem = new(signalBus, cameraConfig);
            
            AbilitiesConfig abilitiesConfig = new();
            AbilityDescription abilityDescription = new();
            AbilityConfig abilityConfig = new();
            
            CollectablesConfig collectablesConfig = new();
            CollectableDescription collectableDescription = new();
            CollectableConfig collectableConfig = new();
            
            EnemiesConfig enemiesConfig = new();
            EnemyDescription enemyDescription = new();
            EnemyConfig enemyConfig = new();
            
            PlayerConfig playerConfig = new();
            
            RewardConfig rewardConfig = new();
            RewardDescription rewardDescription = new();
            
            EntityType entityType = default;
            GameConfigs gameConfigs = new();
            
            ExperienceController experienceController = new(collider, collider);
            
            CollectablePrefabsConfig collectablePrefabsConfig = new();
            CollectablesFactoryManager collectablesFactoryManager = new(diContainer, collectablePrefabsConfig);
            ICollectableFactoryManager collectableFactoryManagerInterface = collectablesFactoryManager;
            
            EnemyPrefabsConfig enemyPrefabsConfig = new();
            EnemyEnemyFactoryManager enemyEnemyFactoryManager = new(diContainer, enemyPrefabsConfig);
            IEnemyFactoryManager enemyFactoryManagerInterface = enemyEnemyFactoryManager;
            
            ProjectilePrefabsConfig projectilePrefabsConfig = new();
            ProjectileFactoryManager projectileFactoryManager = new(diContainer, projectilePrefabsConfig);
            IProjectileFactoryManager projectileFactoryManagerInterface = projectileFactoryManager;
            
            InputConfig inputConfig = new();
            InputSystem inputSystem = new(inputConfig);
            IInputSystem inputSystemInterface = inputSystem;
            
            LevelUpSystem levelUpSystem = new(signalBus);
            
            FollowingData followingData = new(transform, transform, 0f);
            BaseFollowingController baseFollowingController = new(followingData);
            EnemyMovementController enemyMovementController = new(transform, transform, enemyConfig);
            PlayerMovementController playerMovementController = new(transform, playerConfig, inputSystemInterface);
            ShootMovementData shootMovementData = new(transform, Vector3.zero, 0f);
            ShootMovementController shootMovementController = new(shootMovementData);
            
            EntityKilledSignal entityKilledSignal = new(EntityType.Player, EntityType.Player, Vector3.zero);
            ExperienceCollectedSignal experienceCollectedSignal = new(0f);
            ExperienceProgressChangedSignal experienceProgressChangedSignal = new(0f, 0);
            GameConfigLoadSignal gameConfigLoadSignal = new(gameConfigs);
            LevelUpSignal levelUpSignal = new(rewardDescription);
            PlayerCreatedSignal playerCreatedSignal = new(transform);
            PlayerDestroyedSignal playerDestroyedSignal = new();
            RewardSelectedSignal rewardSelectedSignal = new(EntityType.Player);
            
            CollectablesSpawnSystem collectablesSpawnSystem = new(signalBus, collectableFactoryManagerInterface);
            EnemySpawnSystem enemySpawnSystem = new(signalBus, enemyFactoryManagerInterface);
            PlayerFactory playerFactory = new();
            PlayerSpawnSystem playerSpawnSystem = new(signalBus, playerFactory);
        }
    }
}