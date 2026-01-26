namespace GameResources.Scripts.Signals
{
    using UnityEngine;

    public class PlayerCreatedSignal
    {
        public PlayerCreatedSignal(Transform transform)
        {
            Transform = transform;
        }

        public readonly Transform Transform;
    }
}