// ProjectileMovementSystem.cs
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(TurretFireSystem))]
public partial struct ProjectileMovementSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ProjectileTag>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float dt = SystemAPI.Time.DeltaTime;

        // Snapshot enemy positions for hit detection
        var enemySnapshots = new NativeList<EnemySnapshot>(Allocator.Temp);

        foreach (var (transform, health, entity) in
            SystemAPI.Query<
                RefRO<LocalTransform>,
                RefRO<HealthComponent>>()
                     .WithAll<EnemyTag>()
                     .WithEntityAccess())
        {
            if (health.ValueRO.IsDead) continue;
            enemySnapshots.Add(new EnemySnapshot
            {
                Entity   = entity,
                Position = transform.ValueRO.Position
            });
        }

        var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (projectile, transform, entity) in
            SystemAPI.Query<
                RefRW<ProjectileComponent>,
                RefRW<LocalTransform>>()
                     .WithAll<ProjectileTag>()
                     .WithEntityAccess())
        {
            float   stepDist = projectile.ValueRO.Speed * dt;
            float3  newPos   = transform.ValueRO.Position +
                               projectile.ValueRO.Direction * stepDist;

            projectile.ValueRW.DistanceTraveled += stepDist;
            transform.ValueRW.Position            = newPos;

            // Despawn if max range exceeded
            if (projectile.ValueRO.DistanceTraveled >= projectile.ValueRO.MaxRange)
            {
                ecb.DestroyEntity(entity);
                continue;
            }

            // Hit detection — check against all live enemies
            // Hit radius of 0.6 matches the enemy capsule collider roughly
            // If you want this data-driven, add HitRadius to ProjectileComponent
            bool hit = false;

            for (int i = 0; i < enemySnapshots.Length; i++)
            {
                if (math.distance(newPos, enemySnapshots[i].Position) > projectile.ValueRO.HitRadius)
                    continue;

                // Apply damage
                var health = SystemAPI.GetComponentRW<HealthComponent>(
                    enemySnapshots[i].Entity);
                health.ValueRW.Current = math.max(
                    0f,
                    health.ValueRO.Current - projectile.ValueRO.Damage);

                ecb.DestroyEntity(entity);
                hit = true;
                break;
            }

            if (hit) continue;
        }

        enemySnapshots.Dispose();
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }

    private struct EnemySnapshot
    {
        public Entity Entity;
        public float3 Position;
    }
}