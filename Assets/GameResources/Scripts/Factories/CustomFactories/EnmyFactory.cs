namespace GameResources.Scripts.Factories.CustomFactories
{
    using Facades;
    using Zenject;

    public class EnmyFactory : PlaceholderFactory<EnemySpawnData, EnemyFacade>
    {
        private readonly EnemyPool _enemyPool;

        public EnmyFactory(EnemyPool enemyPool)
        {
            _enemyPool = enemyPool;
        }

        public EnemyFacade Create(EnemySpawnData data) => _enemyPool.SpawnEnemy(data);
    }
}