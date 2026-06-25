// EnemyMovementSystem.cs
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct MovementSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<MovingComponent>();
        state.RequireForUpdate<TargetComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {   
        new MoveJob().ScheduleParallel();
    }

    [BurstCompile]
    private partial struct MoveJob : IJobEntity
    {
        private void Execute(
            ref PhysicsVelocity velocity,
            ref LocalTransform transform,
            in  TargetComponent target,
            in  MovingComponent moving)
            // in  MeleeDamageComponent melee   // stop distance = attack range
        {
            float3 toTarget = target.TargetPosition - transform.Position;
            toTarget.y = 0f; // ignore vertical distance for movement

            float dist = math.length(toTarget);
            if (dist <= moving.speed)
            {
                velocity.Linear = float3.zero;
                return;
            }

            float3 direction = toTarget / dist;

            velocity.Linear = new float3(
                direction.x * moving.speed,
                0f,
                direction.z * moving.speed);
        }
    }
}
