// CommandBuildingAuthoring.cs
using Unity.Entities;
using UnityEngine;

public class CommandBuildingAuthoring : MonoBehaviour
{
    public float MaxHealth = 1000f;

    class Baker : Baker<CommandBuildingAuthoring>
    {
        public override void Bake(CommandBuildingAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new CommandBuildingTag());
            AddComponent(entity, new HealthComponent
            {
                Current = authoring.MaxHealth,
                Max = authoring.MaxHealth
            });
        }
    }
}