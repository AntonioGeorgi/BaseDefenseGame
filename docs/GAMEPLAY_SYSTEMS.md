# Gameplay Systems

This file describes the intended gameplay systems for BaseDefenseGame.

It includes both implemented and planned systems, but must clearly mark which is which.

## Core Gameplay Loop

Currently / intended:

1. Player defends a central base.
2. Enemies spawn in waves or groups.
3. Enemies move through open terrain toward targets.
4. Player places towers, obstacles, or structures.
5. Towers and other combat systems attack valid targets.
6. Damage, death, rewards, and wave progression happen through reusable systems.

## Factions and Targeting

### Intended

Entities should not assume only two sides.

Use factions or relationship data instead of hardcoding:

- Player
- Enemy
- Neutral
- Summoned allied units
- Environmental hazards
- Boss-owned units
- Temporary converted units

Targeting should support:

- Closest target
- Highest priority target
- Lowest health target
- Strongest target
- Target by faction
- Target by tag/category
- Target by range
- Target by line of sight if needed later

Avoid assuming:

- Towers only target enemies.
- Enemies only target the base.
- Movement target and attack target are always the same.

## Movement and Pathing

### Intended

Movement should support open-field enemy navigation.

The game may eventually include:

- Obstacles
- Dynamic blockers
- Destructible blockers
- Generated terrain
- Flying units
- Units that ignore certain terrain
- Units with alternate pathing behavior

Avoid building movement around one fixed path or lane system unless explicitly chosen later.

## Spawning and Waves

### Intended

Spawning should be data-driven.

Waves may contain:

- Horde enemies
- Elite squads
- Bosses
- Mixed enemy groups
- Summoned entities
- Delayed spawns
- Directional or positional spawn rules

Spawning systems should create entities from spawn requests or wave data instead of hardcoding enemy creation inside unrelated systems.

## Combat

### Intended

Combat should be modular.

An entity may have:

- One weapon
- Multiple weapons
- Passive damage
- Area effects
- Projectiles
- Hitscan attacks
- Melee attacks
- Damage-over-time effects
- Aura effects
- Triggered abilities

Avoid assuming every attack is a bullet.

## Damage and Health

### Intended

Damage should flow through reusable damage events or requests.

Damage should support future modifiers such as:

- Armor
- Resistances
- Shields
- Damage types
- Critical hits
- Status effects
- Friendly fire rules
- Environmental damage

Avoid direct coupling where one weapon system directly owns all health/death logic.

## Structures and Placement

### Intended

Player-built structures may include:

- Towers
- Walls
- Traps
- Resource buildings
- Utility buildings
- Temporary blockers
- Support structures

Placement should eventually consider:

- Valid terrain
- Collision
- Build cost
- Build radius
- Grid or free placement
- Rotation
- Blocking/pathing impact

Avoid assuming every placed object is a tower.

## Terrain and Obstacles

### Intended

Terrain may become part of gameplay.

Possible future features:

- Procedural terrain
- Destructible terrain
- Buildable obstacles
- Enemy-created obstacles
- Terrain modifiers
- Slowing areas
- Blocked areas
- High ground or range modifiers

Systems should avoid assuming the map is permanently static.

## Abilities and Effects

### Intended

The game may support reusable gameplay effects such as:

- Slow
- Burn
- Poison
- Stun
- Knockback
- Buffs
- Debuffs
- Healing
- Shielding
- Summoning
- Conversion
- Area denial

Effects should be component/data-driven where possible.

## Resources and Progression

### Intended

Future progression may include:

- Currency from kills
- Wave rewards
- Upgrade choices
- Tower upgrades
- Unlocks
- Base upgrades
- Research
- Temporary run-based modifiers

Do not overbuild this early, but leave room for it.

## Current vs Planned Rule

Each section should clearly say one of:

- Currently implemented
- Partially implemented
- Planned
- Not implemented yet