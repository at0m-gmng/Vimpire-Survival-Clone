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

    public sealed class GameInstaller : MonoInstaller
    {
        [Header("Prefabs")]
        [SerializeField] private PlayerPrefabsConfig _playerPrefabsConfig;
        [SerializeField] private EnemyPrefabsConfig _enemyPrefabsConfig;

        [Header("Configs")] 
        [SerializeField] private InputConfig _inputConfig = default;
        [SerializeField] private CameraConfig _cameraConfig = default;
        
        [Header("Pool Parents")] [SerializeField]
        private Transform _playerPool = default;
        [SerializeField] private Transform _enemiesPool = default;

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
            Container.BindFactory<PlayerSpawnData, PlayerFacade, PlayerFactory>()
                .FromMonoPoolableMemoryPool(x => x.WithInitialSize(1)
                    .FromComponentInNewPrefab(_playerPrefabsConfig.PlayerFacade)
                    .UnderTransform(_playerPool));
            
            Container.BindFactory<EnemySpawnData, EnemyFacade, EnemyFactory>()
                .FromMonoPoolableMemoryPool(x => x.WithInitialSize(20)
                    .FromComponentInNewPrefab(_enemyPrefabsConfig.EnemyFacade)
                    .UnderTransform(_enemiesPool));
        }

        private void BindSystems()
        {
            Container.BindInterfacesTo<SaveLoadSystem>().AsSingle();
            Container.BindInterfacesTo<InputSystem>().AsSingle();
            Container.BindInterfacesTo<CameraSystem>().AsSingle();
            Container.Bind<PlayerSpawnSystem>().AsSingle();
            Container.BindInterfacesAndSelfTo<EnemySpawnSystem>().AsSingle();
        }
    }
}
