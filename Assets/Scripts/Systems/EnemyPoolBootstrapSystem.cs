// EnemyPoolBootstrapSystem.cs
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

[UpdateInGroup(typeof(InitializationSystemGroup))] // runs before SimulationSystemGroup
public partial struct EnemyPoolBootstrapSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        const int POOL_SIZE = 2000;

        // Create the singleton pool entity
        var poolEntity = state.EntityManager.CreateEntity();
        var pool = new EnemyPoolComponent
        {
            Available = new NativeQueue<Entity>(Allocator.Persistent)
        };

        // Pre-warm: instantiate POOL_SIZE copies of the prefab, immediately disable them
        // You need the prefab entity — get it from a config singleton (see SpawnerComponent)
        // For now we populate the pool lazily in HealthSystem when enemies die

        state.EntityManager.AddComponentData(poolEntity, pool);

        // This system only needs to run once
        state.Enabled = false;
    }

    public void OnDestroy(ref SystemState state)
    {
        // ALWAYS dispose Persistent NativeContainers or you leak memory
        if (SystemAPI.HasSingleton<EnemyPoolComponent>())
        {
            var pool = SystemAPI.GetSingleton<EnemyPoolComponent>();
            if (pool.Available.IsCreated) pool.Available.Dispose();
        }
    }
}