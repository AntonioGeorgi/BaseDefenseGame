// TurretFireSystem.cs
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(TurretRotationSystem))]
public partial struct TurretFireSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<TurretBaseTag>();
    }

    public void OnUpdate(ref SystemState state)
    {
        float dt = SystemAPI.Time.DeltaTime;

        var l2wLookup       = SystemAPI.GetComponentLookup<LocalToWorld>(isReadOnly: true);
        var firePointLookup = SystemAPI.GetComponentLookup<FirePointComponent>(isReadOnly: true);

        // Map base entity → fire point entity using the new FirePointComponent tag
        var firePointMap = new NativeHashMap<Entity, Entity>(16, Allocator.Temp);

        foreach (var (part, entity) in
            SystemAPI.Query<RefRO<TurretPartComponent>>()
                     .WithAll<FirePointComponent>()
                     .WithEntityAccess())
        {
            firePointMap.TryAdd(part.ValueRO.BaseEntity, entity);
        }

        var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (weapon, target, part) in
            SystemAPI.Query<
                RefRW<TurretWeaponComponent>,
                RefRO<TargetComponent>,
                RefRO<TurretPartComponent>>()
                     .WithAll<TurretBaseTag>())
        {
            weapon.ValueRW.TimeSinceLastShot += dt;

            if (!target.ValueRO.HasTarget) continue;

            float fireInterval = 1f / math.max(weapon.ValueRO.FireRate, 0.01f);
            if (weapon.ValueRO.TimeSinceLastShot < fireInterval) continue;

            if (weapon.ValueRO.ProjectilePrefab == Entity.Null)
            {
                Debug.LogWarning("[TurretFireSystem] ProjectilePrefab is Entity.Null.");
                continue;
            }

            if (!firePointMap.TryGetValue(part.ValueRO.BaseEntity, out Entity fpEntity))
            {
                Debug.LogWarning("[TurretFireSystem] No FirePoint found for this turret. " +
                                 "Add FirePointAuthoring to the FirePoint child GameObject.");
                continue;
            }

            if (!l2wLookup.TryGetComponent(fpEntity, out var fpL2W))
            {
                Debug.LogWarning("[TurretFireSystem] FirePoint has no LocalToWorld.");
                continue;
            }

            if (!firePointLookup.TryGetComponent(fpEntity, out var firePoint))
            {
                Debug.LogWarning("[TurretFireSystem] FirePoint has no FirePointComponent.");
                continue;
            }

            float3 spawnPos = fpL2W.Position;

            // Rotate the stored local axis into world space using the FirePoint's
            // world rotation. This correctly maps X-axis barrels regardless of
            // how the FirePoint GameObject itself is rotated in the scene.
            float3 fireDir = math.normalize(
                math.rotate(fpL2W.Rotation, firePoint.LocalFireAxis));

            var projectile = ecb.Instantiate(weapon.ValueRO.ProjectilePrefab);
            ecb.SetComponent(projectile, LocalTransform.FromPositionRotation(
                spawnPos,
                quaternion.LookRotationSafe(fireDir, math.up())
            ));
            ecb.SetComponent(projectile, new ProjectileComponent
            {
                Direction         = fireDir,
                Speed             = weapon.ValueRO.ProjectileSpeed,
                Damage            = weapon.ValueRO.Damage,
                MaxRange          = weapon.ValueRO.Range,
                DistanceTraveled = 0f
            });

            weapon.ValueRW.TimeSinceLastShot = 0f;
        }

        firePointMap.Dispose();
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}