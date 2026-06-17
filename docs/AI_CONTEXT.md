# AI Context

This is the entry point for AI assistants working on BaseDefenseGame.

Read this file first, then read the other docs as needed.

## Project Summary

BaseDefenseGame is a performance-first Unity DOTS/ECS 3D open-field base defense game.

The player places structures and obstacles to defend a central base or command structure against enemies.

The project should support future expansion into hordes, elite squads, bosses, neutral entities, summons, destructible or generated terrain, modular weapons, and other gameplay experiments.

## Required Read Order

Before modifying code, read:

1. `PROJECT_VISION.md`
2. `CODE_OVERVIEW.md`
3. `ARCHITECTURE_PRINCIPLES.md`
4. `GAMEPLAY_SYSTEMS.md`
5. `AI_DOC_UPDATE_RULES.md`

Only read files outside `docs/` when the user gives permission.

## Most Important Rules

1. Prefer ECS composition over inheritance.
2. Prefer reusable systems over enemy-specific or turret-specific systems.
3. Prefer data components, tags, factions, and target categories over hardcoded object types.
4. Prefer event/request flows for damage, spawning, targeting, firing, and pathing.
5. Preserve performance-first DOTS patterns.
6. Start simple behind stable abstractions when needed.
7. Clearly separate current implementation from planned architecture.
8. Do not treat early placeholder code as final architecture.

## Common Mistakes to Avoid

Do not build the game around fixed subclasses such as:

```text
FastEnemy
TankEnemy
FlamethrowerTurret
SniperTurret
```

Do not put high-count runtime behavior into ScriptableObjects or MonoBehaviour inheritance hierarchies.

Do not assume:

* The game is only player versus enemy.
* Every attack is a bullet or projectile.
* Movement target and attack target are always the same.
* All movement is ground movement.
* Obstacles are static.
* Every turret has only one weapon.
* Every enemy only attacks the command center.

## Design Priority

When choosing between two valid designs, prefer:

1. Runtime performance and scalability.
2. Long-term modularity.
3. Stable abstractions that allow simple internals to be replaced later.
4. Editor convenience.
5. Readability.

Readability matters, but performance and modular extensibility have higher priority in this project.

## Documentation Rule

When code changes affect future development, update the relevant docs.

Use `AI_DOC_UPDATE_RULES.md` to decide what should be updated.
