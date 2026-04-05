// TurretRotationSystem.cs — full replacement
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(TargetingSystem))]
public partial struct TurretRotationSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<TurretMountTag>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var targetLookup    = SystemAPI.GetComponentLookup<TargetComponent>(isReadOnly: true);
        var weaponLookup    = SystemAPI.GetComponentLookup<TurretWeaponComponent>(isReadOnly: true);
        var baseL2WLookup   = SystemAPI.GetComponentLookup<LocalToWorld>(isReadOnly: true);
        var firePointLookup = SystemAPI.GetComponentLookup<FirePointComponent>(isReadOnly: true);
        var partLookup      = SystemAPI.GetComponentLookup<TurretPartComponent>(isReadOnly: true);

        new MountRotationJob
        {
            DeltaTime       = SystemAPI.Time.DeltaTime,
            TargetLookup    = targetLookup,
            WeaponLookup    = weaponLookup,
            BaseL2WLookup   = baseL2WLookup,
            FirePointLookup = firePointLookup,
            PartLookup      = partLookup
        }.Schedule();
    }

    [BurstCompile]
    private partial struct MountRotationJob : IJobEntity
    {
        public float DeltaTime;

        [ReadOnly] public ComponentLookup<TargetComponent>       TargetLookup;
        [ReadOnly] public ComponentLookup<TurretWeaponComponent> WeaponLookup;
        [ReadOnly] public ComponentLookup<LocalToWorld>          BaseL2WLookup;
        [ReadOnly] public ComponentLookup<FirePointComponent>    FirePointLookup;
        [ReadOnly] public ComponentLookup<TurretPartComponent>   PartLookup;

        private void Execute(
            ref LocalTransform      mountLocal,
            in  TurretPartComponent turretPart,
            in  TurretMountTag _)
        {
            if (!TargetLookup.TryGetComponent(turretPart.BaseEntity, out var target))
                return;
            if (!target.HasTarget) return;
            if (!WeaponLookup.TryGetComponent(turretPart.BaseEntity, out var weapon))
                return;
            if (!BaseL2WLookup.TryGetComponent(turretPart.BaseEntity, out var baseL2W))
                return;

            float3 mountWorldPos = math.transform(baseL2W.Value, mountLocal.Position);
            float3 toTarget      = target.LastKnownPosition - mountWorldPos;
            toTarget.y           = 0f;

            if (math.lengthsq(toTarget) < 0.001f) return;

            float3 toTargetNorm = math.normalize(toTarget);

            // LookRotationSafe points Z toward target.
            // If the barrel is along X we need to compensate by rotating
            // the desired orientation so X points toward target instead.
            quaternion zForwardRot = quaternion.LookRotationSafe(toTargetNorm, math.up());

            // Find the fire axis from the fire point entity
            float3 fireAxis = new float3(0, 0, 1); // default Z
            if (PartLookup.TryGetComponent(turretPart.FirePointEntity, out var fpPart) == false)
            {
                // fallback: check fire point stored on this part
            }
            if (FirePointLookup.TryGetComponent(turretPart.FirePointEntity, out var firePoint))
            {
                fireAxis = firePoint.LocalFireAxis;
            }

            // Compute the rotation that takes Z to fireAxis
            // Then compose: first apply that offset, then face the target
            quaternion axisOffset = QuaternionFromZToAxis(fireAxis);
            quaternion desiredWorldRot = math.mul(zForwardRot, math.inverse(axisOffset));

            // Convert desired world rotation to local space
            quaternion desiredLocalRot = math.mul(
                math.inverse(baseL2W.Rotation),
                desiredWorldRot);

            mountLocal.Rotation = math.slerp(
                mountLocal.Rotation,
                desiredLocalRot,
                math.saturate(math.radians(weapon.MountRotationSpeed) * DeltaTime));
        }

        // Returns the rotation that maps Z forward onto the given axis
        private static quaternion QuaternionFromZToAxis(float3 targetAxis)
        {
            float3 zAxis = new float3(0, 0, 1);
            targetAxis   = math.normalizesafe(targetAxis);

            float dot = math.dot(zAxis, targetAxis);

            // Already aligned — no rotation needed
            if (dot > 0.9999f) return quaternion.identity;

            // Opposite direction — 180 degree rotation around Y
            if (dot < -0.9999f)
                return quaternion.AxisAngle(new float3(0, 1, 0), math.PI);

            float3 cross = math.cross(zAxis, targetAxis);
            return math.normalize(new quaternion(
                cross.x,
                cross.y,
                cross.z,
                1f + dot));
        }
    }
}