// TurretBarrelAuthoring.cs
using Unity.Entities;
using UnityEngine;

public class TurretBarrelAuthoring : MonoBehaviour
{
    public GameObject BaseObject;
    public GameObject MountObject;
    [Tooltip("Drag the FirePoint child here")]
    public GameObject FirePointObject;

    class Baker : Baker<TurretBarrelAuthoring>
    {
        public override void Bake(TurretBarrelAuthoring authoring)
        {
            if (authoring.BaseObject  == null ||
                authoring.MountObject == null ||
                authoring.FirePointObject == null)
            {
                Debug.LogError($"TurretBarrelAuthoring on '{authoring.name}': " +
                               "All three object slots must be assigned.");
                return;
            }

            var barrelEntity    = GetEntity(TransformUsageFlags.Dynamic);
            var baseEntity      = GetEntity(authoring.BaseObject,      TransformUsageFlags.Dynamic);
            var mountEntity     = GetEntity(authoring.MountObject,     TransformUsageFlags.Dynamic);
            var firePointEntity = GetEntity(authoring.FirePointObject, TransformUsageFlags.Dynamic);

            AddComponent(barrelEntity, new TurretBarrelTag());
            AddComponent(barrelEntity, new TurretPartComponent
            {
                BaseEntity      = baseEntity,
                MountEntity     = mountEntity,
                FirePointEntity = firePointEntity
            });
        }
    }
}