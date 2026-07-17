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

