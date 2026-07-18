using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(CalculateDesiredVelocitySystem))]
[UpdateBefore(typeof(CommitMovementSystem))]
public partial struct ApplyTurningSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        new ApplyTurningJob
        {
            DeltaTime = SystemAPI.Time.DeltaTime
        }.ScheduleParallel();
    }

    [BurstCompile]
    private partial struct ApplyTurningJob : IJobEntity
    {
        public float DeltaTime;

        private void Execute(ref MovementState movementState, in MovementStats stats)
        {
            movementState.Velocity = MovementMath.ApplyTurning(
                movementState.Velocity,
                movementState.DesiredVelocity,
                stats.TurnSpeed,
                DeltaTime);
        }
    }
}
