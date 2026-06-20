// EnemyAuthoring.cs
using Unity.Entities;
using UnityEngine;

public class MovingEntityAuthoring : MonoBehaviour
{
    [Tooltip("Drag a MovingEntityDataSO asset here")]
    public MovingEntityDataSO Data;

    class Baker : Baker<MovingEntityAuthoring>
    {
        public override void Bake(MovingEntityAuthoring authoring)
        {
            if (authoring.Data == null)
            {
                Debug.LogError($"MovingEntityAuthoring on '{authoring.name}' has no Data assigned!");
                return;
            }

            DependsOn(authoring.Data);

            var entity = GetEntity(TransformUsageFlags.Dynamic);
            
            AddComponent(entity, authoring.Data.CreateFactionComponent());
            AddComponent(entity, authoring.Data.CreateHealthComponent());
            AddComponent(entity, authoring.Data.CreateMovingComponent());
            AddComponent(entity, authoring.Data.CreateTargetComponent());

            AddComponent(entity, new LifetimeComponent
            {
                SecondsRemaining = authoring.Data.Lifetime,
                MaxLifetime      = authoring.Data.Lifetime
            });
        }
    }
}
