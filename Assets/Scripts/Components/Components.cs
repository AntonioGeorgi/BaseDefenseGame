// Components.cs
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

// ─── Identity / Tags ───────────────────────────────────────────────────────

public struct CommandBuildingTag : IComponentData { }

public enum Faction : byte
{
    Player,
    Enemy,
    Neutral
}

public struct FactionComponent : IComponentData
{
    public Faction faction;
}

/// <summary>
/// Added by SpawnerSystem when a pooled enemy is re-enabled.
/// Consumed immediately by EnemyInitSystem which resets all state.
/// Think of this as the DOTS equivalent of OnEnabled.
/// </summary>
public struct PendingInitTag : IComponentData { }


// ─── Core Stats ────────────────────────────────────────────────────────────

/// <summary>
/// Universal health component. Works on enemies, turrets, and the Command Building.
/// Extensible: add shields, armor types later without touching this struct.
/// </summary>
public struct HealthComponent : IComponentData
{
    public float current;
    public float max;
    public bool isDead => current <= 0f;
}

/// <summary>
/// How fast this entity moves. Pure scalar — direction is computed by systems.
/// Add "SpeedMultiplier" here later for terrain slow effects.
/// </summary>
public struct MovingComponent : IComponentData
{
    public float speed; // units/second
    // TODO delete this as it's 
    public Vector3 direction; // normalized vector
}


public struct TurnComponent : IComponentData
{
    public float turnSpeedDegrees;
    
    public float3 currentDirection; // normalized vector
    public float3 desiredDirection; // normalized vector
    
}

// ─── Targeting ─────────────────────────────────────────────────────────────

/// <summary>
/// Stores the closest in-range target found by the TargetingSystem.
/// Written each frame by TargetingSystem, read by TurretRotationSystem and FireSystem.
/// </summary>
public struct TargetComponent : IComponentData
{
    public float3 TargetPosition;
}

/// <summary>
/// </summary>
public struct WeaponComponent : IComponentData
{
    public float range;
    public float damagePerHit;
    public float cooldown;
    public float timeSinceLastAttack;
}

public struct AimComponent : IComponentData
{
    public float alignmentAngle; // allowed error in degrees / half the total cone shaped attack area
    public bool isAligned;
}

public struct MeleeWeaponComponent : IComponentData
{
    public float hitRadius;
}

public struct ProjectileWeaponComponent : IComponentData
{
    public Entity projectilePrefab;
    public float projectileSpeed;
    public float3 spawnOffset; // where the projectile spawns relative to the firing entity's position
}


// ─── Spawning ──────────────────────────────────────────────────────────────

/// <summary>
/// Attached to Spawner entities at map edges.
/// SpawnerSystem reads this to decide when/what to spawn.
/// </summary>
public struct SpawnerComponent : IComponentData
{
    public Entity enemyPrefab;
    public float  spawnRate;
    public float  spawnTimer;
    public int    spawnBatchSize;
}


/// <summary>
/// Counts down to zero then the entity is returned to the pool.
/// Remove this component in production — turrets handle killing enemies then.
/// </summary>
public struct LifetimeComponent : IComponentData
{
    public float secondsRemaining;
    public float maxLifetime;       // stored so we can reset on pool reuse
}


/// <summary>
/// All data a projectile needs to move and deal damage.
/// Written at spawn time by TurretFireSystem.
/// </summary>
public struct ProjectileComponent : IComponentData
{
    public float3 direction;      // normalized, set at spawn
    public float  speed;          // world units per second
    public float  damage;         // damage on impact
    public float  maxRange;       // despawn after traveling this far
    public float  distanceTraveled;
    public float  hitRadius;        // for collision detection, set at spawn or hardcoded in ProjectileMovementSystem
}