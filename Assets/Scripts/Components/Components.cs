// Components.cs
using Unity.Entities;
using Unity.Mathematics;

// ─── Identity / Tags ───────────────────────────────────────────────────────

/// <summary>Tag: this entity is an Enemy.</summary>
public struct EnemyTag : IComponentData { }

/// <summary>Tag: this entity is the Command Building (the base).</summary>
public struct CommandBuildingTag : IComponentData { }

/// <summary>Tag: this entity is a Turret Base (static, never moves).</summary>
public struct TurretBaseTag : IComponentData { }

/// <summary>Tag: this entity is a Turret Mount (rotates on Y to face target).</summary>
public struct TurretMountTag : IComponentData { }

/// <summary>Tag: this entity is a Turret Barrel (fires projectiles).</summary>
public struct TurretBarrelTag : IComponentData { }

// ─── Core Stats ────────────────────────────────────────────────────────────

/// <summary>
/// Universal health component. Works on enemies, turrets, and the Command Building.
/// Extensible: add shields, armor types later without touching this struct.
/// </summary>
public struct HealthComponent : IComponentData
{
    public float Current;
    public float Max;
    public bool IsDead => Current <= 0f;
}

/// <summary>
/// How fast this entity moves. Pure scalar — direction is computed by systems.
/// Add "SpeedMultiplier" here later for terrain slow effects.
/// </summary>
public struct MovementSpeedComponent : IComponentData
{
    public float Value; // units/second
}

// ─── Targeting ─────────────────────────────────────────────────────────────

/// <summary>
/// Stores the closest in-range target found by the TargetingSystem.
/// Written each frame by TargetingSystem, read by TurretRotationSystem and FireSystem.
/// </summary>
public struct TargetComponent : IComponentData
{
    public Entity Value;       // Entity.Null = no target
    public float3 LastKnownPosition;
    public bool HasTarget => Value != Entity.Null;
}

/// <summary>
/// Defines a turret's weapon properties.
/// Decouple this from TargetComponent so you can have non-firing watchtowers.
/// </summary>
public struct TurretWeaponComponent : IComponentData
{
    public float Range;            // detection + firing radius
    public float FireRate;         // shots per second
    public float Damage;
    public float ProjectileSpeed;
    public float  MountRotationSpeed;   // degrees per second
    public float TimeSinceLastShot; // internal timer, mutated by FireSystem
    public Entity ProjectilePrefab; // set via authoring
}

/// <summary>
/// Links a TurretMount or TurretBarrel back to its parent TurretBase.
/// This is your scene hierarchy replacement in DOTS.
/// </summary>
public struct TurretPartComponent : IComponentData
{
    public Entity BaseEntity;
    public Entity MountEntity;
    public Entity FirePointEntity; // world forward of this = bullet direction
}

// ─── Movement ──────────────────────────────────────────────────────────────

/// <summary>
/// Enemies move toward this world-space position.
/// Populated once at spawn from the Command Building's position.
/// Replace "Value" with a pathfinding waypoint index later (waypointBuffer[pathfinding.CurrentWaypointIndex]).
/// </summary>
public struct MoveTargetComponent : IComponentData
{
    public float3 Value; // world position of Command Building
}

// ─── Spawning ──────────────────────────────────────────────────────────────

/// <summary>
/// Attached to Spawner entities at map edges.
/// SpawnerSystem reads this to decide when/what to spawn.
/// </summary>
public struct SpawnerComponent : IComponentData
{
    public Entity EnemyPrefab;
    public float  SpawnRate;
    public float  SpawnTimer;
    public int    SpawnBatchSize;
}

/// <summary>
/// Attached to ???.
/// ??? reads this to decide when/what to ???.
/// Adds a damage-on-contact component and a simple overlap check:
/// </summary>
public struct MeleeDamageComponent : IComponentData
{
    public float DamagePerSecond;
    public float MeleeRange; // melee reach
}

/// <summary>
/// Counts down to zero then the entity is returned to the pool.
/// Remove this component in production — turrets handle killing enemies then.
/// </summary>
public struct LifetimeComponent : IComponentData
{
    public float SecondsRemaining;
    public float MaxLifetime;       // stored so we can reset on pool reuse
}

/// <summary>
/// Added by SpawnerSystem when a pooled enemy is re-enabled.
/// Consumed immediately by EnemyInitSystem which resets all state.
/// Think of this as the DOTS equivalent of OnEnabled.
/// </summary>
public struct PendingInitTag : IComponentData { }

/// <summary>
/// Marks this entity as an active projectile.
/// </summary>
public struct ProjectileTag : IComponentData { }

/// <summary>
/// All data a projectile needs to move and deal damage.
/// Written at spawn time by TurretFireSystem.
/// </summary>
public struct ProjectileComponent : IComponentData
{
    public float3 Direction;      // normalized, set at spawn
    public float  Speed;          // world units per second
    public float  Damage;         // damage on impact
    public float  MaxRange;       // despawn after traveling this far
    public float  DistanceTraveled;
    public float  HitRadius;        // for collision detection, set at spawn or hardcoded in ProjectileMovementSystem
}

/// <summary>
/// Replaces FirePointTag. Stores which local axis points out of the barrel.
/// Set LocalFireAxis to (1,0,0) if barrel was modelled along X in Blender.
/// Set to (0,0,1) if barrel was modelled along Z (Unity default forward).
/// </summary>
public struct FirePointComponent : IComponentData
{
    public float3 LocalFireAxis;
}

// ─── Future Extension Placeholders (add these when ready) ──────────────────
// public struct EnergyConsumerComponent : IComponentData { public float DrainPerSecond; }
// public struct PathfindingComponent     : IComponentData { public int CurrentWaypointIndex; }
// public struct SlowEffectComponent      : IComponentData { public float Multiplier; public float Duration; }