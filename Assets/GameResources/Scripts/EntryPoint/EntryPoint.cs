namespace GameResources.Scripts.EntryPoint
{
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using Data;
    using SaveLoadSystem;
    using Signals;
    using SpawnSystem;
    using UnityEngine;
    using Zenject;

    public sealed class EntryPoint : MonoBehaviour
    {
        [Inject]
        private void Construct(SignalBus signalBus, ISaveLoadSystem saveLoadSystem, 
            PlayerSpawnSystem playerSpawnSystem, EnemySpawnSystem enemySpawnSystem, 
            CollectablesSpawnSystem collectablesSpawnSystem)
        {
            _signalBus = signalBus;
            _saveLoadSystem = saveLoadSystem;
            _playerSpawnSystem = playerSpawnSystem;
            _enemySpawnSystem = enemySpawnSystem;
            _collectablesSpawnSystem = collectablesSpawnSystem;
        }

        private ISaveLoadSystem _saveLoadSystem;
        private SignalBus _signalBus;
        private GameConfigs _gameConfigs = new();
        private PlayerSpawnSystem _playerSpawnSystem;
        private EnemySpawnSystem _enemySpawnSystem;
        private CollectablesSpawnSystem _collectablesSpawnSystem;

        [SerializeField] private List<Transform> _enemyPositions = default;
        [SerializeField] private BoxCollider _boxCollider = default;

        private async void Start()
        {
            (bool success, GameConfigs gameConfigs) = await _saveLoadSystem.TryLoadDataAsync<GameConfigs>("GameConfigs.json");
            if (success)
            {
                _gameConfigs = gameConfigs;
            }
            else
            {
                Debug.LogWarning("Failed to load GameConfigs, using default values");
            }
            
            _signalBus.Fire(new GameConfigLoadSignal(_gameConfigs));
            
            _playerSpawnSystem.StartSystem(transform, _gameConfigs.PlayerConfig, _gameConfigs.AbilitiesConfig);
            await UniTask.Delay(100);
            _enemySpawnSystem.StartSystem(GetTopFaceVertices(20), _gameConfigs.EnemiesConfig, 20, 0.15f);
            _collectablesSpawnSystem.StartSystem(_gameConfigs.CollectablesConfig);
        }
        
        private List<Vector3> GetTopFaceVertices(int pointsPerAxis)
        {
            List<Vector3> vertices = new();
            Vector3 center = _boxCollider.center;
            Vector3 size = _boxCollider.size;
    
            float halfX = size.x * 0.5f;
            float halfZ = size.z * 0.5f;
            float topY = size.y * 0.5f;
    
            for (int i = 0; i < pointsPerAxis; i++)
            {
                float tX = i / (float)(pointsPerAxis - 1);
                float xCoord = Mathf.Lerp(-halfX, halfX, tX);
        
                for (int j = 0; j < pointsPerAxis; j++)
                {
                    float tZ = j / (float)(pointsPerAxis - 1);
                    float zCoord = Mathf.Lerp(-halfZ, halfZ, tZ);
            
                    Vector3 localPoint = center + new Vector3(xCoord, topY, zCoord);
                    vertices.Add(_boxCollider.transform.TransformPoint(localPoint));
                }
            }
    
            return vertices;
        }
    }
}