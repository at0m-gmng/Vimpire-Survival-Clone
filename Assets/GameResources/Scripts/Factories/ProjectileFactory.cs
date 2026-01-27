namespace GameResources.Scripts.Factories
{
    using Data.Entities;
    using Facades;
    using UnityEngine;
    using Zenject;

    public sealed class ProjectileFactory: PlaceholderFactory<ProjectileSpawnData, ProjectileFacade>
    {
        
    }

    public class ProjectileSpawnData
    {
        public ProjectileSpawnData(Vector3 targetPosition, Transform targetPlayer, AbilityDescription abilityDescription)
        {
            TargetPosition = targetPosition;
            TargetPlayer = targetPlayer;
            AbilityDescription = abilityDescription;
        }

        public readonly Vector3 TargetPosition;
        public readonly Transform TargetPlayer;
        public readonly AbilityDescription AbilityDescription;
    }
}