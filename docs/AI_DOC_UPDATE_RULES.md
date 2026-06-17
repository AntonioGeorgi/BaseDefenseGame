# AI Doc Update Rules

This file tells AI assistants when and how to update the project documentation.

## Purpose

The docs folder should help future AI chats understand the project quickly without reading the whole repository.

Docs should be:

* Short.
* Current.
* Non-repetitive.
* Focused on architecture, responsibilities, and design intent.
* Useful for future code changes.

Do not turn the docs into copied code or a full manual.

## Read Order for AI Assistants

When starting a new chat about this project, read docs in this order:

1. `AI_CONTEXT.md`
2. `PROJECT_VISION.md`
3. `CODE_OVERVIEW.md`
4. `ARCHITECTURE_PRINCIPLES.md`
5. `GAMEPLAY_SYSTEMS.md`
6. `AI_DOC_UPDATE_RULES.md`

## When to Update Docs

Update docs when a code change affects future development.

Update docs if:

* A new gameplay system, manager, component, authoring object, or data flow is added.
* The responsibility of an existing system/class/component changes.
* A major concept is renamed.
* A new architectural rule or pattern is introduced.
* A system is removed, replaced, or made obsolete.
* A temporary implementation is added that future AI assistants must not mistake for the final architecture.
* A known limitation becomes important for future work.
* The current code state described in `CODE_OVERVIEW.md` changes.

Do not update docs if:

* The change is only formatting.
* The change is a tiny bug fix with no architectural effect.
* The change only adjusts balance numbers.
* The information would duplicate another doc.
* The detail is obvious from a class or component name and does not affect future decisions.

## Which File to Update

### `AI_CONTEXT.md`

Update this only when the top-level AI guidance changes.

Use it for:

* Project summary.
* Required read order.
* Most important rules.
* Hard constraints.
* Warnings about common wrong approaches.

Do not put detailed system descriptions here.

### `PROJECT_VISION.md`

Update this when the intended game direction changes.

Use it for:

* Core fantasy.
* Player experience.
* Long-term gameplay goals.
* Non-goals.
* Design pillars.

Do not put implementation details here unless they strongly affect the vision.

### `CODE_OVERVIEW.md`

Update this whenever the current code structure changes in a way future AI assistants should know.

Use it for:

* Current folders/files.
* Existing systems.
* Existing components.
* Current data flow.
* What is implemented now versus only planned.
* Where new code should likely be added.

This file should describe the current code, not the ideal future architecture.

### `ARCHITECTURE_PRINCIPLES.md`

Update this when a stable technical rule changes.

Use it for:

* ECS/DOTS design principles.
* Composition over inheritance.
* Event/request patterns.
* Performance rules.
* Runtime data ownership rules.

Do not put project vision or temporary implementation notes here.

### `GAMEPLAY_SYSTEMS.md`

Update this when the intended gameplay system model changes.

Use it for:

* Targeting model.
* Movement/pathing model.
* Combat model.
* Damage model.
* Spawning model.
* Terrain/obstacle model.
* Ability/trigger model.

This file may describe planned systems, but it must clearly separate planned ideas from implemented code.

## Writing Rules

Prefer summaries over code.

Good:

```text
Damage should flow through DamageEvent entities so weapon systems do not directly modify Health.
```

Bad:

```csharp
public struct DamageEvent : IComponentData
{
    public Entity Target;
    public float Amount;
}
```

Small code examples are allowed only when they clarify a naming convention or data shape.

## Keep Docs Non-Repetitive

Before adding information, check whether another doc already says it.

If the same idea appears in multiple files:

* Keep the shortest version in `AI_CONTEXT.md`.
* Keep the detailed technical version in the most specific file.
* Remove or shorten the duplicate.

## Mark Current vs Planned

Always distinguish current code from intended future architecture.

Use clear wording:

```text
Currently implemented:
```

```text
Planned / intended:
```

```text
Temporary implementation:
```

Do not describe a planned system as if it already exists.

## Handling Uncertainty

If the AI is unsure whether something is true in the code:

* Do not guess.
* Inspect the relevant file if permitted.
* If not permitted, write a short note such as:

```text
Needs verification: exact implementation not checked.
```

## Updating After Code Changes

After making code changes, AI assistants should check:

1. Did the change alter current code structure?

   * Update `CODE_OVERVIEW.md`.

2. Did the change alter architectural rules?

   * Update `ARCHITECTURE_PRINCIPLES.md`.

3. Did the change alter intended gameplay systems?

   * Update `GAMEPLAY_SYSTEMS.md`.

4. Did the change alter the game vision?

   * Update `PROJECT_VISION.md`.

5. Did the change affect how future AI chats should approach the project?

   * Update `AI_CONTEXT.md`.

## Cleanup Rule

When updating docs, remove outdated statements.

Do not only append new notes forever.

A shorter accurate doc is better than a long stale doc.
