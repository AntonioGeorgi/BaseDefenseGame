// TurretMountAuthoring.cs
using Unity.Entities;
using UnityEngine;

public class TurretMountAuthoring : MonoBehaviour
{
    [Tooltip("Drag the parent TurretBase GameObject here")]
    public GameObject BaseObject;

    class Baker : Baker<TurretMountAuthoring>
    {
        public override void Bake(TurretMountAuthoring authoring)
        {
            if (authoring.BaseObject == null)
            {
                Debug.LogError($"TurretMountAuthoring on '{authoring.name}': " +
                               "BaseObject must be assigned.");
                return;
            }

            var mountEntity = GetEntity(TransformUsageFlags.Dynamic);
            var baseEntity  = GetEntity(authoring.BaseObject, TransformUsageFlags.Dynamic);

            // Only touching mountEntity — this Baker's own entity
            AddComponent(mountEntity, new TurretMountTag());
            AddComponent(mountEntity, new TurretPartComponent
            {
                BaseEntity  = baseEntity,
                MountEntity = mountEntity
            });
        }
    }
}