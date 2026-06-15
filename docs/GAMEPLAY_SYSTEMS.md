# Gameplay Systems

This document describes the intended future gameplay system architecture. It is not a description of the current project state.

## 1. Entity Identity Layer

All major gameplay entities should use shared identity and relationship concepts.

Expected concepts:

```text
Faction
Targetable
TargetLayer / TargetCategory
Owner / Source entity
Team hostility rules
```

This should allow enemies, allies, neutral units, structures, terrain objects, summons, traps, and projectiles to interact through common rules.

## 2. Targeting Layer

Targeting should be reusable across turrets, enemies, allies, neutral creatures, traps, drones, and abilities.

A system should be able to express requests such as:

```text
Find closest enemy in range.
Find closest structure.
Find command center.
Find lowest-health target.
Find target matching faction/layer/category rules.
Find blocking obstacle.
```

Prefer a generic flow:

```text
TargetRequest -> AcquireTargetSystem -> CurrentTarget / TargetResult
```

Targeting implementation may start simple, such as brute-force scans, but should be replaceable with spatial partitioning, physics queries, grids, or other acceleration structures later.

Movement target and attack target should not be assumed to be the same. An entity may move toward the command center while temporarily attacking a blocking obstacle.

## 3. Movement and Pathing Layer

The game is intended to use open-field movement with player-placed obstacles and possibly generated/destructible terrain.

Movement should be abstracted so simple movement can later be replaced with dynamic pathfinding.

Recommended concepts:

```text
DesiredDestination
MovementSpeed
MovementMode
PathRequest
CurrentPath / NextWaypoint
StopDistance
ObstacleAvoidance
PathInvalidation
```

Start with direct movement if useful, but route it through stable movement data so later pathing can replace the internals.

Movement modes should allow future extension:

```text
Ground
Flying
Burrowing
Other future modes
```

Ground units may be blocked by obstacles. Flying units may ignore some obstacles. Burrowing units may ignore surface obstacles but respect other blockers if implemented.

Do not make every entity recalculate paths every frame. Future dynamic pathing should support:

```text
Repath intervals
Blocked-path detection
Dirty navigation regions
Obstacle placement/destruction invalidation
Staggered path requests over multiple frames
```

Player-built obstacles may be allowed to block paths, but gameplay should handle this. Ground enemies may retarget and attack blocking structures instead of failing permanently.

Soft avoidance is preferred over strict physical body-blocking for large enemy counts unless a specific gameplay need requires hard blocking.

## 4. Combat and Weapon Layer

Weapons should be modular and not limited to bullet projectiles.

Shared weapon concepts may include:

```text
WeaponSlot
WeaponCooldown
FirePoint
FireDirection
Range
Target requirement
Owner / Source
Faction
Damage payload
FireRequest
```

Possible weapon behaviors:

```text
Projectile weapon
Missile weapon
Beam weapon
Cone/flamethrower weapon
Aura weapon
Melee weapon
Trap weapon
Chain weapon
Spawner weapon
Status-effect weapon
```

Targeting and rotation should be reusable independent of the specific weapon output.

Do not force all attacks to use projectile hitboxes. A flamethrower may be a cone/area effect. A beam may apply continuous or instant line damage. An aura may apply periodic effects in a radius.

## 5. Projectile and Effect Layer

Projectiles/effects should be entities when performance and modularity benefit from it.

Projectile-like entities may use:

```text
Position
Direction
Speed
Lifetime / MaxDistance
Hit radius or collision data
Damage payload
Owner / Source
Faction
OnHit behavior
OnExpire behavior
```

Projectile systems should not own all damage/death logic. They should create damage events, explosion events, spawn requests, or status-effect requests as needed.

Examples:

```text
Bullet hit -> DamageEvent
Missile hit -> DamageEvent + ExplosionEvent
Missile explosion -> SpawnRequest for fire patches
Fire patch -> periodic DamageEvent
```

## 6. Damage Layer

Damage should be event-based.

Preferred flow:

```text
Attack/hit/effect system creates DamageEvent.
DamageSystem validates target and applies modifiers.
Health is changed.
DeathEvent is emitted if health reaches zero.
DeathSystem handles destruction/rewards/death effects/spawn-on-death.
```

This keeps attack types independent from health, armor, shields, resistances, death rewards, and future special reactions.

Future damage system may support:

```text
Armor
Shields
Resistances
Damage types
Critical hits
Status effects
Lifesteal
Thorns/reflect damage
Damage numbers
On-damaged triggers
Health thresholds
```

## 7. Spawning Layer

Entities should be able to spawn other entities through a centralized spawn request system.

Preferred flow:

```text
Any system creates SpawnRequest.
SpawnSystem processes requests.
SpawnSystem instantiates/configures entities.
```

Spawn requests may come from:

```text
Wave system
Enemy ability
Projectile impact
Explosion
Death event
Timed spawner
Boss phase
Structure ability
Terrain event
```

Examples:

```text
Enemy walks and periodically spawns minions.
Missile explodes and spawns fire patches.
Boss dies and spawns smaller enemies.
Structure deploys drones.
```

Centralized spawning allows later support for spawn limits, ownership, pooling, spawn VFX, faction inheritance, and performance controls.

## 8. Terrain and Obstacle Layer

The map may include generated terrain, destructible terrain, and player-placed obstacles.

Terrain/obstacles should be represented in a way that can affect targeting, movement, damage, and pathing.

Expected concepts:

```text
Obstacle
Destructible
BlocksMovement
BlocksLineOfSight
TargetableStructure
NavigationDirtyRegion
TerrainHealth
```

Obstacle creation/destruction should be able to invalidate nearby paths without forcing global recalculation every frame.

## 9. Ability and Trigger Layer

Abilities should be built from reusable triggers and effects where possible.

Example triggers:

```text
OnHealthBelowThreshold
OnDeath
OnSpawn
OnHit
OnDamaged
OnCooldownReady
OnTargetInRange
PeriodicTick
```

Example effects:

```text
Create DamageEvent
Create SpawnRequest
Change TargetRequest
Apply status effect
Enable/disable component
Create area effect
Modify movement speed
Destroy self
```

A low-health enemy that changes behavior should be implemented as a trigger causing a component/state change, not as a special subclass.
