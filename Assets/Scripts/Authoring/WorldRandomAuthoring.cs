using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class WorldRandomAuthoring : MonoBehaviour
{
    [Tooltip("The same seed reproduces the same sequence of derived gameplay seeds.")]
    [Min(1)] public uint WorldSeed = 1;

    private sealed class Baker : Baker<WorldRandomAuthoring>
    {
        public override void Bake(WorldRandomAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new WorldRandomState
            {
                Random = new Unity.Mathematics.Random(math.max(1u, authoring.WorldSeed))
            });
        }
    }
}
