using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class SpawnerAuthoring : MonoBehaviour
{
    [Tooltip("The entity prefab created by this spawner. Avoid assigning the spawner prefab itself unless exponential spawning is intentional.")]
    public GameObject Prefab;

    [Header("Timing")]
    [Min(0.01f)] public float Interval = 2f;
    [Min(0f)] public float InitialDelay;
    public bool SpawnImmediately = true;
    public bool StartEnabled = true;

    [Header("Batch")]
    [Min(1)] public int BatchSize = 1;
    [Min(0f)] public float SpawnRadius = 0.5f;

    private sealed class Baker : Baker<SpawnerAuthoring>
    {
        public override void Bake(SpawnerAuthoring authoring)
        {
            if (authoring.Prefab == null)
            {
                Debug.LogError($"{nameof(SpawnerAuthoring)} on '{authoring.name}' needs a prefab.");
                return;
            }

            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            Entity prefab = GetEntity(authoring.Prefab, TransformUsageFlags.Dynamic);

            AddComponent(entity, new SpawnerConfig
            {
                Prefab = prefab,
                Interval = math.max(0.01f, authoring.Interval),
                BatchSize = math.max(1, authoring.BatchSize),
                SpawnRadius = math.max(0f, authoring.SpawnRadius)
            });

            AddComponent(entity, new SpawnerState
            {
                TimeUntilNextSpawn = authoring.SpawnImmediately
                    ? 0f
                    : math.max(0f, authoring.InitialDelay),
                // Assigned from WorldRandomState when this entity enters the world.
                Random = default
            });
            AddComponent<SpawnerEnabled>(entity);
            SetComponentEnabled<SpawnerEnabled>(entity, authoring.StartEnabled);
            AddComponent<SpawnerNeedsRandomSeed>(entity);
        }
    }
}
