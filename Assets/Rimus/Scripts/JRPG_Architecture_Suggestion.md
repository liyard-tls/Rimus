# JRPG Architecture Suggestion

## Current Shape

The current scripts are mostly presentation and input, not gameplay domain yet:

- `Characters/CharacterView.cs` reacts to targeting and plays animations.
- `Characters/CharacterAnimator.cs` is a view utility.
- `Characters/TargetSelection/AttackSelector.cs` already contains useful reusable targeting logic.
- `Input/PlayerAttackSelectorInput.cs` is the right direction for separating input from logic.
- `Tools/GameContentSystem/GameContentDB.cs` suggests a data-driven content pipeline, which is a good fit for JRPG systems.

What is still missing is the runtime combat model:

- no health component
- no stats aggregation
- no skill definition/runtime
- no status effect system
- no turn/action resource model

## Recommended Composite Architecture

Use one `CharacterRoot` object composed from small runtime components.

Suggested runtime components on each character:

- `CharacterEntity`
  Purpose: root reference holder, faction/id/team, access point for other components.
- `HealthComponent`
  Purpose: current HP, max HP, damage/heal, death events.
- `StatsComponent`
  Purpose: computed final stats from base stats + equipment + buffs + level.
- `ResourceComponent`
  Purpose: MP/SP/ATB/etc.
- `StatusEffectController`
  Purpose: active buffs/debuffs, turn ticks, expiry.
- `SkillCaster`
  Purpose: validates skill cost/range/cooldown and executes skill requests.
- `AttackSelector`
  Purpose: target evaluation only.
- `CharacterPresenter`
  Purpose: binds runtime events to view/animation/UI.
- `CharacterView`
  Purpose: pure visuals.

This gives you:

- model/runtime logic independent from input
- enemy AI and player using the same combat components
- cleaner testing and easier balancing

Good dependency direction:

`Input/AI -> SkillCaster -> Targeting/Combat Systems -> Health/Stats/Status -> Presenter/View`

Avoid:

`View/Input directly changing HP or stats`

## Health

Use a dedicated component, not raw fields on character classes.

Suggested split:

### `HealthDefinition`

- max HP formula inputs if static per archetype
- optional death behavior flags

### `HealthComponent`

- `CurrentHp`
- `MaxHp`
- `IsDead`
- `TakeDamage(DamageContext)`
- `Heal(HealContext)`
- events:
  - `OnDamaged`
  - `OnHealed`
  - `OnDeath`
  - `OnRevived`
  - `OnHpChanged`

For JRPGs, health should usually be derived from stats, not duplicated manually:

- `MaxHp = Stats.MaxHp`
- `CurrentHp` is runtime only

Important rule:

- `HealthComponent` owns current HP
- `StatsComponent` owns max HP value
- when stats change, health clamps to new max

## Stats

Do not store only one flat struct. Use layered composition.

Recommended stat pipeline:

1. `BaseStats`
2. `LevelGrowth`
3. `EquipmentModifiers`
4. `PassiveModifiers`
5. `StatusEffectModifiers`
6. final resolved `ComputedStats`

Suggested JRPG stat set:

- `MaxHp`
- `MaxMp`
- `Attack`
- `MagicAttack`
- `Defense`
- `MagicDefense`
- `Speed`
- `Accuracy`
- `Evasion`
- `CritChance`
- `CritDamage`
- `AggroModifier`
- optional elemental modifiers:
  - `FireResist`
  - `IceResist`
  - etc.

Use two layers of data:

### `CharacterStatsData`

- base values
- growth curves or per-level values

### `CharacterStatsRuntime`

- computed values after modifiers

Suggested modifier model:

- flat add
- percent add
- percent multiply

Order:

1. base
2. flat adds
3. additive percents
4. multiplicative percents

That keeps balance predictable.

## Data Model

Your `GameContentSystem` is useful, but currently too generic to drive combat alone.

Add content types like:

