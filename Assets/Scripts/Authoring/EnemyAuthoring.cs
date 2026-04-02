// EnemyAuthoring.cs
using Unity.Entities;
using UnityEngine;

public class EnemyAuthoring : MonoBehaviour
{
    [Header("Health")]
    public float MaxHealth = 50f;

    [Header("Movement")]
    public float MoveSpeed = 3.5f;

    [Header("Melee Attack")]
    [Tooltip("Damage dealt per second to buildings and turrets")]
    public float DamagePerSecond = 10f;
    [Tooltip("How close the enemy must be to start dealing damage")]
    public float MeleeRange = 2.5f;

    [Header("Lifetime")]
    [Tooltip("Seconds before enemy is auto-returned to pool (test feature)")]
    public float Lifetime = 8f;

    class Baker : Baker<EnemyAuthoring>
    {
        public override void Bake(EnemyAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new EnemyTag());
            AddComponent(entity, new HealthComponent
            {
                Current = authoring.MaxHealth,
                Max     = authoring.MaxHealth
            });
            AddComponent(entity, new MovementSpeedComponent
            {
                Value = authoring.MoveSpeed
            });
            AddComponent(entity, new MoveTargetComponent());
            AddComponent(entity, new MeleeDamageComponent
            {
                DamagePerSecond = authoring.DamagePerSecond,
                MeleeRange      = authoring.MeleeRange
            });
            AddComponent(entity, new LifetimeComponent
            {
                SecondsRemaining = authoring.Lifetime,
                MaxLifetime      = authoring.Lifetime
            });
        }
    }
}