## Game Features

| Assignment Point | Status | What Was Done | Where |
|---|---|---|---|
| Character control with keyboard / mouse / controller | ✅ Implemented | Player gameplay uses the New Input System for movement and dash actions. Menu navigation also supports controller-style input flow. | `Assets/_Code/MainGame/Player/PlayerController.cs`<br>`Assets/_Code/MainGame/Input/InputActionsBootstrap.cs`<br>`Assets/_Code/UI/MenuNavigation/GamepadMenuRouter.cs`<br>`Assets/_Code/UI/MenuNavigation/GlobalMenuNavigator.cs` |
| Use of animation system for the character / multiple objects | ✅ Implemented | The player uses an Animator, and the project also contains animation controllers for enemies, buffs, and win / lose UI states. | `Assets/_Code/MainGame/Player/PlayerVisualController.cs`<br>`Assets/Animation/Player/PlayerPenguin.controller`<br>`Assets/Animation/Enemy/EnemyController.controller`<br>`Assets/Animation/UI/HappyPenguinOnWon.controller`<br>`Assets/Animation/UI/SadPenguinOnLost.controller` |
| Transition between multiple scenes, with at least 3 scenes | ✅ Implemented | The game includes `MainMenu`, `Level 1`, `Level 2`, and `Level 3`. | `ProjectSettings/EditorBuildSettings.asset` |
| Options screen controlling volume and additional settings | ✅ Implemented | The options menu controls master, music, and effects volume through sliders and an Audio Mixer. These settings are also saved. | `Assets/_Code/UI/AudioSettingsUI.cs`<br>`Assets/_Code/MainGame/Save/SaveData.cs`<br>`Assets/_Code/MainGame/Save/SaveManager.cs` |
| Game over screen and victory screen | ✅ Implemented | Separate win and lose flows are implemented. Dedicated win / lose visual assets are included. | `Assets/_Code/MainGame/WinCondition/WinConditionManager.cs`<br>`Assets/Animation/UI/HappyPenguinOnWon.controller`<br>`Assets/Animation/UI/SadPenguinOnLost.controller` |
| Pause during gameplay + open options | ✅ Implemented | The game can be paused and resumed through a dedicated pause manager. The UI system also supports switching canvases and opening menu screens during gameplay flow. | `Assets/_Code/MainGame/PauseManager.cs`<br>`Assets/_Code/UI/CanvasSwitcher.cs`<br>`Assets/_Code/UI/AudioSettingsUI.cs` |
| Save and load | ✅ Implemented | Game progress and settings are saved and loaded as JSON files from persistent storage. | `Assets/_Code/MainGame/Save/SaveManager.cs`<br>`Assets/_Code/MainGame/Save/SaveData.cs`<br>`Assets/_Code/MainGame/Save/ProgressSaver.cs` |
| Challenge to overcome + UI showing progress toward win / loss | ✅ Implemented | The core challenge is surviving enemy pressure and progressing through timed gameplay. The project includes enemy AI, enemy spawning, difficulty updates, timer logic, win / lose events, and gameplay UI feedback such as dash cooldown display. | `Assets/_Code/MainGame/Enemy/EnemyController.cs`<br>`Assets/_Code/MainGame/Enemy/EnemySpawner.cs`<br>`Assets/_Code/MainGame/Enemy/Difficulty/DifficultySystem.cs`<br>`Assets/_Code/MainGame/WinCondition/Timer.cs`<br>`Assets/_Code/MainGame/WinCondition/WinConditionManager.cs`<br>`Assets/_Code/UI/DashCooldownUI.cs` |

## Mandatory Requirements

| Assignment Point | Status | What Was Done | Where |
|---|---|---|---|
| Prefab Variant | ✅ Implemented | Enemy prefabs were created as prefab variants based on a shared source prefab. | `Assets/Prefabs/Characters/Enemy Hi.prefab`<br>`Assets/Prefabs/Characters/Enemy Mid.prefab` |
| Save / Load using JSON or binary / text file | ✅ Implemented | Save and load were implemented with JSON serialization and file I/O. | `Assets/_Code/MainGame/Save/SaveManager.cs` |

## Base Requirements

