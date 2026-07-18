using NUnit.Framework;
using Unity.Mathematics;

/// <summary>Regression coverage for the pseudo-2D X/Z movement invariant.</summary>
public class GroundPlaneTests
{
    [Test]
    public void Project_RemovesVerticalComponent()
    {
        float3 result = GroundPlane.Project(new float3(2f, 99f, -4f));

        Assert.That(result, Is.EqualTo(new float3(2f, 0f, -4f)));
    }

    [Test]
    public void Advance_PreservesHeight_WhenVelocityContainsVerticalMovement()
    {
        float3 start = new float3(10f, 7.5f, -3f);
        float3 velocity = new float3(4f, 1_000f, -2f);

        float3 result = GroundPlane.Advance(start, velocity, 0.5f);

        Assert.That(result.x, Is.EqualTo(12f).Within(0.0001f));
        Assert.That(result.y, Is.EqualTo(start.y));
        Assert.That(result.z, Is.EqualTo(-4f).Within(0.0001f));
    }

    [Test]
    public void Advance_PreservesEveryActorsOwnHeight_AcrossManyFrames()
    {
        const int actorCount = 256;
        const int frameCount = 300;
        const float deltaTime = 1f / 60f;

        var positions = new float3[actorCount];
        var initialHeights = new float[actorCount];

        for (int actor = 0; actor < actorCount; actor++)
        {
            float height = actor * 0.125f - 8f;
            positions[actor] = new float3(actor, height, -actor);
            initialHeights[actor] = height;
        }

        for (int frame = 0; frame < frameCount; frame++)
        {
            for (int actor = 0; actor < actorCount; actor++)
            {
                // Deliberately inject vertical velocity. Ground-plane movement must ignore it.
                float3 velocity = new float3(
                    actor % 7 - 3f,
                    frame - 150f,
                    actor % 11 - 5f);

                positions[actor] = GroundPlane.Advance(
                    positions[actor],
                    velocity,
                    deltaTime);
            }
        }

        for (int actor = 0; actor < actorCount; actor++)
        {
            Assert.That(
                positions[actor].y,
                Is.EqualTo(initialHeights[actor]),
                $"Actor {actor} changed presentation height.");
        }
    }
}
