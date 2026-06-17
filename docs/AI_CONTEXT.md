# AI Context

Use this file as the compact entry point for future AI agents working on the project.

Read these docs first:

1. `PROJECT_VISION.md`
2. `ARCHITECTURE_PRINCIPLES.md`

## Project Summary

This is a performance-first Unity DOTS/ECS 3D open-field base defense game.

The player places structures and obstacles to defend a command/base structure against enemies. The game should support hordes, elite squads, bosses, neutral entities, summons, destructible/generated terrain, and future gameplay experiments.

## Most Important Rules

1. Prefer ECS composition over inheritance.
2. Prefer generic systems over enemy/turret-specific systems.
3. Prefer components, tags, factions, and target layers over hardcoded object types.
4. Prefer event/request flows for damage, spawning, targeting, firing, and pathing.
5. Preserve performance-first DOTS patterns.
6. Start simple behind stable abstractions when needed.
7. Do not force future features into current assumptions.

## Avoid These Mistakes

Do not create many subclasses such as:

```text
FastEnemy
TankEnemy
FlamethrowerTurret
SniperTurret
```

as the main architecture.

Do not:

* Make ScriptableObjects perform high-count per-frame runtime behavior.
* Hardcode combat as player-vs-enemy only.
* Assume every attack is a bullet/projectile.
* Assume movement target and attack target are always the same.
* Assume all movement is ground movement.
* Assume obstacles are static.
* Build giant per-frame switch statements over unit type when component queries can express behavior.

## Preferred Concepts

Preserve or introduce concepts like:

```text
Faction
Owner / Source entity
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

## Decision Priority

When choosing between two designs, prefer in this order:

1. Runtime performance and scalability.
2. Long-term modularity/extensibility.
3. Stable abstractions that can replace simple internals later.
4. Editor convenience.
5. Readability.

Readability matters, but performance and modular extensibility are higher priorities for this project.
