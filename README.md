# CMP-7042B-GamePlan-002 – Fragmented Reality Breaker

A 3D third-person action prototype built in Unity 6 where the player explores fragmented environments, collects energy nodes to stabilise a simulated reality, and survives enemy encounters across two levels.

---

## Gameplay Summary

The player is dropped into a broken, fragmented environment. To escape, they must find and collect all energy nodes scattered across the level. Enemy drones patrol the area and will chase and attack the player on sight. Falling off platforms deals damage and respawns the player at their starting position. Collecting all nodes in Level 1 triggers a transition to Level 2. Collecting all nodes in Level 2 triggers the win sequence and returns to the main menu.

---

## Controls

| Input | Action |
|-------|--------|
| WASD | Move |
| Mouse | Rotate camera |
| Space | Jump |
| Left Shift | Sprint |

---

## Game Flow

```
Main Menu → Level 1 → Level 2 → Win Screen → Main Menu
                  ↘ Game Over ↗
```

- **Main Menu** — Start game or quit
- **Level 1** — 3D platformer level with energy nodes; completing it fades and loads Level 2
- **Level 2** — Floating island level with enemy AI and energy nodes; completing it shows the win sequence
- **Game Over** — Triggered when player health reaches zero; offers Restart (Level 1) or Main Menu
- **Win Screen** — Procedural fade-in overlay ("REALITY RESTORED / YOU WIN!") then fades to black and returns to Main Menu

---

## Core Mechanics

### Energy Node Collection
- Nodes are placed throughout each level
- A UI counter tracks collected vs total (e.g. "3 / 8")
- Collecting all nodes triggers the level's win condition

### Player Health
- Max health: 500
- Health bar displayed in HUD
- Damage sources: enemy projectiles, falling off platforms (25 damage per fall)
- At 0 health: cursor unlocks, Game Over scene loads

### Respawn System
- If the player falls below Y = –15, they are teleported back to their level spawn point
- 25 fall damage is applied on each respawn
- Player velocity is reset to prevent continued falling after teleport

### Enemy AI
- Enemies have three states: **Patrol** → **Chase** → **Attack**
- Patrol: wanders between waypoints using Unity NavMesh
- Chase: triggered when player enters detection range
- Attack: fires projectiles at the player when in attack range
- NavMesh configured for platform traversal (slope: 60°, climb: 1.5 m)

### Level Transition
- End of Level 1: fade-out overlay, 1 second hold, 1.5 second black fade, loads Level 2
- End of Level 2: text fades in (0.6 s), holds (2.5 s), screen fades to black (1.5 s), loads Main Menu

---

## Scenes

| Scene | Purpose |
|-------|---------|
| `MainMenu` | Title screen with Start and Quit buttons |
| `Level1` | First playable level — ProBuilder blockout, 8 energy nodes |
| `Level2` | Second playable level — floating islands, enemy AI, 8 energy nodes |
| `GameOver` | Game over screen with Restart and Main Menu buttons |
| `MainScene` | Original sandbox/test scene |

Build index order: MainMenu (0) → Level1 (1) → Level2 (2) → GameOver (3)

---

## Project Structure

```
Assets/
├── Scenes/          # All game scenes
├── Scripts/         # All C# game logic
│   ├── GameManager.cs          # Base class: node tracking, win condition
│   ├── Level2Manager.cs        # Level 2 win sequence (procedural Canvas)
│   ├── EnemyAI.cs              # Patrol / chase / attack state machine
│   ├── EnemyHealth.cs          # Enemy damage and death
│   ├── EnemyProjectile.cs      # Projectile movement and damage
│   ├── PlayerHealth.cs         # Health, damage, death → Game Over
│   ├── PlayerRespawn.cs        # Fall detection, teleport, fall damage
│   ├── LevelTransition.cs      # Fade and load next scene
│   ├── GameOverManager.cs      # Game Over UI logic
│   ├── MainMenuManager.cs      # Main Menu UI logic
│   ├── HUDManager.cs           # In-game HUD (health bar, node counter)
│   ├── HealthBarUI.cs          # Health bar fill logic
│   ├── EnergyNodeAnimator.cs   # Node spin/pulse animation
│   └── LevelLoader.cs          # Scene loading utility
├── Materials/       # Node material, GameManager base
├── Settings/        # URP render pipeline assets (PC + Mobile)
├── StarterAssets/   # Third-person controller, animations, environment art
├── TextMesh Pro/    # Fonts and shaders for UI text
├── Editor/          # AddRespawn editor utility script
└── InputSystem_Actions.inputactions
Packages/            # Unity package manifest and lock file
ProjectSettings/     # Unity project configuration
```

---

## Technologies

| Tool | Version / Notes |
|------|-----------------|
| Unity | 6000.3.10f1 (Unity 6 LTS) |
| Render Pipeline | Universal Render Pipeline (URP) |
| Input System | Unity New Input System |
| Third-Person Controller | Unity Starter Assets |
| Level Design | ProBuilder |
| UI Text | TextMeshPro |
| Language | C# |
| NavMesh | Unity AI Navigation |

---

## Setup & Running

1. Clone the repository
2. Open in Unity 6000.3.10f1 or later
3. Open `Assets/Scenes/MainMenu.unity`
4. Press Play, or build via **File → Build Settings** (all scenes already configured)

> The project uses the New Input System. If prompted to switch input backends on first open, select **Yes**.

---

## Known Limitations

- Levels use ProBuilder blockout geometry (no final art)
- No audio/music (footstep SFX from Starter Assets only)
- Mobile input controls included in Starter Assets but not configured for this project
