# Project Vision

This project is a performance-first 3D open-field base defense / tower defense game built around modular ECS/DOTS gameplay systems.

## Core Game Idea

The player defends a central base or command structure against enemy groups. Enemies may arrive as endless hordes, small elite squads, bosses, mixed formations, or other future encounter types. The player places defensive structures and obstacles in an open field to shape combat, delay enemies, and protect the base.

The game should support many diverse entities on screen, including enemies, defensive structures, projectiles, area effects, summoned units, neutral creatures, terrain objects, and destructible obstacles.

## Primary Design Goal

The main goal is long-term extensibility without sacrificing performance.

New enemies, structures, weapons, projectiles, terrain interactions, or character types should usually be created by composing reusable ECS components and systems, not by writing isolated one-off behavior classes.

The architecture should make it easy to create entities such as:

- Basic enemies that move toward the command center and attack it.
- Enemies that change targeting behavior below a health threshold.
- Enemies that spawn other enemies while moving.
- Turrets with one or multiple weapon modules.
- Flamethrowers, bullets, missiles, beams, auras, traps, and other weapon types.
- Neutral or allied characters that reuse movement, targeting, health, and combat systems.
- Flying, ground, or burrowing units, if added later.
- Destructible or generated terrain that can affect movement.

## Performance Philosophy

Performance is a core priority. The game should favor DOTS/ECS-friendly data-oriented design:

- Small `IComponentData` structs.
- Systems operating over explicit component queries.
- Burst/job-compatible logic where possible.
- Minimal per-frame virtual dispatch.
- Minimal inheritance-based runtime behavior.
- Avoid GameObject-heavy logic for high-count gameplay entities.
- Avoid large universal systems with many type branches when separate component queries would be cleaner and faster.

Readability is useful but secondary to performance and architectural correctness. The code may become technically dense if that allows better runtime behavior, better scaling, and stronger modularity.

## Development Philosophy

Start with simple implementations behind stable abstractions. It is acceptable to begin with inefficient placeholder algorithms if the data contracts allow later replacement.

Examples:

- Start with direct movement, but route movement through destination/path components so real pathfinding can replace it later.
- Start with brute-force closest-target search, but preserve a targeting interface that can later use spatial partitioning.
- Start with simple projectile movement, but do not force all attacks to be projectiles.

Current systems may be replaced if needed. The foundation should not be changed without strong reason, but early code is not sacred.

## Non-Goals

Do not optimize for a fixed list of enemy or turret classes. The game direction is intentionally flexible and may change.

Do not assume all attacks are bullets.

Do not assume all enemies only attack the command center.

Do not assume only two factions exist.

Do not assume all movement is ground movement.

Do not assume all obstacles are static.
