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
        var targetLookup = SystemAPI.GetComponentLookup<TargetComponent>(isReadOnly: true);
        var weaponLookup = SystemAPI.GetComponentLookup<TurretWeaponComponent>(isReadOnly: true);

        new MountRotationJob
        {
            DeltaTime    = SystemAPI.Time.DeltaTime,
            TargetLookup = targetLookup,
            WeaponLookup = weaponLookup
        }.ScheduleParallel();
    }

    [BurstCompile]
    private partial struct MountRotationJob : IJobEntity
    {
        public float DeltaTime;

        [ReadOnly] public ComponentLookup<TargetComponent>      TargetLookup;
        [ReadOnly] public ComponentLookup<TurretWeaponComponent> WeaponLookup;

        private void Execute(
            ref LocalTransform      mountTransform,
            in  TurretPartComponent turretPart,
            in  TurretMountTag _)
        {
            if (!TargetLookup.TryGetComponent(turretPart.BaseEntity, out var target)) return;
            if (!target.HasTarget) return;

            if (!WeaponLookup.TryGetComponent(turretPart.BaseEntity, out var weapon)) return;

            float3 toTarget = target.LastKnownPosition - mountTransform.Position;
            toTarget.y      = 0f;

            // Floating point epsilon guard — not a game constant
            if (math.lengthsq(toTarget) < 0.001f) return;

            quaternion desiredRot = quaternion.LookRotationSafe(
                math.normalize(toTarget), math.up());

            // MountRotationSpeed comes from the weapon component, set in TurretAuthoring
            mountTransform.Rotation = math.slerp(
                mountTransform.Rotation,
                desiredRot,
                math.saturate(math.radians(weapon.MountRotationSpeed) * DeltaTime));
        }
    }
}