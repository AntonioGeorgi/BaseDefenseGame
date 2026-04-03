// EnemyAuthoring.cs
using Unity.Entities;
using UnityEngine;

public class EnemyAuthoring : MonoBehaviour
{
    [Tooltip("Drag a EnemyDataSO asset here")]
    public EnemyDataSO Data;

    class Baker : Baker<EnemyAuthoring>
    {
        public override void Bake(EnemyAuthoring authoring)
        {
            if (authoring.Data == null)
            {
                Debug.LogError($"EnemyAuthoring on '{authoring.name}' has no Data assigned!");
                return;
            }

            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new EnemyTag());
            AddComponent(entity, new HealthComponent
            {
                Current = authoring.Data.MaxHealth,
                Max     = authoring.Data.MaxHealth
            });
            AddComponent(entity, new MovementSpeedComponent
            {
                Value = authoring.Data.MoveSpeed
            });
            AddComponent(entity, new MeleeDamageComponent
            {
                DamagePerSecond = authoring.Data.DamagePerSecond,
                MeleeRange      = authoring.Data.MeleeRange
            });
            AddComponent(entity, new MoveTargetComponent());
            AddComponent(entity, new LifetimeComponent
            {
                SecondsRemaining = authoring.Data.Lifetime,
                MaxLifetime      = authoring.Data.Lifetime
            });
        }
    }
}