- `CharacterDefinition : ScriptableObject, IGameContent`
- `SkillDefinition : ScriptableObject, IGameContent`
- `StatusEffectDefinition : ScriptableObject, IGameContent`
- `EquipmentDefinition : ScriptableObject, IGameContent`

### `CharacterDefinition`

- id
- display name
- faction
- prefab/view reference
- base stats
- growth profile
- default skills
- resistances
- optional AI profile id

### `SkillDefinition`

- id
- selector type
- target rules
- range/radius/angle
- cost
- cooldown
- effect list

### `StatusEffectDefinition`

- id
- duration model
- stack rules
- stat modifiers
- periodic effects
- tags

This fits `Tools/GameContentSystem/GameContentDB.cs` well.

## Targeting and Skills

The current targeting system is already the seed of a good combat architecture.

Make `AttackSelector` feed into a `SkillTargetingProfile` instead of hardcoding selector behavior per skill.

Example targeting model:

- `TargetingShapeType`
  - single
  - circle
  - cone
  - line
  - self
  - party
- `TargetFilter`
  - enemies
  - allies
  - self
  - dead allies
- `MaxTargets`
- `SortMode`
  - nearest to caster
  - nearest to cursor
  - lowest hp
  - all

Then `SkillDefinition` references one targeting profile.

Examples:

- fireball = circle + enemies + all
- sword slash = cone + enemies + max 3
- heal = single + allies + max 1
- revive = single + dead allies

## Suggested Folder Architecture

Suggested evolution for `Assets/Rimus/Scripts`:

- `Characters/Core`
  - `CharacterEntity`
  - `Faction`
  - `CharacterRuntimeState`
- `Characters/Stats`
  - `StatsComponent`
  - `StatType`
  - `StatModifier`
  - `ComputedStats`
- `Characters/Health`
  - `HealthComponent`
  - `DamageContext`
  - `HealContext`
- `Characters/Resources`
  - `ResourceComponent`
- `Characters/StatusEffects`
  - `StatusEffectController`
  - `StatusEffectRuntime`
- `Characters/Skills`
  - `SkillCaster`
  - `SkillExecutor`
  - `SkillRequest`
- `Characters/TargetSelection`
  - keep the existing selector system here
- `Characters/View`
  - move `CharacterView`, `CharacterAnimator`, `CharacterPresenter`
- `Content/Definitions`
  - `CharacterDefinition`
  - `SkillDefinition`
  - `StatusEffectDefinition`
  - `EquipmentDefinition`
- `Input`
  - `PlayerAttackSelectorInput`
  - player command scripts
- `Combat`
  - turn order / battle state / damage formulas

## Concrete Runtime Flow

For a typical JRPG action:

1. player chooses skill
2. `SkillDefinition` sets selector type/profile
3. `PlayerAttackSelectorInput` drives `Characters/TargetSelection/AttackSelector.cs`
4. player confirms
5. `SkillCaster` builds `SkillRequest`
6. `SkillExecutor` validates targets, costs, cooldowns
7. effects apply to `HealthComponent`, `StatsComponent`, `StatusEffectController`
8. `CharacterPresenter` listens and updates animation/UI

Enemy AI uses the same flow, except:

- no mouse input
- AI provides target position/target list directly

## Important Design Rule

For JRPG scale, keep this separation:

- `Definition` = static authoring data
- `Runtime Component` = mutable battle state
- `Presenter/View` = visuals only

That avoids giant MonoBehaviour god objects.

## What To Build Next

Recommended order:

1. `CharacterDefinition`
2. `StatsComponent`
3. `HealthComponent`
4. `SkillDefinition`
5. `SkillCaster`
6. `StatusEffectController`
7. `CharacterPresenter` event wiring

## Suggested First Slice

If implementing the next step, start with:

- `CharacterDefinition`
- `StatsComponent`
- `HealthComponent`
- event flow from health to `CharacterPresenter` / `CharacterView`
