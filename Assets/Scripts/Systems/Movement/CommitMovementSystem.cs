using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(ApplyTurningSystem))]
public partial struct CommitMovementSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        new CommitMovementJob
        {
            DeltaTime = SystemAPI.Time.DeltaTime
        }.ScheduleParallel();
    }

    [BurstCompile]
    private partial struct CommitMovementJob : IJobEntity
    {
        public float DeltaTime;

        private void Execute(ref LocalTransform transform, in MovementState movementState)
        {
            transform.Position = GroundPlane.Advance(
                transform.Position,
                movementState.Velocity,
                DeltaTime);

            float3 flatVelocity = GroundPlane.Project(movementState.Velocity);
            if (math.lengthsq(flatVelocity) > math.EPSILON)
            {
                transform.Rotation = quaternion.LookRotationSafe(
                    math.normalize(flatVelocity),
                    math.up());
            }
        }
    }
}
