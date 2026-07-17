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
            float3 flatVelocity = GroundPlane.Project(movementState.Velocity);
            float3 nextPosition = transform.Position + flatVelocity * DeltaTime;

            transform.Position = GroundPlane.PreserveHeight(transform.Position, nextPosition);

            if (math.lengthsq(flatVelocity) > math.EPSILON)
            {
                transform.Rotation = quaternion.LookRotationSafe(
                    math.normalize(flatVelocity),
                    math.up());
            }
        }
    }
}
