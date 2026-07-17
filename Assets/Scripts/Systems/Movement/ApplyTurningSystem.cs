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
            float desiredSpeed = math.length(movementState.DesiredVelocity);
            if (desiredSpeed <= math.EPSILON)
            {
                movementState.Velocity = float3.zero;
                return;
            }

            float3 desiredDirection = movementState.DesiredVelocity / desiredSpeed;
            float3 currentDirection = math.normalizesafe(
                GroundPlane.Project(movementState.Velocity),
                desiredDirection);

            float dot = math.clamp(math.dot(currentDirection, desiredDirection), -1f, 1f);
            float signedAngle = math.atan2(
                currentDirection.x * desiredDirection.z - currentDirection.z * desiredDirection.x,
                dot);
            float maxTurn = math.radians(math.max(0f, stats.TurnSpeed)) * DeltaTime;
            float appliedAngle = math.clamp(signedAngle, -maxTurn, maxTurn);
            float sin = math.sin(appliedAngle);
            float cos = math.cos(appliedAngle);

            float3 turnedDirection = new float3(
                currentDirection.x * cos - currentDirection.z * sin,
                0f,
                currentDirection.x * sin + currentDirection.z * cos);

            movementState.Velocity = turnedDirection * desiredSpeed;
        }
    }
}
