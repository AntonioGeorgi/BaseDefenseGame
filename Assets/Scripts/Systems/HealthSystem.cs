// HealthSystem.cs
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(EnemyMeleeDamageSystem))]
public partial struct HealthSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<HealthComponent>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        bool hasPool = SystemAPI.HasSingleton<EnemyPoolComponent>();
        EnemyPoolComponent pool = default;
        if (hasPool) pool = SystemAPI.GetSingleton<EnemyPoolComponent>();

        // ── Pool dead enemies ─────────────────────────────────────────────
        foreach (var (health, lifetime, entity) in
            SystemAPI.Query<
                RefRO<HealthComponent>,
                RefRO<LifetimeComponent>>()
                     .WithAll<EnemyTag>()
                     .WithNone<PooledTag>()
                     .WithEntityAccess())
        {
            if (!health.ValueRO.IsDead) continue;

            // Reset stats so the entity is ready when dequeued.
            // Position is NOT touched here — SpawnerSystem sets it
            // just before re-enabling, so the entity never appears
            // at a stale position even for one frame.
            ecb.SetComponent(entity, new HealthComponent
            {
                Current = health.ValueRO.Max,
                Max     = health.ValueRO.Max
            });
            ecb.SetComponent(entity, new LifetimeComponent
            {
                SecondsRemaining = lifetime.ValueRO.MaxLifetime,
                MaxLifetime      = lifetime.ValueRO.MaxLifetime
            });
            ecb.SetComponent(entity, new MoveTargetComponent());

            // Mark as pooled and disable — that's it, no position change
            ecb.AddComponent<PooledTag>(entity);
            ecb.AddComponent<Disabled>(entity);

            if (hasPool)
                pool.Available.Enqueue(entity);

            Debug.Log($"[Pool] Enemy pooled. Pool size now: " +
                      $"{(hasPool ? pool.Available.Count : 0)}");
        }

        // ── Destroy dead turrets ──────────────────────────────────────────
        foreach (var (health, entity) in
            SystemAPI.Query<RefRO<HealthComponent>>()
                     .WithAll<TurretBaseTag>()
                     .WithEntityAccess())
        {
            if (health.ValueRO.IsDead)
                ecb.DestroyEntity(entity);
        }

        // ── Handle command building death ─────────────────────────────────
        foreach (var (health, entity) in
            SystemAPI.Query<RefRO<HealthComponent>>()
                     .WithAll<CommandBuildingTag>()
                     .WithEntityAccess())
        {
            if (health.ValueRO.IsDead)
            {
                Debug.Log("GAME OVER — Command Building destroyed!");
                ecb.DestroyEntity(entity);
            }
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();

        if (hasPool) SystemAPI.SetSingleton(pool);
    }
}