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

        // ── Read enemy stats directly off the prefab entity ───────────────
        // Prefab entities have all their baked components and are accessible
        // via ComponentLookup even though they don't appear in normal queries
        var healthLookup   = SystemAPI.GetComponentLookup<HealthComponent>(isReadOnly: true);
        var lifetimeLookup = SystemAPI.GetComponentLookup<LifetimeComponent>(isReadOnly: true);

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

            // Guard against zero/negative spawn rate — division safety only
            spawner.ValueRW.SpawnTimer = 1f / math.max(spawner.ValueRO.SpawnRate, 0.01f);

            // Read the authoritative stats from the prefab's baked components.
            // This is the single source of truth — no duplication anywhere.
            if (!healthLookup.TryGetComponent(spawner.ValueRO.EnemyPrefab, out var prefabHealth))
            {
                Debug.LogError("[SpawnerSystem] EnemyPrefab has no HealthComponent baked. " +
                               "Check EnemyAuthoring has a Data asset assigned.");
                continue;
            }

            if (!lifetimeLookup.TryGetComponent(spawner.ValueRO.EnemyPrefab, out var prefabLifetime))
            {
                Debug.LogError("[SpawnerSystem] EnemyPrefab has no LifetimeComponent baked. " +
                               "Check EnemyAuthoring has a Data asset assigned.");
                continue;
            }

            for (int i = 0; i < spawner.ValueRO.SpawnBatchSize; i++)
            {
                float3 spawnPos = spawnerTransform.ValueRO.Position + new float3(
                    rng.NextFloat(-3f, 3f), 0f, rng.NextFloat(-3f, 3f));

                Entity enemy;

                if (hasPool && pool.Available.Count > 0 &&
                    pool.Available.TryDequeue(out enemy))
                {
                    // Spawner only decides WHERE the enemy appears
                    ecb.SetComponent(enemy, LocalTransform.FromPosition(spawnPos));

                    // Signal that this entity needs its state reset
                    ecb.AddComponent<PendingInitTag>(enemy);

                    // Enable — EnemyInitSystem runs this frame and resets everything
                    ecb.RemoveComponent<PooledTag>(enemy);
                    ecb.RemoveComponent<Disabled>(enemy);
                }
                else
                {
                    // Fresh entity — baked values are already correct, just set position
                    enemy = ecb.Instantiate(spawner.ValueRO.EnemyPrefab);
                    ecb.SetComponent(enemy, LocalTransform.FromPosition(spawnPos));
                    ecb.SetComponent(enemy, new MoveTargetComponent
                    {
                        Value = _commandBuildingPos
                    });
                }
            }
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();

        if (hasPool) SystemAPI.SetSingleton(pool);
    }
}