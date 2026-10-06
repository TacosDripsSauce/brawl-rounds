# Build notes

## Known route

ROUNDS is a Unity/Mono game with an established BepInEx + UnboundLib modding route. This project targets .NET Framework 4.6.1 to match the current UnboundLib project configuration and compiles against DLLs from the player's ROUNDS installation rather than committing them.

A late-2025 ROUNDS update disrupted many existing mods. Current compatibility therefore has to be verified against the player's actual game build. DuctTape is a promising current compatibility layer for older ROUNDS mods, but it is **not bundled or declared required here until its license, exact files and Melty one-click behavior are validated**.

## Local prerequisites for a real build

- ROUNDS installed.
- BepInEx for ROUNDS installed by the chosen one-click recipe/profile.
- UnboundLib present and compatible with the current ROUNDS build.
- .NET SDK capable of building `net461` projects.

`tools/build-windows.ps1` locates the Steam installation, searches the installed BepInEx tree for `UnboundLib.dll`, then runs sheet preflight + generation before compiling.

No game DLL or third-party dependency DLL is copied into this repository.
