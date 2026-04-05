// FirePointAuthoring.cs
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class FirePointAuthoring : MonoBehaviour
{
    public GameObject BaseObject;
    public GameObject MountObject;

    [Tooltip("Which local axis points out of the barrel.\n" +
             "X (1,0,0) = barrel modelled along X in Blender\n" +
             "Z (0,0,1) = barrel modelled along Z (Unity default)")]
    public Vector3 LocalFireAxis = new Vector3(1, 0, 0); // X — most common from Blender

    class Baker : Baker<FirePointAuthoring>
    {
        public override void Bake(FirePointAuthoring authoring)
        {
            if (authoring.BaseObject == null || authoring.MountObject == null)
            {
                Debug.LogError($"FirePointAuthoring on '{authoring.name}': " +
                               "BaseObject and MountObject must be assigned.");
                return;
            }

            float3 axis = math.normalizesafe(authoring.LocalFireAxis);
            if (math.lengthsq(axis) < 0.001f)
            {
                Debug.LogError($"FirePointAuthoring on '{authoring.name}': " +
                               "LocalFireAxis cannot be zero.");
                return;
            }

            var firePointEntity = GetEntity(TransformUsageFlags.Dynamic);
            var baseEntity      = GetEntity(authoring.BaseObject,  TransformUsageFlags.Dynamic);
            var mountEntity     = GetEntity(authoring.MountObject, TransformUsageFlags.Dynamic);

            AddComponent(firePointEntity, new FirePointComponent
            {
                LocalFireAxis = axis
            });
            AddComponent(firePointEntity, new TurretPartComponent
            {
                BaseEntity      = baseEntity,
                MountEntity     = mountEntity,
                FirePointEntity = firePointEntity
            });
        }
    }
}