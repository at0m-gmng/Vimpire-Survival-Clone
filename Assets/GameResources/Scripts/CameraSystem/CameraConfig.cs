namespace GameResources.Scripts.CameraSystem
{
    using UnityEngine;

    [CreateAssetMenu(fileName = nameof(CameraConfig), menuName = "Configs/CameraConfig")]
    public sealed class CameraConfig : ScriptableObject
    {
        [field: SerializeField] public Vector3 Offset { get; private set; } = new( 0, 10, 0);
        [field: SerializeField] public float Rotation{ get; private set; } = 0; 
        [field: SerializeField] public float Tilt { get; private set; } = 45f;
    }
}