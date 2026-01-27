namespace GameResources.Scripts.Installers
{
    using CameraSystem;
    using Configs.PrefabConfigs;
    using Facades;
    using Factories;
    using InputSystem;
    using SaveLoadSystem;
    using Signals;
    using SpawnSystem;
    using UnityEngine;
    using Zenject;
    using EnemyFactory = Factories.EnemyFactory;

    public sealed class GameInstaller : MonoInstaller
    {
        [Header("Prefabs")]
        [SerializeField] private PlayerPrefabsConfig _playerPrefabsConfig;
        [SerializeField] private EnemyPrefabsConfig _enemyPrefabsConfig;
        [SerializeField] private CollectablePrefabsConfig _collectablePrefabsConfig;

        [Header("Configs")] 
        [SerializeField] private InputConfig _inputConfig = default;
        [SerializeField] private CameraConfig _cameraConfig = default;
        
        [Header("Pool Parents")] [SerializeField]
        private Transform _playerPool = default;
        [SerializeField] private Transform _enemiesPool = default;
        [SerializeField] private Transform _collectablesPool = default;

        public override void InstallBindings()
        {
            BindConfigs();
            BindSignals();
            BindFactories();
            BindSystems();
        }

        private void BindConfigs()
        {
            Container.BindInstance(_inputConfig).AsSingle().IfNotBound();
            Container.BindInstance(_cameraConfig).AsSingle().IfNotBound();
            Container.BindInstance(_enemyPrefabsConfig).AsSingle().IfNotBound();
            Container.BindInstance(_collectablePrefabsConfig).AsSingle().IfNotBound();
        }

        private void BindSignals()
        {
            SignalBusInstaller.Install(Container);

            Container.DeclareSignal<ExperienceGainedSignal>();
            Container.DeclareSignal<LevelUpSignal>();
            Container.DeclareSignal<EntityKilledSignal>();
            Container.DeclareSignal<GameConfigLoadSignal>();
            Container.DeclareSignal<PlayerCreatedSignal>();
            Container.DeclareSignal<PlayerDestroyedSignal>();
        }

        private void BindFactories()
        {
            Container.BindInterfacesTo<EnemyEnemyFactoryManager>().AsSingle();
            Container.BindInterfacesTo<CollectablesFactoryManager>().AsSingle();
            
            Container.BindFactory<PlayerSpawnData, PlayerFacade, PlayerFactory>()
                .FromMonoPoolableMemoryPool(x => x.WithInitialSize(1)
                .FromComponentInNewPrefab(_playerPrefabsConfig.PlayerFacade)
                .UnderTransform(_playerPool));

            for (int i = 0; i < _enemyPrefabsConfig.EnemyPrefabs.Count; i++)
            {
                Container.BindFactory<EnemySpawnData, EnemyFacade, EnemyFactory>()
                    .WithId(_enemyPrefabsConfig.EnemyPrefabs[i].EntityType.ToString())
                    .FromMonoPoolableMemoryPool(x => x.WithInitialSize(10)
                    .FromComponentInNewPrefab(_enemyPrefabsConfig.EnemyPrefabs[i].EnemyFacade)
                    .UnderTransform(_enemiesPool));
            }
            
            for (int i = 0; i < _collectablePrefabsConfig.CollectablePrefabs.Count; i++)
            {
                Container.BindFactory<CollectableSpawnData, CollectablesFacade, CollectablesFactory>()
                    .WithId(_collectablePrefabsConfig.CollectablePrefabs[i].EntityType.ToString())
                    .FromMonoPoolableMemoryPool(x => x.WithInitialSize(20)
                    .FromComponentInNewPrefab(_collectablePrefabsConfig.CollectablePrefabs[i].CollectableFacade)
                    .UnderTransform(_collectablesPool));
            }
        }

        private void BindSystems()
        {
            Container.BindInterfacesTo<SaveLoadSystem>().AsSingle();
            Container.BindInterfacesTo<InputSystem>().AsSingle();
            Container.BindInterfacesTo<CameraSystem>().AsSingle();
            Container.Bind<PlayerSpawnSystem>().AsSingle();
            Container.BindInterfacesAndSelfTo<EnemySpawnSystem>().AsSingle();
            Container.Bind<CollectablesSpawnSystem>().AsSingle();
        }
    }
}
