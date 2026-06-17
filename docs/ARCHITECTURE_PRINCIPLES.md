# Architecture Principles

This project should be designed as a modular Unity DOTS/ECS gameplay sandbox.

Future work should preserve composition-first architecture unless there is a strong technical reason not to.

## Core Rule

Use ECS components and systems as the runtime gameplay foundation.

Use prefabs, authoring MonoBehaviours, Bakers, and ScriptableObjects as editor/configuration tools that produce entity data.

Do not put high-count per-frame gameplay behavior into ScriptableObjects or inheritance-heavy MonoBehaviour hierarchies.

## Preferred Mental Model

```text
Tags = identity/category.
Components = data, state, capability, or configuration.
Systems = behavior over entities matching component queries.
Events/requests = decoupled communication between systems.
Authoring objects = editor convenience that feeds ECS data.
```

## Composition Over Inheritance

Avoid designs like:

```text
FastEnemy : Enemy
TankEnemy : Enemy
FlamethrowerTurret : Turret
SniperTurret : Turret
```

Prefer entity composition:

```text
Entity:
- Faction
- Targetable
- Health
- MovementSpeed
- DesiredDestination
- TargetRequest
- WeaponSlot
- WeaponCooldown
- Specific behavior components/tags
```

New gameplay should usually be added by adding components and systems, not by subclassing existing gameplay objects.

## Runtime Behavior Changes

Entities should be able to change behavior at runtime through component state, enableable components, tags, buffers, or structural changes.

Examples:

* An enemy below a health threshold activates different targeting behavior.
* A boss gains a spawn-over-time ability during a phase.
* A unit moves toward the base but temporarily attacks a blocking obstacle.
* A structure enables a second weapon module after an upgrade.

Prefer:

* Separate systems for meaningfully different behavior.
* Enableable components for behavior that frequently turns on and off.
* Data components for configurable behavior thresholds.
* Tags for broad filtering.
* Events/requests for decoupled communication.

Avoid giant per-frame switch statements over entity type when component queries can express the behavior more directly.

## Identity, Factions, and Targetability

Do not hardcode the game as only `Player` vs `Enemy`.

The architecture should allow future factions such as:

* Player
* Enemy
* Neutral
* Wildlife
* Ally
* Enemy subfactions
* Summoned/owned entities

Use general concepts such as:

```text
Faction
Owner / Source entity
Targetable
TargetLayer / TargetCategory
Hostility rules
```

Targets should be selected by data-driven category/faction rules, not by hardcoded class names.

Example target layers/categories:

```text
Enemy
Structure
CommandCenter
Neutral
Flying
Ground
Obstacle
Projectile
Terrain
```

Tags are useful for identity and filtering, but tags alone are not enough. Use data components for configurable rules.

## Generic Systems Over Role-Specific Systems

Prefer systems that can be reused by enemies, allies, neutral units, turrets, traps, projectiles, abilities, and terrain objects.

Prefer names/concepts like:

```text
AcquireTargetSystem
MoveToDestinationSystem
MoveAlongPathSystem
RotateToTargetSystem
WeaponCooldownSystem
ProjectileWeaponFireSystem
ConeWeaponFireSystem
MeleeAttackSystem
DamageSystem
DeathSystem
SpawnSystem
```

Avoid unnecessary role-specific systems like:

```text
EnemyMovementSystem
TurretOnlyTargetingSystem
SpecificEnemyAttackSystem
```

Role-specific systems are acceptable only when the behavior is genuinely unique and cannot be cleanly expressed with reusable components.

## Event and Request Pattern

Use events/requests to decouple systems.

Common examples:

```text
TargetRequest
TargetResult
PathRequest
FireRequest
DamageEvent
DeathEvent
SpawnRequest
ExplosionEvent
NavigationDirtyRegion
```

This prevents every attack, projectile, enemy, ability, or structure system from directly knowing how health, death, spawning, rewards, or pathfinding work.

Preferred flow examples:

```text
TargetRequest -> AcquireTargetSystem -> CurrentTarget / TargetResult

Attack/hit/effect system -> DamageEvent
DamageSystem -> health change
DamageSystem -> DeathEvent
DeathSystem -> destruction/rewards/death effects

Any system -> SpawnRequest
SpawnSystem -> instantiate/configure entity
```

## Targeting Layer

Targeting should be reusable across turrets, enemies, allies, neutral creatures, traps, drones, abilities, and terrain interactions.

A system should be able to express requests such as:

```text
Find closest enemy in range.
Find closest structure.
Find command center.
Find lowest-health target.
Find target matching faction/layer/category rules.
Find blocking obstacle.
```

Targeting may start with brute-force scans, but the request/result interface should allow replacement with spatial partitioning, physics queries, grids, or other acceleration structures later.

Movement target and attack target must not be assumed to be the same. An entity may move toward the command center while attacking a nearby obstacle.

## Movement and Pathing Layer

The game uses open-field movement with player-placed obstacles and possible generated/destructible terrain.

Movement should be abstracted so direct movement can later be replaced with dynamic pathfinding.

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
NavigationDirtyRegion
```

Movement modes should allow future extension:

```text
Ground
Flying
Burrowing
Other future modes
```

Ground units may be blocked by obstacles. Flying units may ignore some obstacles. Burrowing units may ignore surface obstacles but respect other blocker types if implemented.

Do not recalculate every path every frame. Future pathing should support:

```text
Repath intervals
Blocked-path detection
Dirty navigation regions
Obstacle placement/destruction invalidation
Staggered path requests over multiple frames
```

Player-built obstacles may block paths, but gameplay should handle this. Ground enemies may retarget and attack blocking structures instead of failing permanently.

For large enemy counts, soft avoidance is usually preferred over strict physical body-blocking unless a specific gameplay need requires hard blocking.

## Combat and Weapon Layer

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

Targeting, rotation, cooldown, and attack output should be separable.

A visible structure may contain multiple weapon modules. A turret should not be limited to one weapon by architecture.

Preferred model:

```text
TurretRoot:
- Structure identity
- Health
- Faction
- Placement/cost data

Weapon entity 1:
- Targeting data
- Rotation data
- Cooldown
- Fire behavior

Weapon entity 2:
- Targeting data
- Cooldown
- Different fire behavior
```

Multiple weapon entities can appear as one structure to the player.

## Projectile and Effect Layer

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

## Damage Layer

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

Future damage concepts may include:

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

## Spawning Layer

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

Centralized spawning allows later support for spawn limits, ownership, pooling, spawn VFX, faction inheritance, and performance controls.

## Terrain and Obstacle Layer

The map may include generated terrain, destructible terrain, and player-placed obstacles.

Terrain/obstacles should be represented in a way that can affect targeting, movement, damage, line of sight, and pathing.

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

Obstacle creation/destruction should invalidate nearby navigation locally instead of forcing global recalculation every frame.

## Ability and Trigger Layer

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

A low-health enemy that changes behavior should be implemented as a trigger causing a component or state change, not as a special subclass.

## ScriptableObjects

ScriptableObjects may be used for authoring/configuration, such as:

* Enemy definitions.
* Weapon definitions.
* Structure definitions.
* Spawn/wave definitions.
* Balance values.

They should not be the runtime source of per-frame behavior for large numbers of entities.

At runtime, important values should be converted into ECS components.
