// TurretRotationSystem.cs
// Rotates the turret mount (Y-axis only) to face the target found by TargetingSystem.
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(TargetingSystem))] // MUST run after targeting writes TargetComponent
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
        float dt = SystemAPI.Time.DeltaTime;
        const float rotationSpeed = 120f; // degrees per second

        // Mounts don't hold TargetComponent directly — they read it from their BaseEntity
        var targetLookup    = SystemAPI.GetComponentLookup<TargetComponent>(isReadOnly: true);
        var transformLookup = SystemAPI.GetComponentLookup<LocalTransform>(isReadOnly: true);

        new MountRotationJob
        {
            DeltaTime       = dt,
            RotationSpeed   = math.radians(rotationSpeed),
            TargetLookup    = targetLookup,
            TransformLookup = transformLookup
        }.ScheduleParallel();
    }

    [BurstCompile]
    private partial struct MountRotationJob : IJobEntity
    {
        public float DeltaTime;
        public float RotationSpeed;

        [ReadOnly] public ComponentLookup<TargetComponent>   TargetLookup;
        [ReadOnly] public ComponentLookup<LocalTransform>    TransformLookup;

        private void Execute(
            ref LocalTransform mountTransform,
            in  TurretPartComponent turretPart,
            in  TurretMountTag _)
        {
            // Look up the target from the base entity
            if (!TargetLookup.TryGetComponent(turretPart.BaseEntity, out var target)) return;
            if (!target.HasTarget) return;

            // Get mount world position to compute look direction
            float3 mountPos  = mountTransform.Position;
            float3 toTarget  = target.LastKnownPosition - mountPos;
            toTarget.y       = 0f; // Y-axis rotation only — no barrel tilt here

            if (math.lengthsq(toTarget) < 0.001f) return;

            quaternion desiredRot = quaternion.LookRotationSafe(math.normalize(toTarget), math.up());

            // Slerp for smooth rotation
            mountTransform.Rotation = math.slerp(
                mountTransform.Rotation,
                desiredRot,
                math.saturate(RotationSpeed * DeltaTime)
            );
        }
    }
}