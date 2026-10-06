# MODLOG — Brawl ROUNDS

## 2026-10-06 — intake and route

- Goal: ROUNDS host, 1v1 platform-fighter mashup inspired by Brawlhalla.
- First minute: spawn unarmed -> pickup -> percentage damage -> increasing knockback -> ring-out.
- Weapons: sword, spear, pistol, shotgun.
- Cards: retain ROUNDS draft, retarget upgrades to mobility/recovery/knockback/weapon handling.
- Multiplayer: native ROUNDS 1v1 only; no invented server or player count.
- Required game for v0.1: ROUNDS. Brawlhalla is inspiration only unless future code genuinely consumes the player's Brawlhalla install.
- Chosen route: Unity/Mono loader API + managed Harmony patches, using BepInEx + UnboundLib. Reason: established ROUNDS modding route; avoids overwriting game files.
- universal-modder mod-any-game rules applied: source/data recon first, no game files in repo, vertical slice before broadening, real game is final oracle, no fake screenshot or test claim.

## Recon

- UnboundLib exposes NetworkingManager, CustomCard and GameMode hook APIs needed by the design.
- Project targets the ROUNDS install assemblies locally; no proprietary assemblies are committed.
- Current ecosystem compatibility needs real-game verification because the late-2025 ROUNDS update broke many older mods.
- DuctTape is being tracked as a possible compatibility route for the current ROUNDS build; it is not bundled or assumed publishable yet.

## Sheets and source

- `weapons.json`: 4 rows.
- `cards.json`: 8 rows.
- `systems.json`: 8 rows.
- `hooks.json`: 4 rows.
- Preflight result at source milestone: 24 rows checked, no missing required cell/reference.
- Code generation result: one generated C# artifact per row.

## Runtime vertical slice implemented in source

- percentage state per player;
- damage interception -> percentage + launch force;
- camera-bound ring-out;
- unarmed round reset;
- synchronized weapon pickup events;
- melee sword/spear hit selection;
- ranged pistol/shotgun configuration;
- one-air-dash movement extension;
- generated custom cards;
- minimal percentage/weapon HUD.

## Current hard blockers before calling it playable/publishable

1. Compile against the user's real/current ROUNDS + installed loader libraries.
2. Fix any API drift revealed by that build.
3. Launch and complete the real-game two-client checklist.
4. Capture gameplay media from that exact build.
5. Run Melty package/recipe/one-click checks with private Melty tooling.
6. Creator chooses source/content license and remix permission.
7. Melty Test and explicit creator authorization before publish.
