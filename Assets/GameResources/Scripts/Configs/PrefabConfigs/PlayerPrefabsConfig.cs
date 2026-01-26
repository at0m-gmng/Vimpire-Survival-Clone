namespace GameResources.Scripts.Configs.PrefabConfigs
{
    using Facades;
    using UnityEngine;

    [CreateAssetMenu(fileName = nameof(PlayerPrefabsConfig), menuName = "Configs/PrefabConfigs/PlayerPrefabsConfig")]
    public class PlayerPrefabsConfig : ScriptableObject
    {
        [field: SerializeField] public PlayerFacade PlayerFacade { get; private set; }
    }
}