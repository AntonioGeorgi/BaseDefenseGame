using Unity.Mathematics;

/// <summary>Pure, Burst-compatible movement rules shared by systems and tests.</summary>
public static class MovementMath
{
    public static float3 CalculateDesiredVelocity(
        float3 position,
        float3 target,
        float maxSpeed,
        float stopDistance,
        float deltaTime)
    {
        float distance = math.sqrt(GroundPlane.DistanceSq(position, target));
        float remainingDistance = math.max(0f, distance - math.max(0f, stopDistance));
        if (remainingDistance <= math.EPSILON || deltaTime <= math.EPSILON)
            return float3.zero;

        // The final step is shortened so movement cannot overshoot the stop radius.
        float speed = math.min(math.max(0f, maxSpeed), remainingDistance / deltaTime);
        return GroundPlane.Direction(position, target) * speed;
    }

    public static float3 ApplyTurning(
        float3 currentVelocity,
        float3 desiredVelocity,
        float turnSpeedDegrees,
        float deltaTime)
    {
        float desiredSpeed = math.length(GroundPlane.Project(desiredVelocity));
        if (desiredSpeed <= math.EPSILON)
            return float3.zero;

        float3 desiredDirection = GroundPlane.Project(desiredVelocity) / desiredSpeed;
        float3 currentDirection = math.normalizesafe(
            GroundPlane.Project(currentVelocity),
            desiredDirection);

        float dot = math.clamp(math.dot(currentDirection, desiredDirection), -1f, 1f);
        float signedAngle = math.atan2(
            currentDirection.x * desiredDirection.z - currentDirection.z * desiredDirection.x,
            dot);
        float maxTurn = math.radians(math.max(0f, turnSpeedDegrees)) * math.max(0f, deltaTime);
        float appliedAngle = math.clamp(signedAngle, -maxTurn, maxTurn);
        float sin = math.sin(appliedAngle);
        float cos = math.cos(appliedAngle);

        return new float3(
            currentDirection.x * cos - currentDirection.z * sin,
            0f,
            currentDirection.x * sin + currentDirection.z * cos) * desiredSpeed;
    }
}