| Assignment Point | Status | What Was Done | Where |
|---|---|---|---|
| Async scene loading | ❌ Not implemented | Scene transitions currently use regular scene loading and not async loading. | `Assets/_Code/UI/SceneLoader.cs` |
| Coroutine | ✅ Implemented | Coroutines are used for timed gameplay behavior such as enemy spawning and cooldown UI updates. | `Assets/_Code/MainGame/Enemy/EnemySpawner.cs`<br>`Assets/_Code/UI/DashCooldownUI.cs` |
| Scriptable Objects | ✅ Implemented | Buff configuration data is stored in ScriptableObject assets. | `Assets/_Code/MainGame/Buff/BuffSetup.cs` |
| Unity / C# Events | ✅ Implemented | Multiple gameplay systems use UnityEvents for player state, difficulty, timer flow, and win / lose logic. | `Assets/_Code/MainGame/Player/PlayerController.cs`<br>`Assets/_Code/MainGame/Enemy/Difficulty/DifficultySystem.cs`<br>`Assets/_Code/MainGame/WinCondition/WinConditionManager.cs`<br>`Assets/_Code/MainGame/PlayerStateChannel.cs`<br>`Assets/_Code/MainGame/WinCondition/Timer.cs` |
| PlayerPrefs | ❌ Not implemented | PlayerPrefs were not used in this branch. | — |
| Old Input System | ❌ Not used | The project uses the New Input System instead of the old input API. | `Assets/_Code/MainGame/Player/PlayerController.cs`<br>`Assets/_Code/MainGame/Input/InputActionsBootstrap.cs` |
| Animator | ✅ Implemented | Animator parameters are updated directly from gameplay code, and multiple animation controllers are used in the project. | `Assets/_Code/MainGame/Player/PlayerVisualController.cs`<br>`Assets/Animation/` |
| Transform Movement | ✅ Implemented | Transform-based movement is used in gameplay / camera-related code. | `Assets/_Code/MainGame/Camera/CameraPlayerFollower.cs`<br>`Assets/_Code/MainGame/Player/PlayerController.cs` |
| Navigation System | ✅ Implemented | Enemies use Unity navigation through `NavMeshAgent`. | `Assets/_Code/MainGame/Enemy/EnemyController.cs` |
| UI | ✅ Implemented | The project contains menu UI, gameplay UI, options UI, level selection, navigation, and cooldown feedback. | `Assets/_Code/UI/AudioSettingsUI.cs`<br>`Assets/_Code/UI/CanvasSwitcher.cs`<br>`Assets/_Code/UI/LevelSelectButtons.cs`<br>`Assets/_Code/UI/DashCooldownUI.cs`<br>`Assets/_Code/UI/MenuNavigation/` |
| Lighting Baking | ❌ Not implemented | Baked lighting was not used in this project. The game uses a 2D sprite-based pipeline, so this was not part of the implemented visual setup. | — |

## Advanced Requirements

| Assignment Point | Status | What Was Done | Where |
|---|---|---|---|
| Job System | ❌ Not implemented | No Job System usage was added in this branch. | — |
| Custom Inspector | ✅ Implemented | A custom inspector was created for the map painter tool. | `Assets/_Code/Editor/Map/MapPainterEditor.cs` |
| New Input System | ✅ Implemented | The project uses Unity's New Input System for gameplay and menu interaction. | `Packages/manifest.json`<br>`Assets/_Code/MainGame/Input/InputActionsBootstrap.cs`<br>`Assets/_Code/MainGame/Player/PlayerController.cs`<br>`Assets/_Code/UI/MenuNavigation/GamepadMenuRouter.cs` |
| Complex Animator - Layers / Blend | ❌ Not claimed | Advanced animator layers / blend trees are not being claimed in this README. | — |
| Audio Mixer | ✅ Implemented | Audio settings control mixer parameters for master, music, and effects channels. | `Assets/_Code/UI/AudioSettingsUI.cs` |
| Editor scripts | ✅ Implemented | The project includes editor-only tools for hierarchy settings and map painting. | `Assets/_Code/Editor/HierarchyPainter/HierarchyPainterSettingsProvider.cs`<br>`Assets/_Code/Editor/Map/MapPainterEditor.cs` |
| Controller Support | ✅ Implemented | Controller-oriented input flow is supported through the New Input System in gameplay and menus. | `Assets/_Code/MainGame/Player/PlayerController.cs`<br>`Assets/_Code/UI/MenuNavigation/GamepadMenuRouter.cs`<br>`Assets/_Code/UI/MenuNavigation/GlobalMenuNavigator.cs` |
| Addressables | ❌ Not implemented | Addressables were not used in this branch. | — |
| Assembly Definitions | ✅ Implemented | The codebase is split into separate assemblies for editor, main game, and UI systems. | `Assets/_Code/Editor/EditorDef.asmdef`<br>`Assets/_Code/MainGame/MainGameDef.asmdef`<br>`Assets/_Code/UI/UIDef.asmdef` |

## Order and Efficiency

| Assignment Point | Status | What Was Done | Where |
|---|---|---|---|
| Order and efficiency | ✅ Implemented | The project is organized into separate gameplay, UI, save, editor, enemy, player, and win-condition systems. Assembly Definitions were also used to separate code domains. | `Assets/_Code/MainGame/`<br>`Assets/_Code/UI/`<br>`Assets/_Code/Editor/`<br>`.asmdef` files under `Assets/_Code/` |