// TurretBaseAuthoring.cs
using Unity.Entities;
using UnityEngine;

public class TurretBaseAuthoring : MonoBehaviour
{
    [Header("Hierarchy")]
    public GameObject MountObject;
    public GameObject BarrelObject;
    public GameObject FirePointObject;

    [Header("Health")]
    public float MaxHealth = 200f;

    [Header("Weapon")]
    public float Range              = 15f;
    public float FireRate           = 1.5f;
    public float Damage             = 25f;
    public float ProjectileSpeed    = 20f;
    public float MountRotationSpeed = 120f;

    public GameObject ProjectilePrefab;

    class Baker : Baker<TurretBaseAuthoring>
    {
        public override void Bake(TurretBaseAuthoring authoring)
        {
            if (authoring.MountObject    == null ||
                authoring.BarrelObject   == null ||
                authoring.FirePointObject == null)
            {
                Debug.LogError($"TurretBaseAuthoring on '{authoring.name}': " +
                               "All hierarchy slots must be assigned.");
                return;
            }

            var baseEntity      = GetEntity(TransformUsageFlags.Dynamic);
            var mountEntity     = GetEntity(authoring.MountObject,     TransformUsageFlags.Dynamic);
            var barrelEntity    = GetEntity(authoring.BarrelObject,    TransformUsageFlags.Dynamic);
            var firePointEntity = GetEntity(authoring.FirePointObject, TransformUsageFlags.Dynamic);

            AddComponent(baseEntity, new TurretBaseTag());
            AddComponent(baseEntity, new HealthComponent
            {
                Current = authoring.MaxHealth,
                Max     = authoring.MaxHealth
            });
            AddComponent(baseEntity, new TargetComponent());
            AddComponent(baseEntity, new TurretWeaponComponent
            {
                Range               = authoring.Range,
                FireRate            = authoring.FireRate,
                Damage              = authoring.Damage,
                ProjectileSpeed     = authoring.ProjectileSpeed,
                MountRotationSpeed  = authoring.MountRotationSpeed,
                TimeSinceLastShot   = 0f,
                ProjectilePrefab    = authoring.ProjectilePrefab != null
                    ? GetEntity(authoring.ProjectilePrefab, TransformUsageFlags.Dynamic)
                    : Entity.Null
            });
            AddComponent(baseEntity, new TurretPartComponent
            {
                BaseEntity      = baseEntity,
                MountEntity     = mountEntity,
                FirePointEntity = firePointEntity
            });

            // All parts destroyed together when base is destroyed
            var linkedGroup = AddBuffer<LinkedEntityGroup>(baseEntity);
            linkedGroup.Add(new LinkedEntityGroup { Value = baseEntity      });
            linkedGroup.Add(new LinkedEntityGroup { Value = mountEntity     });
            linkedGroup.Add(new LinkedEntityGroup { Value = barrelEntity    });
            linkedGroup.Add(new LinkedEntityGroup { Value = firePointEntity });
        }
    }
}