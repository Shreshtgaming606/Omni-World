# Omni World

**Omni World** is an original 2D side-scrolling platformer prototype for Windows 11 starring **Core-Man**.

This first version is built with C# WinForms/GDI+ because the target machine already includes the Windows .NET Framework runtime and compiler, so the prototype can build without downloading SDL2, SFML, Raylib, or a C++ toolchain. The code is organized into small engine-style classes so the gameplay can later be ported to C++/Raylib or another 2D framework.

## Features

- 5 complete playable courses:
  - Course 1: **Greenlit Grove**, a beginner grassy course
  - Course 2: **Voltage Vale**, a harder obstacle course
  - Course 3: **Crystal Canopy**, a twilight crystal course with longer gaps and moving-platform timing
  - Course 4: **Emberworks**, an industrial heat-vent course with pressure enemies
  - Course 5: **Skyline Circuit**, a fast rooftop course built around bounce pads and chained bursts
- Smooth side-scrolling camera
- Walking, running, variable-height jumping, gravity, and platform physics
- **Core Burst** dash with cooldown, spark trail, enemy impact, and breakable-block shatter behavior
- **Burst Cells** that instantly recharge Core Burst when collected
- **Bounce Pads** that launch Core-Man upward and recharge Core Burst for aerial routes
- Floors, walls, ceilings, breakable blocks, spikes, pits, moving platforms, checkpoints, and goal gates
- Original enemies: Rollers, Hoppers, and Seekers
- Stomp-to-defeat enemy behavior
- Energy orb collectibles, score, health, lives, and checkpoints
- Original power-up: **Flux Core**, which boosts speed and lets Core-Man smash enemies and breakable blocks on contact
- Stomp combo bonus scoring, landing dust, pickup sparks, impact particles, and small camera shake feedback
- Smoother camera look-ahead that shows more of the course in the direction Core-Man is moving
- Decorated terrain surfaces, foreground grass/tech effects, checkpoint bases, and subtle atmosphere overlays
- Course geometry tuned so progression platforms and checkpoint respawns stay reachable
- Original upgraded in-code art for energy coins, core blocks, checkpoint flags, enemies, and the end beacon
- Course-specific scenery for grassy, tech-valley, crystal-canopy, industrial, and skyline themes
- Course-selection screen, pause menu, game over screen, course-clear screen, and victory screen
- Basic generated sound effects and looping background music support
- Original placeholder pixel-art-style visuals drawn in code

## Controls

- Move: `A/D` or `Left/Right`
- Run: `Shift`
- Jump: `Space`, `Z`, or `Up`
- Core Burst dash: `X`, `C`, or `Ctrl`
- Pause: `Esc` or `P`
- Select/menu confirm: `Enter`
- Menu navigation: `Up/Down` or `W/S`
- Restart current course from pause: `R`
- Return to course select from pause: `M`

## Build

Open PowerShell in this folder and run:

```powershell
.\build.ps1
```

If PowerShell script execution is disabled on your system, use:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\build.ps1
```

The executable is created at:

```text
bin\OmniWorld.exe
```

## Publisher and SmartScreen

The executable metadata uses **OmniCore tech** as the company/publisher name in Windows file properties.

Windows SmartScreen still requires a trusted Authenticode code-signing certificate to show a verified publisher and avoid reputation warnings. If you obtain a code-signing certificate, place the final EXE in `release\OmniWorld.exe` and run:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\sign-release.ps1 -CertificateThumbprint YOUR_CERT_THUMBPRINT
```

## Run

```powershell
.\run.ps1
```

or, if script execution is disabled:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\run.ps1
```

or run the built executable directly:

```powershell
.\bin\OmniWorld.exe
```

## Project Layout

```text
src/
  AudioManager.cs    Generated sound effects and background loop support
  Enemy.cs           Enemy movement and stomp logic data
  Entity.cs          Shared entity, collectible, platform, checkpoint, and goal classes
  Game.cs            Game state machine and gameplay orchestration
  GameForm.cs        Windows window, fixed-step-ish loop, and keyboard events
  InputState.cs      Keyboard input and per-frame pressed-state tracking
  Level.cs           Tile map, course data containers, and helper methods
  LevelFactory.cs    The five handcrafted playable courses
  MathTypes.cs       Small vector/math helpers and game enums
  Physics.cs         Collision, movement, tiles, hazards, and platform physics
  Player.cs          Core-Man movement, health, lives, scoring, and power-up state
  Program.cs         Entry point and smoke-test hook
  Renderer.cs        World rendering and pixel-art-style visuals
  UiRenderer.cs      HUD, menus, pause, game over, and victory UI
```

## Notes for Future Expansion

The game currently uses original drawn placeholder art and generated audio. New courses can be added by creating another `Level` in `LevelFactory.cs`, then exposing it through the course-selection list in `Game.cs`. New enemies can be added with another `EnemyKind` and behavior in `Enemy.cs`.

Optional original PNG assets can be placed in `assets/`. The Nintendo-labeled reference files supplied outside the project were not embedded because Omni World is keeping its art original.
