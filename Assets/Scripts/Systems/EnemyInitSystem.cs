// EnemyInitSystem.cs
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;


/// <summary>
/// Processes all enemies that have just been re-enabled from the pool.
/// This is the DOTS equivalent of OnEnabled — runs once per reactivation.
/// Separates "when to spawn" (SpawnerSystem) from "how to initialise" (here).
/// </summary>
[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(SpawnerSystem))]
public partial struct EnemyInitSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PendingInitTag>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // Cache command building position for move target
        float3 commandPos = float3.zero;
        bool   found      = false;

        foreach (var (_, transform) in
            SystemAPI.Query<RefRO<CommandBuildingTag>, RefRO<LocalTransform>>())
        {
            commandPos = transform.ValueRO.Position;
            found      = true;
            break;
        }

        if (!found) return;

        var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (health, lifetime, _, entity) in
            SystemAPI.Query<
                RefRW<HealthComponent>,
                RefRW<LifetimeComponent>,
                RefRO<PendingInitTag>>()
                     .WithEntityAccess())
        {
            // Reset health to full
            health.ValueRW.Current = health.ValueRO.Max;

            // Reset lifetime using the stored max — no hardcoded values
            lifetime.ValueRW.SecondsRemaining = lifetime.ValueRO.MaxLifetime;

            // Point toward the command building
            ecb.SetComponent(entity, new MoveTargetComponent
            {
                Value = commandPos
            });

            // Consume the tag — this system won't touch this entity again
            // until it's pooled and re-enabled a second time
            ecb.RemoveComponent<PendingInitTag>(entity);
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}