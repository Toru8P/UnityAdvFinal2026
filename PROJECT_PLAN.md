## UnityAdvFinal2026 – Work Plan and Remaining Features


## 1. Remaining Required Features (High‑Level Checklist)

### 1.1 Mandatory / Core systems

- **Save and Load System**  
  - Implement persistent save/load using JSON or binary files.  
  - Connect to gameplay flow (main menu, checkpoints, or explicit save points).

- **Options Menu and Player Settings**  
  - Options menu accessible from both **main menu** and **pause menu while playing**.  
  - Volume sliders (master/music/SFX) and at least one additional setting (e.g., difficulty or sensitivity).  
  - Settings persisted via `PlayerPrefs`.

- **Asynchronous Scene Loading**  
  - Scene transitions handled via `SceneManager.LoadSceneAsync`.  
  - Dedicated loading screen with visible progress (slider, bar, or percentage).

### 1.2 Feature polish / feedback

- **Immunity Buff Integration**  
  - Finalize immunity buff as Scriptable Object and wire it into the existing buff system.  
  - Ensure proper interaction with damage and invulnerability windows.

- **Buff Visual Feedback**  
  - Add character animation feedback (Animator state/layer) when immunity buff is active.  
  - Optional: UI icon/timer showing active buff duration.

- **Lighting Baking Review**  
  - Verify scenes use baked lighting where appropriate for performance.  
  - Adjust lightmaps and mixed lighting as needed.

---

## 2. Mapping to GitHub Issues

- **[Required] Save System**  
  - Covers: Save/Load requirement and basic Save/Load feature.  
  - Related work: Section 3.1.

- **[Required] Player settings**  
  - Covers: PlayerPrefs usage and configuration storage.  
  - Related work: Section 3.2.

- **[Required] Options button in the menu while playing and in main menu**  
  - Covers: Options menu visibility and navigation in both menus.  
  - Related work: Section 3.2.

- **[Required] Asynchronous scene loading – load scenes asynchronously and display loading progress**  
  - Covers: Asynchronous scene loading core feature.  
  - Related work: Section 3.3.

- **[Required] Readme**  
  - Partially covered by current `README.md`.  
  - Final polish: ensure the README fully reflects all implemented features and how they map to course requirements.

- **Immunity buff**  
  - Covers: buff system extension and Scriptable Object usage.  
  - Related work: Section 3.4.

- **Animate character during buff**  
  - Covers: Animator usage and visual feedback for buffs.  
  - Related work: Section 3.4.

---

## 3. Implementation Plan by Area

### 3.1 Save and Load System

**Goal:** Implement robust save/load covering scene, player state and essential progression.

- Create a central **SaveManager** (singleton or persistent object) that:
  - Holds a serializable `SaveData` class with all required fields.  
  - Serializes/deserializes `SaveData` to/from JSON or binary.  
  - Reads/writes to `Application.persistentDataPath`.
- Define a simple interface or pattern for systems that need persistence, e.g.:
  - `CaptureState(SaveData saveData)` and `RestoreState(SaveData saveData)`.  
  - Player, progression, unlocked content and other critical systems implement these methods.
- Add UI hooks:
  - In main menu: **New Game**, **Continue/Load** (if save file exists).  
  - In‑game: optional manual save at checkpoints or explicit save triggers.
- Testing:
  - Save in mid‑game, quit, reload the game and verify scene, player state and progress are correctly restored.

### 3.2 Options Menu and Player Settings

**Goal:** Provide configurable audio and gameplay settings, stored via `PlayerPrefs`.

- Implement a **SettingsManager** responsible for:
  - Reading/writing values from/to `PlayerPrefs`.  
  - Applying audio settings to the `AudioMixer` (master/music/SFX volumes).  
  - Exposing API like `GetVolume`, `SetVolume`, `GetSetting<T>`, `SetSetting<T>`.
- Build a reusable **Options UI** (single prefab or canvas section):
  - Sliders for master/music/SFX volume.  
  - At least one additional setting (e.g. difficulty, mouse sensitivity, gamepad vibration).
  - Buttons: **Apply**, **Back/Close**.
- Integrate Options UI in:
  - **Main menu** (Options button opens the same Options UI).  
  - **Pause menu** (Options button accessible during gameplay, while paused).
- On opening Options:
  - Read current values from `SettingsManager` / `PlayerPrefs`.  
  - Update sliders and toggles.  
  - On change, update runtime state and persist.

### 3.3 Asynchronous Scene Loading

**Goal:** Replace blocking scene transitions with asynchronous loading and clear feedback.

- Introduce a **SceneLoader** component or static service with methods like:
  - `LoadSceneAsync(string sceneName)` or `LoadSceneAsync(SceneId id)`.  
  - Optional wrapper for additive loading if needed.
- Use `SceneManager.LoadSceneAsync`:
  - Show a loading screen canvas with a progress bar or percentage text.  
  - Update UI using `operation.progress` until the scene is fully loaded.  
  - Optionally control activation via `allowSceneActivation`.
- Integrate with existing flow:
  - Replace direct `SceneManager.LoadScene` calls in menus and game logic.  
  - Ensure Game Over / Victory transitions also use the SceneLoader.
- Testing:
  - Switch between all gameplay scenes, confirm UI feedback and no noticeable frame‑freeze.

### 3.4 Buff System and Visual Feedback

**Goal:** Finalize immunity buff and make its effect visually clear to the player.

- Extend the existing buff system:
  - Define an **ImmunityBuff** Scriptable Object (duration, effect type, stack rules).  
  - Ensure damage/health system checks for active immunity before applying damage.
- Animator integration:
  - Add Animator parameter(s) and state/ layer for “buff active” visual.  
  - On buff start: set parameter and transition to buff state (or enable layer).  
  - On buff end: reset parameter and return to normal state.
- UI feedback:
  - Optional buff icon with countdown timer.  
  - Clear visual for when immunity starts and ends (flash, outline, color change).

### 3.5 Lighting Baking Review

**Goal:** Optimize performance using baked lighting where appropriate.

- For each main scene:
  - Decide which lights can be baked, mixed, or real‑time only.  
  - Bake lightmaps and verify visual quality and performance.  
  - Adjust lightmap resolution and ambient settings as needed.

---

## 4. Suggested Order of Work

1. **Save and Load System**  
   - Unblocks testing across sessions and connects directly to course requirements.

2. **Options Menu + Player Settings (PlayerPrefs)**  
   - Completes mandatory options feature and improves usability.

3. **Asynchronous Scene Loading**  
   - Improves UX during transitions and fulfills another core requirement.

4. **Immunity Buff + Buff Visual Feedback**  
   - Polishes gameplay and leverages existing buff and animation systems.

5. **Lighting Baking Review and Minor Polish**  
   - Final performance/visual pass before submission.

6. **README and Documentation Final Pass**  
   - Verify `README.md` and internal docs clearly describe how all features map to course requirements.

