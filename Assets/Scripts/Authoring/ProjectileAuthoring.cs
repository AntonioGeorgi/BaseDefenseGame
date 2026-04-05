// ProjectileAuthoring.cs
using Unity.Entities;
using UnityEngine;

public class ProjectileAuthoring : MonoBehaviour
{
    [Tooltip("How fast the projectile moves in world units per second")]
    public float Speed  = 20f;
    [Tooltip("How far it travels before despawning if it hits nothing")]
    public float MaxRange = 30f;

    // Damage and direction are set at spawn time by TurretFireSystem
    // because they depend on the firing turret, not the projectile prefab

    class Baker : Baker<ProjectileAuthoring>
    {
        public override void Bake(ProjectileAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ProjectileTag());
            AddComponent(entity, new ProjectileComponent
            {
                Speed    = authoring.Speed,
                MaxRange = authoring.MaxRange,
                // Direction and Damage are zero here — TurretFireSystem overwrites them
            });
        }
    }
}