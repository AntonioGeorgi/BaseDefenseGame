# BaseDefenseGame

An early Unity DOTS/ECS rewrite of a 3D base-defense game with gameplay movement on the X/Z ground plane.

## Project tracking

GitHub issues are the source of truth for the current project state, plans, acceptance criteria, and architecture references:

- [All issues](https://github.com/AntonioGeorgi/BaseDefenseGame/issues)
- [Issue #8: umbrella rewrite and ECSGalaxySample reference](https://github.com/AntonioGeorgi/BaseDefenseGame/issues/8)
- [Issue #10: deterministic bootstrap](https://github.com/AntonioGeorgi/BaseDefenseGame/issues/10)
- [Issue #11: pseudo-2D ground plane](https://github.com/AntonioGeorgi/BaseDefenseGame/issues/11)
- [Issue #12: custom ECS movement](https://github.com/AntonioGeorgi/BaseDefenseGame/issues/12)

Do not maintain plans or in-progress status in repository documentation. When an issue is finished and verified, record its final outcome in [`docs/COMPLETED_ISSUES.md`](docs/COMPLETED_ISSUES.md).

## Repository boundaries

- `Assets/Scripts/` contains the rewrite.
- `Old/` contains legacy code and should not be extended accidentally.
- `Assets/Scenes/SampleScene.unity` is the current main scene.
- Unity version: `6000.4.0f1`.

The current architecture reference is Unity Technologies' `ECSGalaxySample`, as specified in issue #8.
