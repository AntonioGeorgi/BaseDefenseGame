// EnemyDataSO.cs
using UnityEngine;

[CreateAssetMenu(fileName = "MovingEntityData", menuName = "BaseDefense/MovingEntityData")]
public class MovingEntityDataSO : ScriptableObject
{
    [Header("Health")]
    public float MaxHealth = 50f;

    [Header("Movement")]
    public float MoveSpeed = 3.5f;

    [Header("Faction")]
    public Faction faction;

    public FactionComponent CreateFactionComponent()
    {
        return new FactionComponent
        {
            faction = faction
        };
    }

    public HealthComponent CreateHealthComponent()
    {
        return new HealthComponent
        {
            current = MaxHealth,
            max = MaxHealth
        };
    }
    public MovingComponent CreateMovingComponent()
    {
        return new MovingComponent
        {
            speed = MoveSpeed,
            direction = Vector3.forward
        };
    }

}