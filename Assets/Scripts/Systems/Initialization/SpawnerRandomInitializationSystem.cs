using Unity.Collections;
using Unity.Entities;

/// <summary>
/// Gives each newly created spawner its own deterministic random stream.
/// Assignment is deliberately single-threaded because stream allocation order
/// is part of deterministic world state.
/// </summary>
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateBefore(typeof(SpawnerSystem))]
public partial struct SpawnerRandomInitializationSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<WorldRandomState>();
    }

    public void OnUpdate(ref SystemState state)
    {
        RefRW<WorldRandomState> worldRandom = SystemAPI.GetSingletonRW<WorldRandomState>();
        EntityCommandBuffer commandBuffer = new EntityCommandBuffer(Allocator.Temp);

        foreach ((RefRW<SpawnerState> spawnerState, Entity entity)
                 in SystemAPI.Query<RefRW<SpawnerState>>()
                     .WithAll<SpawnerNeedsRandomSeed>()
                     .WithEntityAccess())
        {
            uint derivedSeed = worldRandom.ValueRW.Random.NextUInt(1u, uint.MaxValue);
            spawnerState.ValueRW.Random = new Unity.Mathematics.Random(derivedSeed);
            commandBuffer.RemoveComponent<SpawnerNeedsRandomSeed>(entity);
        }

        commandBuffer.Playback(state.EntityManager);
        commandBuffer.Dispose();
    }
}
