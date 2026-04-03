// EnemyDataSO.cs
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "BaseDefense/Enemy Data")]
public class EnemyDataSO : ScriptableObject
{
    [Header("Health")]
    public float MaxHealth = 50f;

    [Header("Movement")]
    public float MoveSpeed = 3.5f;

    [Header("Melee Attack")]
    [Tooltip("Damage dealt per second while in contact range")]
    public float DamagePerSecond = 10f;
    [Tooltip("Distance at which the enemy stops moving and starts attacking")]
    public float MeleeRange = 2.5f;

    [Header("Lifetime (test feature — remove when turrets kill enemies)")]
    public float Lifetime = 8f;
}