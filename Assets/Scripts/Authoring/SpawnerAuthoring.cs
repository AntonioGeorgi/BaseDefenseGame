// SpawnerAuthoring.cs
using Unity.Entities;
using UnityEngine;

public class SpawnerAuthoring : MonoBehaviour
{
    public GameObject EnemyPrefab;

    [Header("Spawn Timing")]
    public float SpawnRate  = 2f;
    public int   BatchSize  = 3;

    [Header("Enemy Stats — must match EnemyAuthoring on the prefab")]
    [Tooltip("Used to reset health when reusing a pooled enemy")]
    public float EnemyMaxHealth = 50f;
    [Tooltip("Used to reset lifetime when reusing a pooled enemy")]
    public float EnemyLifetime  = 8f;

    class Baker : Baker<SpawnerAuthoring>
    {
        public override void Bake(SpawnerAuthoring authoring)
        {
            if (authoring.EnemyPrefab == null)
            {
                Debug.LogWarning("SpawnerAuthoring: EnemyPrefab not assigned!");
                return;
            }

            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new SpawnerComponent
            {
                EnemyPrefab    = GetEntity(authoring.EnemyPrefab, TransformUsageFlags.Dynamic),
                SpawnRate      = authoring.SpawnRate,
                SpawnTimer     = 0f,
                SpawnBatchSize = authoring.BatchSize,
                EnemyMaxHealth = authoring.EnemyMaxHealth,
                EnemyLifetime  = authoring.EnemyLifetime
            });
        }
    }
}