// SpawnerAuthoring.cs
using Unity.Entities;
using UnityEngine;

public class SpawnerAuthoring : MonoBehaviour
{
    [Tooltip("Must have EnemyAuthoring with a Data asset assigned")]
    public GameObject EnemyPrefab;

    [Header("Spawn Timing")]
    public float SpawnRate  = 2f;
    public int   BatchSize  = 3;

    class Baker : Baker<SpawnerAuthoring>
    {
        public override void Bake(SpawnerAuthoring authoring)
        {
            if (authoring.EnemyPrefab == null)
            {
                Debug.LogError($"SpawnerAuthoring on '{authoring.name}' has no EnemyPrefab assigned!");
                return;
            }

            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new SpawnerComponent
            {
                EnemyPrefab    = GetEntity(authoring.EnemyPrefab, TransformUsageFlags.Dynamic),
                SpawnRate      = authoring.SpawnRate,
                SpawnTimer     = 0f,
                SpawnBatchSize = authoring.BatchSize
            });
        }
    }
}