// TurretAuthoring.cs
using Unity.Entities;
using UnityEngine;

public class TurretAuthoring : MonoBehaviour
{
    public GameObject MountObject;   // drag in Inspector
    public GameObject BarrelObject;  // drag in Inspector
    public float MaxHealth   = 200f;
    public float Range       = 15f;
    public float FireRate    = 1.5f;
    public float Damage      = 25f;
    public float ProjectileSpeed = 20f;
    public GameObject ProjectilePrefab;

    class Baker : Baker<TurretAuthoring>
    {
        public override void Bake(TurretAuthoring authoring)
        {
            var baseEntity   = GetEntity(TransformUsageFlags.Dynamic);
            var mountEntity  = GetEntity(authoring.MountObject,  TransformUsageFlags.Dynamic);
            var barrelEntity = GetEntity(authoring.BarrelObject, TransformUsageFlags.Dynamic);

            // ── Base ──
            AddComponent(baseEntity, new TurretBaseTag());
            AddComponent(baseEntity, new HealthComponent { Current = authoring.MaxHealth, Max = authoring.MaxHealth });
            AddComponent(baseEntity, new TargetComponent());
            AddComponent(baseEntity, new TurretWeaponComponent
            {
                Range           = authoring.Range,
                FireRate        = authoring.FireRate,
                Damage          = authoring.Damage,
                ProjectileSpeed = authoring.ProjectileSpeed,
                ProjectilePrefab = GetEntity(authoring.ProjectilePrefab, TransformUsageFlags.Dynamic)
            });
            AddComponent(baseEntity, new TurretPartComponent { BaseEntity = baseEntity, MountEntity = mountEntity });

            // ── Mount ──
            AddComponent(mountEntity, new TurretMountTag());
            AddComponent(mountEntity, new TurretPartComponent { BaseEntity = baseEntity, MountEntity = mountEntity });

            // ── Barrel ──
            AddComponent(barrelEntity, new TurretBarrelTag());
            AddComponent(barrelEntity, new TurretPartComponent { BaseEntity = baseEntity, MountEntity = mountEntity });

            // LinkedEntityGroup tells ECB.DestroyEntity to also destroy all children
            // Unity sets this up automatically if you use GetEntity on child GameObjects
            // but we make it explicit for safety:

            var linkedGroup = AddBuffer<LinkedEntityGroup>(baseEntity);
            linkedGroup.Add(new LinkedEntityGroup { Value = baseEntity });
            linkedGroup.Add(new LinkedEntityGroup { Value = mountEntity });
            linkedGroup.Add(new LinkedEntityGroup { Value = barrelEntity });
        }
    }
}