// EnemyMovementSystem.cs
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
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
            ref PhysicsVelocity     velocity,
            ref LocalTransform      transform,
            in  MoveTargetComponent moveTarget,
            in  MovementSpeedComponent speed,
            in  MeleeDamageComponent melee,   // stop distance = attack range
            in  EnemyTag _)
        {
            float3 toTarget = moveTarget.Value - transform.Position;
            toTarget.y      = 0f;
            float dist      = math.length(toTarget);

            // Stop at melee range — same value that EnemyMeleeDamageSystem uses
            if (dist < melee.MeleeRange)
            {
                velocity.Linear  = float3.zero;
                velocity.Angular = float3.zero;
                return;
            }

            float3 direction = toTarget / dist;
            velocity.Linear  = new float3(
                direction.x * speed.Value,
                0f,
                direction.z * speed.Value);
            velocity.Angular = float3.zero;

            // Floating point epsilon — not a game constant
            if (dist > 0.001f)
                transform.Rotation = quaternion.LookRotationSafe(direction, math.up());
        }
    }
}