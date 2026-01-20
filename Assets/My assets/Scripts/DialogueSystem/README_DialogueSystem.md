# Dialogue System (Monologue)

This folder contains a small, self-contained monologue dialogue system.

## Setup (once)

1. Create a Dialogue UI in your Canvas with:
   - a root `DialogueBox` GameObject
   - a `DialogNameBox` that contains `DialogName`
   - a `DialogText`
   - a `DialogButton` (continue)

2. Add `DialogueUI` to the root object and assign references.

3. Create an empty GameObject in the scene named `DialogueManager` and add:
   - `DialogueManager`

4. (Recommended) Add `DialogueInputBlockerAdapter` somewhere (often on the Player) and list the behaviours to disable during dialogue.
   Then assign that component to `DialogueManager > Input Blocker Behaviour`.

## Creating dialogue content

- Create a `DialogueSequence` asset (Right click in Project):
  `Create > Hubble > Dialogue > Monologue Sequence`

- Fill `speakerName` (optional) and add lines.
- Each line can set:
  - text
  - delayAfter (seconds)
  - charactersPerSecond override

## Triggering

- Add `DialogueTrigger` to any GameObject with a trigger collider.
- Assign the `DialogueSequence`.
- Set `playerTag` to match your player.

## Controls

- Continue button becomes clickable only after typing finishes.
- The dialogue also supports keyboard/controller via an `InputActionReference` on `DialogueManager`.

### Important: don't reuse Gameplay/Jump

Avoid using your **Gameplay/Jump** action as dialogue continue. It can cause side effects (jump buffering / jump getting blocked / actions being enabled-disable by multiple systems).

Create and use a dedicated input action for dialogue.

### Recommended (Space + Gamepad A)

This project includes a ready-to-use Input Actions asset:

- `Assets/My assets/Input/Dialogue Input Actions.inputactions`
   - Map: `Dialogue`
   - Action: `Continue`
   - Bindings: `Keyboard/Space` + `Gamepad/buttonSouth`

Assign it like this:

1. Select `Dialogue Input Actions.inputactions` in the Project window.
2. Expand it, select the `Continue` action.
3. Drag the `Continue` action into `DialogueManager > Continue Action`.

If `Allow Skip Typewriter` is enabled, pressing continue during typing reveals instantly.

### Troubleshooting (skip/continue doesn't work)

- Make sure `DialogueManager > Continue Action` references the **action** `Dialogue/Continue` (not just the asset file).
- In `DialogueManager`, keep **Manage Continue Action Enable State** enabled (default): it will enable the action during dialogue and restore the previous state after.
- (Optional) Enable **Debug Continue** in `DialogueManager` to see `Continue performed` logs when you press Space/A.

## TextMeshPro

This system targets TextMeshPro (`TMP_Text`) for `DialogName` and `DialogText`.
