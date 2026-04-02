// EnemyMovementSystem.cs
// Moves all enemies toward their MoveTargetComponent. Burst-compiled, runs as a parallel job.
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;       // <── needed for PhysicsVelocity
using Unity.Transforms;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct EnemyMovementSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EnemyTag>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        new EnemyMoveJob
        {
            DeltaTime = SystemAPI.Time.DeltaTime
        }.ScheduleParallel();
    }

    [BurstCompile]
    private partial struct EnemyMoveJob : IJobEntity
    {
        public float DeltaTime;

        private void Execute(
            ref PhysicsVelocity  velocity,    // ← SET this, don't set transform directly
            ref LocalTransform   transform,
            in  MoveTargetComponent moveTarget,
            in  MovementSpeedComponent speed,
            in  EnemyTag _)
        {
            float3 toTarget  = moveTarget.Value - transform.Position;
            toTarget.y       = 0f;            // stay on the ground plane
            float  dist      = math.length(toTarget);

            if (dist < 1.5f)
            {
                // Close enough — stop moving, start attacking
                velocity.Linear  = float3.zero;
                velocity.Angular = float3.zero;
                return;
            }

            float3 direction = toTarget / dist; // normalize

            // Set velocity — physics engine moves the entity AND resolves
            // collisions with other enemies so they spread out
            velocity.Linear = new float3(
                direction.x * speed.Value,
                0f,                           // Y locked by Rigidbody constraints
                direction.z * speed.Value
            );

            // Kill any spin the physics engine might add
            velocity.Angular = float3.zero;

            // Face direction of travel
            transform.Rotation = quaternion.LookRotationSafe(direction, math.up());
        }
    }
}