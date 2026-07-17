using Unity.Entities;
using Unity.Mathematics;

public struct MoveTarget : IComponentData
{
    public float3 TargetPosition;
}
public struct MovementStats : IComponentData
{
    public float MaxSpeed;
    public float TurnSpeed; // Degrees per second.
    public float StopDistance;
}
public struct MovementState : IComponentData
{
    public float3 Velocity;
    public float3 DesiredVelocity;
}

public struct GameBootstrapConfig : IComponentData
{
    public Entity MovingActorPrefab;
    public int ActorCount;
    public int ActorsPerRow;
    public float Spacing;
    public float3 SpawnOrigin;
    public float3 TargetPosition;
}

/// <summary>Marks a bootstrap config after its initial actors have been created.</summary>
public struct GameInitialized : IComponentData { }

/// <summary>
/// The single deterministic random stream used to derive independent streams
/// for systems and entities. Gameplay systems should not all copy this stream.
/// </summary>
public struct WorldRandomState : IComponentData
{
    public Random Random;
}

/// <summary>Immutable spawning rules baked from SpawnerAuthoring.</summary>
public struct SpawnerConfig : IComponentData
{
    public Entity Prefab;
    public float Interval;
    public int BatchSize;
    public float SpawnRadius;
}

/// <summary>
/// Runtime spawning state. Disable this component to pause the spawner without
/// removing its configuration.
/// </summary>
public struct SpawnerState : IComponentData
{
    public float TimeUntilNextSpawn;
    public Random Random;
}

/// <summary>Enable or disable this component to resume or pause spawning.</summary>
public struct SpawnerEnabled : IComponentData, IEnableableComponent { }

/// <summary>Removed after a spawner receives its derived random seed.</summary>
public struct SpawnerNeedsRandomSeed : IComponentData { }
