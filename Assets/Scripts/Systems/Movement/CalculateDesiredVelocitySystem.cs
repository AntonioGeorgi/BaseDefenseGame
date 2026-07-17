using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateBefore(typeof(ApplyTurningSystem))]
public partial struct CalculateDesiredVelocitySystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        new CalculateDesiredVelocityJob().ScheduleParallel();
    }

    [BurstCompile]
    private partial struct CalculateDesiredVelocityJob : IJobEntity
    {
        private void Execute(
            ref MovementState movementState,
            in LocalTransform transform,
            in MoveTarget target,
            in MovementStats stats)
        {
            float stopDistance = math.max(0f, stats.StopDistance);

            if (GroundPlane.DistanceSq(transform.Position, target.TargetPosition)
                <= stopDistance * stopDistance)
            {
                movementState.DesiredVelocity = float3.zero;
                return;
            }

            movementState.DesiredVelocity =
                GroundPlane.Direction(transform.Position, target.TargetPosition)
                * math.max(0f, stats.MaxSpeed);
        }
    }
}
