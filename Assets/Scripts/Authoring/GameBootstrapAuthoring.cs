using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public sealed class GameBootstrapAuthoring : MonoBehaviour
{
    public GameObject MovingActorPrefab;

    [Min(0)] public int ActorCount = 6;
    [Min(1)] public int ActorsPerRow = 3;
    [Min(0.1f)] public float Spacing = 2f;
    public Vector3 SpawnOrigin;
    public Vector3 TargetPosition = new Vector3(10f, 0f, 10f);

    private sealed class Baker : Baker<GameBootstrapAuthoring>
    {
        public override void Bake(GameBootstrapAuthoring authoring)
        {
            if (authoring.MovingActorPrefab == null)
            {
                Debug.LogError($"{nameof(GameBootstrapAuthoring)} on '{authoring.name}' needs a moving actor prefab.");
                return;
            }

            Entity entity = GetEntity(TransformUsageFlags.None);
            Entity prefab = GetEntity(authoring.MovingActorPrefab, TransformUsageFlags.Dynamic);

            AddComponent(entity, new GameBootstrapConfig
            {
                MovingActorPrefab = prefab,
                ActorCount = authoring.ActorCount,
                ActorsPerRow = authoring.ActorsPerRow,
                Spacing = authoring.Spacing,
                SpawnOrigin = (float3)authoring.SpawnOrigin,
                TargetPosition = (float3)authoring.TargetPosition
            });
        }
    }
}
