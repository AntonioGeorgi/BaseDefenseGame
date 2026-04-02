// EnemyMeleeDamageSystem.cs — full rewrite, no hidden constants
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(EnemyMovementSystem))]
public partial struct EnemyMeleeDamageSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EnemyTag>();
    }

    public void OnUpdate(ref SystemState state)
    {
        float dt = SystemAPI.Time.DeltaTime;

        // ── Snapshot damageable targets ───────────────────────────────────
        var damageAccumulator = new NativeHashMap<Entity, float>(32, Allocator.Temp);
        var targetSnapshots   = new NativeList<TargetSnapshot>(Allocator.Temp);

        foreach (var (transform, entity) in
            SystemAPI.Query<RefRO<LocalTransform>>()
                     .WithAny<TurretBaseTag, CommandBuildingTag>()
                     .WithEntityAccess())
        {
            damageAccumulator.TryAdd(entity, 0f);
            targetSnapshots.Add(new TargetSnapshot
            {
                Entity   = entity,
                Position = transform.ValueRO.Position
            });
        }

        if (targetSnapshots.Length == 0)
        {
            targetSnapshots.Dispose();
            damageAccumulator.Dispose();
            return;
        }

        // ── Each enemy reads its OWN range and damage from its component ──
        foreach (var (transform, meleeDamage, _) in
            SystemAPI.Query<
                RefRO<LocalTransform>,
                RefRO<MeleeDamageComponent>,
                RefRO<EnemyTag>>())
        {
            for (int i = 0; i < targetSnapshots.Length; i++)
            {
                float dist = math.distance(
                    transform.ValueRO.Position,
                    targetSnapshots[i].Position);

                // MeleeRange comes from the enemy's own component
                if (dist <= meleeDamage.ValueRO.MeleeRange)
                {
                    damageAccumulator[targetSnapshots[i].Entity] +=
                        meleeDamage.ValueRO.DamagePerSecond * dt;
                }
            }
        }

        // ── Apply accumulated damage ──────────────────────────────────────
        foreach (var kvp in damageAccumulator)
        {
            if (kvp.Value <= 0f) continue;

            var health = SystemAPI.GetComponentRW<HealthComponent>(kvp.Key);
            health.ValueRW.Current =
                math.max(0f, health.ValueRO.Current - kvp.Value);
        }

        targetSnapshots.Dispose();
        damageAccumulator.Dispose();
    }

    private struct TargetSnapshot
    {
        public Entity Entity;
        public float3 Position;
    }
}