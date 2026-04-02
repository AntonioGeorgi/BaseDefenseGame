// TargetingSystem.cs
// The most performance-critical system. Finds the closest enemy within range for each turret.
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct TargetingSystem : ISystem
{
    // Persistent enemy position cache — rebuilt each frame
    private NativeList<EnemyData> _enemyCache;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _enemyCache = new NativeList<EnemyData>(1024, Allocator.Persistent);
        state.RequireForUpdate<TurretBaseTag>();
    }

    public void OnDestroy(ref SystemState state)
    {
        if (_enemyCache.IsCreated) _enemyCache.Dispose();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // ── Step 1: Rebuild enemy position cache ──
        _enemyCache.Clear();

        foreach (var (transform, entity) in
            SystemAPI.Query<RefRO<LocalTransform>>()
                     .WithAll<EnemyTag>()
                     .WithEntityAccess())
        {
            _enemyCache.Add(new EnemyData
            {
                Entity   = entity,
                Position = transform.ValueRO.Position
            });
        }

        // ── Step 2: For each turret base, find closest enemy in range ──
        // Note: we run this single-threaded because writing TargetComponent
        // needs main-thread safety. Use a parallel job + ECB for 10k+ turrets.

        // Scaling note: For 10,000+ enemies, replace the inner loop with a NativeParallelHashMap spatial grid 
        // or Unity Physics OverlapSphereCommand.
        // The interface (TargetComponent) stays identical — swap the internal algorithm, nothing else changes.
        foreach (var (target, weapon, transform) in
            SystemAPI.Query<
                RefRW<TargetComponent>,
                RefRO<TurretWeaponComponent>,
                RefRO<LocalTransform>>()
            .WithAll<TurretBaseTag>())
        {
            float  rangeSquared  = weapon.ValueRO.Range * weapon.ValueRO.Range;
            float  closestDistSq = float.MaxValue;
            Entity closestEnemy  = Entity.Null;
            float3 closestPos    = float3.zero;

            float3 turretPos = transform.ValueRO.Position;

            for (int i = 0; i < _enemyCache.Length; i++)
            {
                float distSq = math.distancesq(turretPos, _enemyCache[i].Position);
                if (distSq < rangeSquared && distSq < closestDistSq)
                {
                    closestDistSq = distSq;
                    closestEnemy  = _enemyCache[i].Entity;
                    closestPos    = _enemyCache[i].Position;
                }
            }

            target.ValueRW.Value             = closestEnemy;
            target.ValueRW.LastKnownPosition = closestPos;
        }
    }

    // Small cache struct for the enemy snapshot
    private struct EnemyData
    {
        public Entity Entity;
        public float3 Position;
    }
}