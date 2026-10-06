# Brawl ROUNDS

> Development source for a ROUNDS 1v1 platform-fighter mashup. This repository is **source-only until the Windows/ROUNDS verification checklist is completed**.

Brawl ROUNDS turns a normal ROUNDS duel into a platform-fighter ruleset: both players spawn unarmed, race for weapon pickups, build a visible damage percentage, launch farther as that percentage rises, and lose a point by flying outside the arena. Between rounds, the normal ROUNDS card draft stays in place, with cards aimed at movement, recovery, knockback and weapon handling.

## First playable scope

- 1v1 using ROUNDS' native lobby/session.
- Spawn unarmed at the start of each point.
- Four pickup weapons: Sword, Spear, Pistol and Shotgun.
- Percentage damage instead of ordinary lethal HP damage.
- Knockback scales with accumulated percentage.
- Ring-out when a player leaves the camera bounds.
- Double-tap ground/air dash and one air dash before touching ground again.
- Eight generated draft cards that modify movement, recovery, knockback, attack speed or melee reach.
- Minimal in-game HUD for percentage and equipped weapon.

## Required game

**ROUNDS** is the host and the only game required by this first release. **Brawlhalla is design inspiration, not a fake install dependency and no Brawlhalla files are redistributed.** If a future version genuinely reads player-owned Brawlhalla data at runtime, that requirement will be added only after it exists and is tested.

## Modding route

The mod is a C# BepInEx plugin built against ROUNDS and UnboundLib. It does not overwrite the game's own files. The project intentionally ships no ROUNDS assemblies, extracted assets or decompiled source.

## Sheets are the source of truth

Gameplay content and hooks live in `sheets/*.json`. Run:

```text
python tools/preflight.py
python tools/generate.py
```

The preflight rejects missing/unchecked fields and unresolved cross-sheet references. Each sheet row generates one C# row artifact under `src/BrawlRounds/Generated/`.

## Build

The build must run on a machine that has ROUNDS plus the loader/runtime libraries. See `docs/BUILD.md`. The repository does not claim a successful binary build until `docs/TEST.md` is completed against the real game.

## Melty status

The intended distribution is one-click through Melty, but `melty.json` is deliberately **not fabricated**. It will be committed only after Melty's package inspection, recipe validation and one-click check are available and pass. See `docs/MELTY.md`.
