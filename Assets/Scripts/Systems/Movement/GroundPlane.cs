using Unity.Mathematics;

/// <summary>
/// Shared math for gameplay that is simulated on the X/Z plane.
/// Y is deliberately excluded from gameplay directions and distances.
/// </summary>
public static class GroundPlane
{
    public static float3 Project(float3 value)
    {
        return new float3(value.x, 0f, value.z);
    }

    public static float DistanceSq(float3 from, float3 to)
    {
        return math.lengthsq(Project(to - from));
    }

    public static float3 Direction(float3 from, float3 to)
    {
        return math.normalizesafe(Project(to - from));
    }

    public static float3 PreserveHeight(float3 position, float3 flatPosition)
    {
        flatPosition.y = position.y;
        return flatPosition;
    }

    /// <summary>
    /// Advances a position using only X/Z velocity and keeps its presentation height.
    /// </summary>
    public static float3 Advance(float3 position, float3 velocity, float deltaTime)
    {
        float3 nextPosition = position + Project(velocity) * deltaTime;
        return PreserveHeight(position, nextPosition);
    }
}
