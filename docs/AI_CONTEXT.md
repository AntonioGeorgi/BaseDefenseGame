# AI Context for Future Agents

Use this file as compact guidance when modifying or extending the project.

## Project Summary

This is a performance-first Unity DOTS/ECS 3D open-field base defense game. The player places structures and obstacles to defend a command/base structure against enemies. The game should support hordes, elite squads, bosses, neutral entities, summons, destructible/generated terrain, and many future gameplay experiments.

The architecture should maximize modularity through ECS composition while preserving performance.

## Core Architectural Intent

Do not build the game around fixed enemy/turret subclasses. Build it around reusable components and systems.

Preferred runtime model:

```text
Components define data/state/capabilities.
Systems define behavior for matching component sets.
Events/requests decouple systems.
Authoring/prefabs/ScriptableObjects only feed ECS data.
```

## Most Important Rules

1. Prefer ECS composition over inheritance.
2. Prefer generic systems over enemy/turret-specific systems.
3. Prefer components/tags/factions/target layers over hardcoded object types.
4. Prefer event/request flows for damage, spawning, targeting, firing, and pathing.
5. Preserve performance-first DOTS patterns.
6. Start simple behind stable abstractions when needed.
7. Do not force future features into current assumptions.

## Avoid These Mistakes

Do not create many subclasses such as `FastEnemy`, `TankEnemy`, `FlamethrowerTurret`, or `SniperTurret` as the main architecture.

Do not make ScriptableObjects perform high-count runtime behavior every frame.

Do not hardcode all combat as player-vs-enemy only.

Do not assume every attack is a bullet/projectile.

Do not assume movement target and attack target are always the same.

Do not assume all movement is ground movement.

Do not assume obstacles are static.

Do not build giant per-frame switch statements over unit type if component queries can express the behavior.

## Preferred Concepts

Use or preserve concepts like:

```text
Faction
Targetable
TargetLayer / TargetCategory
TargetRequest
CurrentTarget / TargetResult
DesiredDestination
MovementMode
PathRequest
CurrentPath / NextWaypoint
WeaponSlot
WeaponCooldown
FireRequest
DamageEvent
DeathEvent
SpawnRequest
Obstacle
NavigationDirtyRegion
HealthThresholdTrigger
```

## Targeting Guidance

Targeting should be reusable for turrets, enemies, allies, traps, drones, neutral creatures, and abilities.

Targeting should support faction/category/layer filtering and later performance acceleration. Start with simple scans if necessary, but preserve the interface so spatial partitioning can replace internals.

## Movement Guidance

The game uses open-field movement with player-placed obstacles and possible generated/destructible terrain.

Movement should be abstracted through destination/path data. Direct movement is acceptable early, but the architecture should allow dynamic pathfinding later.

Do not recalculate all paths every frame. Future pathing should support dirty regions, repath intervals, blocked-path detection, and staggered path requests.

Ground, flying, and burrowing movement may exist later. Keep movement mode extensible.

## Combat Guidance

Weapons should be modular. One visible structure may have multiple weapon entities/modules.

Weapon targeting, rotation, cooldown, and attack output should be separable.

Support different attack types:

```text
Projectile
Missile
Beam
Cone/flamethrower
Aura
Melee
Trap
Chain
Spawner
Status effect
```

Do not require all attacks to be projectile hitboxes.

## Damage Guidance

Use damage events.

Attack systems should create `DamageEvent` or related events. A centralized damage system should apply health changes and emit death events. This allows armor, shields, resistances, status effects, death triggers, rewards, and on-damaged behavior later.

## Spawning Guidance

Use centralized spawn requests.

Any entity/system may request spawning: waves, enemies, projectiles, deaths, bosses, structures, or abilities.

A centralized spawn system should handle instantiation/configuration. This supports future pooling, ownership, faction inheritance, limits, and VFX.

## Terrain/Obstacle Guidance

Obstacles and terrain may be generated, destructible, and player-placed.

They should be able to affect movement, targeting, line of sight, and combat. Their creation/destruction should invalidate navigation locally rather than forcing global recalculation.

## Decision Priority

When choosing between two designs, prefer in order:

1. Runtime performance and scalability.
2. Long-term modularity/extensibility.
3. Stable abstractions that can replace simple internals later.
4. Editor convenience.
5. Readability.

Readability matters, but the user prioritizes performance and modular extensibility over simple-looking code.
