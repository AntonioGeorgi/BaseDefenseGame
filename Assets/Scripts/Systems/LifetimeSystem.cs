// LifetimeSystem.cs
using Unity.Burst;
using Unity.Entities;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct LifetimeSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<LifetimeComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float dt = SystemAPI.Time.DeltaTime;

        foreach (var (lifetime, health) in
            SystemAPI.Query<RefRW<LifetimeComponent>, RefRW<HealthComponent>>()
                     .WithAll<EnemyTag>())
        {
            lifetime.ValueRW.SecondsRemaining -= dt;

            if (lifetime.ValueRO.SecondsRemaining <= 0f)
            {
                // Set health to zero — HealthSystem will then pool this entity
                health.ValueRW.Current = 0f;
            }
        }
    }
}