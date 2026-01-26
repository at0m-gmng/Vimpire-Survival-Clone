namespace GameResources.Scripts.InputSystem
{
    using UnityEngine;
    using UnityEngine.InputSystem;

    [CreateAssetMenu(menuName = "Configs/InputConfig", fileName = "InputConfig")]
    public sealed class InputConfig : ScriptableObject
    {
        [Header("Movement")]
        [field: SerializeField] public InputActionReference Movement { get; private set; }
    }
}