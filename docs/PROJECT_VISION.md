# Project Vision

This project is a performance-first 3D open-field base defense game built with Unity DOTS/ECS.

The player defends a central base or command structure by placing defensive structures and obstacles in an open field. Enemies may arrive as hordes, elite squads, bosses, mixed formations, summoned units, or other future encounter types.

The game should support many active entities at once, including enemies, structures, projectiles, area effects, summoned units, neutral creatures, terrain objects, and destructible obstacles.

## Primary Goal

The main goal is long-term gameplay flexibility without sacrificing runtime performance.

New enemies, structures, weapons, projectiles, terrain interactions, and character behaviors should usually be created by composing reusable ECS components and systems, not by building isolated one-off class hierarchies.

The architecture should make it easy to support things like:

* Basic enemies that move toward the command center and attack it.
* Enemies that change targeting behavior below a health threshold.
* Enemies that spawn other enemies while moving.
* Turrets with one or multiple weapon modules.
* Flamethrowers, bullets, missiles, beams, auras, traps, melee attacks, and other weapon types.
* Neutral or allied entities that reuse movement, targeting, health, and combat systems.
* Ground, flying, burrowing, or other future movement modes.
* Destructible, generated, or player-placed terrain that affects movement and combat.

## Performance Philosophy

Performance is a core design constraint.

Favor DOTS/ECS-friendly, data-oriented design:

* Small `IComponentData` structs.
* Systems operating over explicit component queries.
* Burst/job-compatible logic where practical.
* Minimal per-frame virtual dispatch.
* Minimal inheritance-based runtime behavior.
* Minimal GameObject-heavy logic for high-count gameplay entities.
* Avoid large universal systems with many type branches when component queries can express behavior more clearly.

Readability matters, but it is secondary to performance, scalability, and correct architecture. The code may become technically dense if that enables better runtime behavior and long-term modularity.

## Development Philosophy

Start simple behind stable data contracts.

Early implementations may be naive if the surrounding architecture allows them to be replaced later.

Examples:

* Start with direct movement, but route movement through destination/path components so pathfinding can replace it later.
* Start with brute-force target search, but keep targeting behind request/result data so spatial partitioning can replace it later.
* Start with simple projectile movement, but do not design combat as if every attack must be a projectile.
* Start with basic spawning, but use spawn requests so pooling, limits, ownership, and VFX can be added later.

Current systems may be replaced when the project grows. The foundation should not be changed without a strong reason, but early code is not sacred.

## Non-Goals

Do not design around:

* A fixed list of enemy or turret subclasses.
* Player-vs-enemy-only combat.
* Command-center-only enemy behavior.
* Bullet/projectile-only weapons.
* Ground-only movement.
* Static-only obstacles.
* Two-faction-only gameplay.
* GameObject-heavy high-count runtime behavior.
* ScriptableObjects performing large amounts of per-frame gameplay logic.

The game direction is intentionally flexible. Avoid assumptions that make future systems harder to add.
