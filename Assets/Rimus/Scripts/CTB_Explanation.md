# CTB Explanation

## What CTB Means

CTB usually means `Conditional Turn-Based`.

It is a turn system where units do not simply act once per round in a fixed order.
Instead, every unit progresses on a hidden or visible timeline, and faster units reach their turns more often.

This is different from:

- classic round-based combat:
  everyone acts once per round
- ATB:
  time flows continuously in real time

CTB is usually easier to reason about than ATB and more flexible than strict rounds.

## Core Idea

Each unit has:

- a `Speed` stat
- a current `initiative` value
- a threshold required to act

In this project:

- `TurnManager` tracks initiative for each `CharacterEntity`
- `ReadyThreshold` defaults to `100`
- units gain initiative based on `Speed`

When a unit's initiative reaches or exceeds the threshold, that unit can take a turn.

## Basic Formula

Simplified model:

`initiative += speed * time`

When:

`initiative >= ReadyThreshold`

the unit becomes ready to act.

After the action, initiative is reduced by the action cost:

`initiative -= actionCost`

So:

- high speed means turns come more often
- expensive actions delay the next turn longer
- fast actions let the unit act again sooner

## How `TurnManager` Works Here

File:

- `Assets/Rimus/Scripts/Managers/TurnManager.cs`

The manager stores:

- `_characters`
- `_initiative`
- `_turnOrder`
- `CurrentActor`
- `ReadyThreshold`

Important methods:

### `Initialize(...)`

Registers combatants and resets initiative state.

### `Tick(deltaTime)`

Advances initiative using elapsed time and promotes a ready actor if one reaches the threshold.

Use this if you want time-based filling.

### `AdvanceToNextTurn()`

Advances initiative directly to the next ready unit.

This is useful when you do not want to simulate every frame of initiative growth and instead want to jump directly to the next turn.

### `CompleteTurn(actionCost)`

Spends initiative from the current actor after the action resolves.

Example:

- normal attack cost = `100`
- heavy spell cost = `130`
- quick skill cost = `70`

### `GetTurnPreview(count)`

Builds a predicted upcoming order using the current initiative state.

This is useful for a future battle timeline UI.

## How Battle Flow Works

File:

- `Assets/Rimus/Scripts/Managers/BattleManager.cs`

The minimal flow is:

1. battle starts
2. `BattleManager` initializes `TurnManager`
3. `TurnManager.AdvanceToNextTurn()` finds the next ready unit
4. that unit becomes `ActiveActor`
5. player input or AI chooses a skill
6. `SkillCaster` resolves the skill
7. `BattleManager` receives `OnSkillCast`
8. `TurnManager.CompleteTurn(skill.ActionCost)` spends initiative
9. next unit is prepared

## Why Speed Matters

Suppose:

- warrior speed = `10`
- rogue speed = `20`
- threshold = `100`

Then:

- warrior reaches a turn after `10` time units
- rogue reaches a turn after `5` time units

The rogue acts about twice as often.

If the rogue uses a heavy action with cost `140`, the benefit is partly reduced because the next turn comes later.

That is one of the main strengths of CTB:

- turn frequency
- action weight
- buffs and debuffs

all interact cleanly.

## Why Action Cost Matters

Without action cost, a fast unit is simply always better.

With action cost:

- cheap actions are fast but weaker
- expensive actions are slower but stronger

This creates meaningful tactical choices.

Examples:

- `Attack`: cost `100`
- `Fireball`: cost `120`
- `Quick Stab`: cost `70`
- `Mega Heal`: cost `150`

So turn order is affected not only by stats, but also by the action selected.

## AI and Player Turns

In the current setup:

- player-controlled units are marked on `CharacterEntity`
- `BattleManager` enables `PlayerAttackSelectorInput` only during that actor's turn
- enemy units use simple AI

Current AI logic:

- damage skills target the nearest opponent
- heal skills target the lowest-health ally

This is enough for a basic playable loop.

## Example Timeline

Assume:

- threshold = `100`
- knight initiative = `95`, speed = `10`
- mage initiative = `60`, speed = `20`

Next advance:

- knight needs `5 / 10 = 0.5`
- mage needs `40 / 20 = 2`

Knight acts first.

If knight uses a skill with cost `100`:

- knight initiative becomes `0`

Then mage may become next depending on current initiative values.

If the knight used a very cheap action with cost `50`:

- knight initiative becomes `45`

so the knight will be back much sooner.

## Benefits of CTB

- more interesting than fixed rounds
- easier to debug than ATB
- speed matters naturally
- action weight is easy to express
- turn preview UI is straightforward
- haste / slow / delay / stun mechanics fit well

## Typical Extensions

Good next additions for CTB:

- visible turn timeline UI
- haste and slow initiative modifiers
- delay effects that reduce initiative
- stun / skip-turn flags
- cast times
- counter-attacks or interrupts
- per-skill action costs and cooldowns

## Mental Model

The simplest way to think about CTB:

- every unit is always moving toward its next turn
- speed determines how fast it moves
- chosen action determines how far it gets pushed back afterward

That is the whole system.
