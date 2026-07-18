using NUnit.Framework;
using Unity.Mathematics;

public class MovementMathTests
{
    private const float DeltaTime = 0.1f;

    [Test]
    public void OneUnit_ReachesStopDistanceWithoutOvershooting()
    {
        float3 position = float3.zero;
        float3 target = new float3(1f, 0f, 0f);

        for (int frame = 0; frame < 20; frame++)
        {
            float3 desired = MovementMath.CalculateDesiredVelocity(
                position, target, 4f, 0.2f, DeltaTime);
            position = GroundPlane.Advance(position, desired, DeltaTime);
        }

        Assert.That(GroundPlane.DistanceSq(position, target), Is.EqualTo(0.04f).Within(0.0001f));
    }

    [Test]
    public void ManyUnits_KeepIndependentTargetsAndHeights()
    {
        const int unitCount = 256;

        for (int unit = 0; unit < unitCount; unit++)
        {
            float height = unit * 0.01f;
            float3 position = new float3(-unit, height, unit);
            float3 target = new float3(unit + 1f, -100f, -unit);
            float3 desired = MovementMath.CalculateDesiredVelocity(
                position, target, 3f, 0.1f, DeltaTime);
            float3 next = GroundPlane.Advance(position, desired, DeltaTime);

            Assert.That(next.y, Is.EqualTo(height));
            Assert.That(GroundPlane.DistanceSq(next, target), Is.LessThan(GroundPlane.DistanceSq(position, target)));
        }
    }

    [Test]
    public void InsideStopDistance_DesiredVelocityIsZero()
    {
        float3 desired = MovementMath.CalculateDesiredVelocity(
            float3.zero, new float3(0.1f, 50f, 0f), 10f, 0.2f, DeltaTime);

        Assert.That(desired, Is.EqualTo(float3.zero));
    }

    [Test]
    public void Turning_IsLimitedPerUnitAndPreservesDesiredSpeed()
    {
        float3 current = new float3(0f, 0f, 2f);
        float3 desired = new float3(2f, 0f, 0f);

        float3 slow = MovementMath.ApplyTurning(current, desired, 45f, 1f);
        float3 fast = MovementMath.ApplyTurning(current, desired, 180f, 1f);

        Assert.That(math.length(slow), Is.EqualTo(2f).Within(0.0001f));
        Assert.That(slow.x, Is.EqualTo(math.sqrt(2f)).Within(0.0001f));
        Assert.That(slow.z, Is.EqualTo(math.sqrt(2f)).Within(0.0001f));
        Assert.That(fast.x, Is.EqualTo(2f).Within(0.0001f));
        Assert.That(fast.z, Is.EqualTo(0f).Within(0.0001f));
    }
}
