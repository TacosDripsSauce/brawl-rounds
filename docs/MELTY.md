# Melty release draft

## Proposed listing

**Title:** Brawl ROUNDS

**Tagline:** Turn ROUNDS into a 1v1 platform fighter with weapon pickups, rising knockback and ring-outs.

**Player description:**
Spawn unarmed, grab one of four weapons, build your opponent's damage percentage and launch them out of the arena. ROUNDS' between-round card draft stays intact, but upgrades focus on mobility, recovery, knockback and weapon handling.

## Game requirement

- Required host game: ROUNDS.
- Brawlhalla: inspiration only in v0.1; not listed as a required game unless the build later genuinely reads/plays from the player's Brawlhalla copy.

## Multiplayer

- Mode: multiplayer.
- Actual first-release capacity: 2 players (1v1).
- Session transport: ROUNDS' native lobby/session; this mod does not invent or bundle a server.
- `recipe.multiplayer.maxPlayers`: intended value `2`.
- `recipe.multiplayer.connect`: **leave unset until Melty can prove an automatic native-lobby join route.** Do not guess launch arguments, log addresses or a relay.

## Recipe policy

Do not commit a speculative `melty.json`. Before release, Melty must run all of the following on the finished archive:

1. package inspection;
2. recipe validation against every archive entry;
3. one-click check;
4. multiplayer host/join verification;
5. Melty Test on the user's PC.

If any dependency still needs manual installation, the release remains a draft.

## Metadata still requiring creator choice

- content/source license;
- whether others may remix;
- final credit wording.

Those are permissions and will not be guessed.
