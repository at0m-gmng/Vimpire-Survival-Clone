namespace GameResources.Scripts.Signals
{
    using UnityEngine;

    public sealed class PlayerCreatedSignal
    {
        public PlayerCreatedSignal(Transform transform)
        {
            Transform = transform;
        }
        public readonly Transform Transform;
    }
}