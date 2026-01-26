namespace GameResources.Scripts.Configs.PrefabConfigs
{
    using Facades;
    using UnityEngine;

    [CreateAssetMenu(fileName = nameof(EnemyPrefabsConfig), menuName = "Configs/PrefabConfigs/EnemyPrefabsConfig")]
    public class EnemyPrefabsConfig : ScriptableObject
    {
        [field: SerializeField] public EnemyFacade EnemyFacade { get; private set; }
    }
}