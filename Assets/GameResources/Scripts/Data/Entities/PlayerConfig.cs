namespace GameResources.Scripts.Data.Entities
{
    using System;

    [Serializable]
    public class PlayerConfig : EntityConfig
    {
        public float Health = 10;
        public float MoveSpeed = 5f;
        public float AttackRange = 7.5f;
        public int AttackDamage = 5;
        public float AttackCooldown = 1.5f;
    }
}