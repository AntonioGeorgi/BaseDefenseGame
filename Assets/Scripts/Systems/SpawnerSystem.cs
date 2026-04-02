// SpawnerSystem.cs
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct SpawnerSystem : ISystem
{
    private float3 _commandBuildingPos;
    private bool   _commandBuildingFound;
    private uint   _randomSeed;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SpawnerComponent>();
        _randomSeed = 1;
    }

    public void OnUpdate(ref SystemState state)
    {
        float dt = SystemAPI.Time.DeltaTime;

        if (!_commandBuildingFound)
        {
            foreach (var (_, transform) in
                SystemAPI.Query<RefRO<CommandBuildingTag>, RefRO<LocalTransform>>())
            {
                _commandBuildingPos   = transform.ValueRO.Position;
                _commandBuildingFound = true;
                break;
            }
            if (!_commandBuildingFound) return;
        }

        bool hasPool = SystemAPI.HasSingleton<EnemyPoolComponent>();
        EnemyPoolComponent pool = default;
        if (hasPool) pool = SystemAPI.GetSingleton<EnemyPoolComponent>();

        var ecb = new EntityCommandBuffer(Allocator.Temp);
        var rng = new Unity.Mathematics.Random(_randomSeed++);
        if (_randomSeed == 0) _randomSeed = 1;

        foreach (var (spawner, spawnerTransform) in
            SystemAPI.Query<RefRW<SpawnerComponent>, RefRO<LocalTransform>>())
        {
            spawner.ValueRW.SpawnTimer -= dt;
            if (spawner.ValueRO.SpawnTimer > 0f) continue;

            spawner.ValueRW.SpawnTimer = 1f / math.max(spawner.ValueRO.SpawnRate, 0.01f);

            for (int i = 0; i < spawner.ValueRO.SpawnBatchSize; i++)
            {
                float3 spawnPos = spawnerTransform.ValueRO.Position + new float3(
                    rng.NextFloat(-3f, 3f), 0f, rng.NextFloat(-3f, 3f));

                Entity enemy;
                bool   fromPool = false;

                if (hasPool && pool.Available.Count > 0 &&
                    pool.Available.TryDequeue(out enemy))
                {
                    fromPool = true;

                    // ── ORDER MATTERS: position and stats BEFORE enable ───
                    // The entity is still Disabled (invisible) while these
                    // writes happen. RemoveComponent<Disabled> is last,
                    // so the entity pops into existence already correct.
                    ecb.SetComponent(enemy, LocalTransform.FromPosition(spawnPos));
                    ecb.SetComponent(enemy, new MoveTargetComponent
                    {
                        Value = _commandBuildingPos
                    });
                    ecb.SetComponent(enemy, new HealthComponent
                    {
                        Current = spawner.ValueRO.EnemyMaxHealth,
                        Max     = spawner.ValueRO.EnemyMaxHealth
                    });
                    ecb.SetComponent(enemy, new LifetimeComponent
                    {
                        SecondsRemaining = spawner.ValueRO.EnemyLifetime,
                        MaxLifetime      = spawner.ValueRO.EnemyLifetime
                    });

                    // Enable last — entity is now visible at the correct spot
                    ecb.RemoveComponent<PooledTag>(enemy);
                    ecb.RemoveComponent<Disabled>(enemy);

                    Debug.Log($"[Pool] Reused pooled enemy. " +
                              $"Remaining in pool: {pool.Available.Count}");
                }
                else
                {
                    // Pool empty — create a fresh entity from the prefab
                    enemy = ecb.Instantiate(spawner.ValueRO.EnemyPrefab);
                    ecb.SetComponent(enemy, LocalTransform.FromPosition(spawnPos));
                    ecb.SetComponent(enemy, new MoveTargetComponent
                    {
                        Value = _commandBuildingPos
                    });
                    Debug.Log("[Pool] Pool empty — created new enemy entity.");
                }
            }
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();

        if (hasPool) SystemAPI.SetSingleton(pool);
    }
}