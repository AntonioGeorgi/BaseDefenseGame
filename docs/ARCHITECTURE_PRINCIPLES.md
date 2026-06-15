# Architecture Principles

This project should be designed as a modular ECS/DOTS gameplay sandbox. Future AI agents should preserve composition-first architecture unless there is a strong technical reason not to.

## Core Rule

Use ECS components and systems as the runtime gameplay foundation.

Use prefabs, authoring MonoBehaviours, and ScriptableObjects only as editor/configuration tools that produce entity data. Do not put high-count per-frame gameplay behavior into ScriptableObjects or inheritance-heavy MonoBehaviour hierarchies.

## Preferred Mental Model

- Tags = identity/category.
- Components = data, state, capability, or configuration.
- Systems = behavior over entities matching component queries.
- Events/requests = decoupled communication between systems.
- Authoring objects = editor convenience only.

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
- Specific behavior components
```

New gameplay should usually be added by adding components and systems, not by subclassing existing gameplay objects.

## Runtime Behavior Changes

Entities should be able to change behavior at runtime through component state, enableable components, tags, or structural changes.

Examples:

- Enemy below a health threshold gains/activates a different targeting behavior.
- Boss gains a spawn-over-time ability during a phase.
- Unit switches from moving toward the base to attacking a nearby blocking structure.

For performance, prefer:

- Separate systems for meaningfully different behavior.
- Enableable components for behavior that frequently turns on/off.
- Data components for configurable behavior thresholds.
- Tags for broad filtering.

Avoid per-frame giant switch statements over entity type when component queries can express the behavior more directly.

## Identity, Factions, and Targetability

Do not hardcode the game as only `Player` vs `Enemy` unless absolutely temporary.

The architecture should support future factions such as:

- Player
- Enemy
- Neutral
- Wildlife
- Ally
- Enemy subfactions

Use general concepts such as:

```text
Faction
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
```

Tags are useful, but tags alone are not enough. Use tags for identity and filtering; use data components for configurable rules.

## Generic Systems Over Role-Specific Systems

Prefer generic systems that can be reused by enemies, allies, neutral units, turrets, traps, projectiles, and abilities.

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

## Event/Request Pattern

Use events/requests to decouple systems.

Examples:

```text
DamageEvent
DeathEvent
SpawnRequest
FireRequest
PathRequest
TargetRequest
TargetResult
ExplosionEvent
```

This prevents every attack, projectile, enemy, or ability system from directly knowing how health, death, spawning, rewards, or pathfinding work.

## Weapons as Modular Child/Linked Entities

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

## ScriptableObjects

ScriptableObjects may be used for authoring/configuration, such as:

- Enemy definitions.
- Weapon definitions.
- Structure definitions.
- Spawn/wave definitions.
- Balance values.

They should not be the runtime source of per-frame gameplay behavior for large numbers of entities.

At runtime, important values should be converted into ECS components.
