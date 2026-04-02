// EntityPool.cs
using Unity.Collections;
using Unity.Entities;

/// <summary>
/// A singleton component that lives on one "manager" entity.
/// Holds the queue of disabled (pooled) enemy entities.
/// </summary>
public struct EnemyPoolComponent : IComponentData
{
    // NativeQueue is thread-safe for push/pop between systems
    public NativeQueue<Entity> Available;
}

/// <summary>
/// Tag added to entities that are currently pooled (inactive).
/// Lets us query ONLY pooled entities if we ever need to inspect the pool.
/// </summary>
public struct PooledTag : IComponentData { }