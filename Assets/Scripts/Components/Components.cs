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

