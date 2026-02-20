# Game Design Document
## Bound

---

## 1. Game Overview

**Genre:** 2D Top-Down Survivor / Roguelite  
**Platform:** PC (Unity 6, URP 2D)  
**Theme:** Duality (Game Jam)  
**Target Session Length:** 5–15 minutes per run  

A Vampire Survivors–style game where the player controls an angel that transforms into a devil. The core loop revolves around managing two opposing forms: Angel (safe, ranged, healing) and Devil (aggressive, melee, high risk/reward).

---

## 2. Core Concept

The player embodies **duality** — an angel that, through killing, corrupts into a devil. The angel seeks to purify; the devil seeks destruction. The player must balance both: build corruption to unleash fury, then retreat to angel form to heal and survive.

**Tagline:** *"If something is bound to be good, it's also bound to be bad."*

---

## 3. Player Character

### Angel Form
- **Weapon:** Holy Spear — auto-targets the enemy closest to the mouse cursor, ranged, piercing
- **Health:** Passive regeneration over time
- **Movement:** Slight speed boost in Angel form (configurable multiplier)
- **XP Gems:** Heal the player when collected (Angel-only)
- **Playstyle:** Kite enemies, stay at range, build corruption by killing

### Devil Form
- **Weapon:** Katana — melee slash arc toward mouse direction, high damage, AoE
- **Health:** No regeneration; XP gems give score only, no healing
- **Playstyle:** Dive into crowds, cleave enemies, maximize score before corruption drains

### Transformation
- **Angel → Devil:** Corruption meter fills with kills; at threshold the player transforms
- **Devil → Angel:** Corruption drains over time; at 0% the player reverts
- **VFX:** Screen flash (red/devil, gold/angel), zoom punch, player sprite pulse
- **Audio:** Distinct transformation sounds for each form

---

## 4. Duality System

| Phase | Corruption Source | Threshold | Duration |
|-------|-------------------|-----------|----------|
| Angel | +X per kill | 150 (configurable) | Longer — more kills needed |
| Devil | Drains per second | 60 (configurable) | Shorter — ~7.5s base |

**Devil Form Modifiers:**
- Attack speed: 2x
- Damage: 1.5x
- Move speed: 1.15x (configurable)
- Enemies: 1.6x health, 1.4x speed, 1.5x damage, 1.6x spawn rate

**Angel Movement:** Angel has a slight speed multiplier (e.g., 1.05) to give a subtle advantage while kiting.

**Devil Kills:** Add a small amount of corruption back, slightly extending devil time.

---

## 5. Combat

### Angel — Holy Spear
- Auto-targets enemy closest to mouse cursor within range
- Piercing projectile (hits multiple enemies)
- Fire rate: ~0.45s base
- Damage: 12 base

### Devil — Katana
- Melee slash arc (120°) toward mouse direction
- AoE cleave — damages all enemies in arc
- Fire rate: ~0.22s base
- Damage: 18 base
- Range: 2.5 units
- **SFX:** Katana sound plays with a short cooldown to avoid overlapping

### Enemies
- Chase the player
- Contact damage on overlap (with cooldown)
- Drop XP gems on death
- Stats scale with devil form when spawned (tougher, faster, more damaging)

**New enemy types:**
- **Skeleton (base):** Basic fodder, quick to produce and serve as the primary horde
- **Skeleton Variants:** Recolors / small stat differences (faster / tankier variants) to add variety
- **Vampire (elite):** Stronger enemy with higher HP, higher damage; spawns only after 30s into the run (configurable)

---

## 6. Enemy Spawning

- Spawner uses a configurable spawn table where each entry defines a prefab, earliest spawn time (seconds), and weight.
- Example table:
  - Skeleton: earliestSpawnTime = 0, weight = 3
  - Skeleton (variant): earliestSpawnTime = 10, weight = 2
  - Vampire: earliestSpawnTime = 30, weight = 1
- Spawner ramps up spawn rate over time via `difficultyRampTime` and is impacted by Devil form spawn rate multiplier.

---

## 7. Progression & Score

- **Score:** Increases with kills and XP gem pickups
- **Devil Multiplier:** 3x score when in devil form
- **XP Gems:** Heal in angel form; give score in both forms
- **Goal:** Survive as long as possible, maximize score by farming in devil form

---

## 8. Controls

| Input | Action |
|-------|--------|
| WASD / Arrows | Move |
| Mouse | Aim / target (weapon auto-fires) |

No manual attack — weapons fire automatically when a target is in range (angel) or continuously (devil).

---

## 9. UI & HUD

### In-Game
- Health bar (top-left)
- Corruption bar (top-left, below health)
- Kill count
- Score
- Timer

### Game Over
- Final score (center, large)
- Enemies killed
- Time survived
- Restart / Main Menu buttons

### Main Menu
- Game title
- New Game
- How To Play
- Exit

### How To Play
- Brief explanation of angel vs devil mechanics
- Tip: manage fury for XP, retreat to angel to regen

---

## 10. Audio

| Type | Description |
|------|-------------|
| Music | Looping background track (~25s) |
| Spear | Sound per spear shot |
| Katana | Sound per swing (with cooldown to avoid overlap) |
| Transform | Sound on Angel→Devil and Devil→Angel |

---

## 11. Visual Style

- **Art:** Pixel art, simple sprites (Aseprite)
- **Resolution:** 16x16–32x32 sprites
- **Background:** Tiled 16x16 gray texture, infinite scroll
- **Color:** Angel = white/gold; Devil = red/dark
- **VFX:** Screen flash, zoom punch, sprite pulse on transform

### Palette (Ephemera)
The game's entire visual identity uses the EPHEMERA palette.

Palette (Ephemera) — hex values:
- #392C31
- #4A3C4A
- #5A555A
- #62696A
- #73817B
- #83898B
- #83918B
- #A49DA4
- #C5B2BD
- #D5BECD
- #DED6DE
- #E6EAEE

Link: https://lospec.com/palette-list/ephemera

---

## 12. Scenes & Flow

1. **MainMenu** — Title, New Game, How To Play, Exit
2. **Quote** — \"If something is bound to be good, it's also bound to be bad\" (fade in/out)
3. **MainGameplay** — Core survivor loop
4. **Game Over** — Overlay with stats, Restart, Main Menu

---

## 13. Technical Notes

- **Engine:** Unity 6, URP 2D
- **Input:** New Input System
- **Camera:** Cinemachine follow
- **Collision:** 2D physics, trigger-based for projectiles and contact damage

---

## 14. Design Pillars

1. **Duality** — Two forms, two playstyles, one character
2. **Risk/Reward** — Devil = high score, high danger; Angel = safe, heal, build up
3. **Accessibility** — Auto-aim, simple controls, short runs
4. **Juice** — Flash, zoom, sound, feedback on every transform

