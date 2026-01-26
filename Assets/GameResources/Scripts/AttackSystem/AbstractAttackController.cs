namespace GameResources.Scripts.AttackSystem
{
    using System;

    public abstract class AbstractAttackController : IDisposable
    {
        public abstract void Start();
        public abstract void Dispose();
    }
}
