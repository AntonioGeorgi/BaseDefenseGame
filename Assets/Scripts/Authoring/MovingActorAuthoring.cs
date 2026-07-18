using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public sealed class MovingActorAuthoring : MonoBehaviour
{
    [Header("Destination")]
    public Vector3 TargetPosition = new Vector3(5f, 0f, 5f);

    [Header("Movement")]
    [Min(0f)] public float MaxSpeed = 3.5f;
    [Min(0f)] public float TurnSpeed = 180f;
    [Min(0f)] public float StopDistance = 0.1f;

    [Header("Spatial")]
    [Min(0f)] public float PersonalSpaceRadius = 0.5f;

    private sealed class Baker : Baker<MovingActorAuthoring>
    {
        public override void Bake(MovingActorAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new MoveTarget
            {
                TargetPosition = (float3)authoring.TargetPosition
            });
            AddComponent(entity, new MovementStats
            {
                MaxSpeed = authoring.MaxSpeed,
                TurnSpeed = authoring.TurnSpeed,
                StopDistance = authoring.StopDistance
            });
            AddComponent(entity, new MovementState
            {
                Velocity = float3.zero,
                DesiredVelocity = float3.zero
            });
            AddComponent(entity, new PersonalSpace
            {
                Radius = math.max(0f, authoring.PersonalSpaceRadius)
            });
        }
    }
}
