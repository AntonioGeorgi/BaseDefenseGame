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
        new CalculateDesiredVelocityJob
        {
            DeltaTime = SystemAPI.Time.DeltaTime
        }.ScheduleParallel();
    }

    [BurstCompile]
    private partial struct CalculateDesiredVelocityJob : IJobEntity
    {
        public float DeltaTime;

        private void Execute(
            ref MovementState movementState,
            in LocalTransform transform,
            in MoveTarget target,
            in MovementStats stats)
        {
            movementState.DesiredVelocity = MovementMath.CalculateDesiredVelocity(
                transform.Position,
                target.TargetPosition,
                stats.MaxSpeed,
                stats.StopDistance,
                DeltaTime);
        }
    }
}
