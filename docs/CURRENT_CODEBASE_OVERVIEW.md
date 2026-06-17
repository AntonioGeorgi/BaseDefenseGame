# Current Codebase Map

Purpose: compact overview of current implementation. Use this before reading source files.

## Runtime Model

The game is a Unity DOTS/ECS prototype. Authoring MonoBehaviours bake prefab data into ECS components. Runtime behavior is handled by ECS systems under `Assets/Scripts/Systems`.

Current gameplay loop:
Spawner → Enemy movement → Enemy melee damage → Turret targeting → Turret rotation → Turret firing → Projectile movement/damage → Health/death/pooling.

## Main Gameplay Entities

### Command Building
Represents the base objective.
Current role:
- has health
- is targeted by enemy movement
- receives melee damage
- triggers simple game-over log/destruction when dead

Relevant files:
- `CommandBuildingAuthoring.cs`
- `HealthSystem.cs`
- `EnemyMeleeDamageSystem.cs`
- `SpawnerSystem.cs`
- `EnemyInitSystem.cs`

### Enemy
Current role:
- spawned by spawners
- moves directly toward the command building
- deals melee DPS when in range
- has temporary lifetime expiry
- gets pooled instead of destroyed when dead

Relevant files:
- `EnemyAuthoring.cs`
- `EnemyDataSO.cs`
- `SpawnerSystem.cs`
- `EnemyInitSystem.cs`
- `EnemyMovementSystem.cs`
- `EnemyMeleeDamageSystem.cs`
- `LifetimeSystem.cs`
- `HealthSystem.cs`

### Turret
Current role:
- scans for nearest enemy in range
- rotates mount toward target
- fires projectile prefab from fire point
- currently has one weapon config on the base entity

Relevant files:
- `TurretBaseAuthoring.cs`
- `TurretMountAuthoring.cs`
- `TurretBarrelAuthoring.cs`
- `FirePointAuthoring.cs`
- `TargetingSystem.cs`
- `TurretRotationSystem.cs`
- `TurretFireSystem.cs`

### Projectile
Current role:
- spawned by turrets
- moves forward
- checks distance overlap against enemies
- directly subtracts enemy health
- destroys itself on hit or max range

Relevant files:
- `ProjectileAuthoring.cs`
- `TurretFireSystem.cs`
- `ProjectileMovementSystem.cs`

## Important Current Data Groups

Identity tags:
- enemy
- command building
- turret base/mount/barrel
- projectile
- pooled/pending init

Shared gameplay data:
- health
- movement speed
- move target
- melee damage
- lifetime

Turret data:
- current target
- weapon config
- turret part links
- fire point axis

Projectile data:
- direction
- speed
- damage
- max range
- distance traveled
- hit radius

Pooling data:
- enemy pool singleton
- pooled tag
- pending init tag

## Current System Responsibilities

`EnemyPoolBootstrapSystem`
Creates enemy pool singleton. No prewarming yet.

`SpawnerSystem`
Periodically spawns batches. Reuses pooled enemies if available, otherwise instantiates. Sets destination to command building position.

`EnemyInitSystem`
Resets pooled enemies after reactivation.

`EnemyMovementSystem`
Moves enemies directly toward their move target using physics velocity. Stops within melee range.

`EnemyMeleeDamageSystem`
Applies direct damage to command building when enemies are in melee range.

`LifetimeSystem`
Temporary test system. Kills enemies after lifetime expires.

`HealthSystem`
Handles death. Pools dead enemies, destroys dead turrets, logs/destroys command building on death.

`TargetingSystem`
Each turret scans all enemies and chooses nearest enemy in range.

`TurretRotationSystem`
Rotates turret mount horizontally toward target.

`TurretFireSystem`
Spawns projectile when cooldown allows.

`ProjectileMovementSystem`
Moves projectiles, checks enemy overlap, applies direct damage, destroys projectile.

## Current Data Flow

```text
SpawnerSystem
  creates/reuses Enemy

EnemyInitSystem
  resets pooled Enemy

EnemyMovementSystem
  moves Enemy toward CommandBuilding

EnemyMeleeDamageSystem
  Enemy damages CommandBuilding directly

TargetingSystem
  TurretBase selects Enemy target

TurretRotationSystem
  TurretMount rotates toward target

TurretFireSystem
  TurretBase spawns Projectile

ProjectileMovementSystem
  Projectile damages Enemy directly

HealthSystem
  dead Enemy → pool
  dead Turret → destroy
  dead CommandBuilding → game over log + destroy