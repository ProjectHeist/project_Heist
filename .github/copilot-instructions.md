# Copilot Instructions - Project Bank Robbers

## Project Overview
A 3D turn-based tactical game (Unity 2021.3.17f1) where players control thief characters to infiltrate bank locations, avoid detection by guards, and extract successfully. Focus: gameplay mechanics, state management, procedural turn sequences.

## Architecture

### Scene Organization (Outgame vs Ingame)
- **Outgame** (`Assets/Scripts/Outgame/`): Persistent UI/meta layer across scenes (Title → Lobby → MissionScene → actual game)
  - `DataManager`: Singleton managing save/load of game state (JSON to `persistentDataPath`)
  - `GameManager`: Singleton (DontDestroyOnLoad) holding `currentMaster` (MasterData with money, stages, unlocked players/weapons)
  - Scenes: TitleScene → LobbyScene (team selection) → MissionScene (loadout prep) → ingame scenes
- **Ingame** (`Assets/Scripts/Ingame/`): Active gameplay logic, turns, character actions
  - `IngameManager`: Singleton orchestrating turn cycles, enemy phases, win/lose conditions
  - Characters, Map, Logics subsystems

### Core Data Model
**Database** (ScriptableObject, cached in `Resources/Data/TotalDatabase`):
- `PlayerDatabase`: Available player characters (stats, abilities)
- `WeaponDatabase`: Available weapons (accuracy, damage modifiers)
- `MapDatabase`, `EnemyDatabase`: Stage-specific setup
- `EXDatabase`, `TagDatabase`: Special abilities and grid tags

**MasterData** (JSON serializable, per save slot):
```csharp
MasterData: { money, levels[], stage, playerNumbers[], weaponNumbers[] }
MasterDatabase: { List<MasterData> masterDatas }  // one per player save
```

### Turn System & Control Flow
1. **Player Phase** (`InputManager.cs`): 
   - Click grid → select player with `PlayerState.canMove > 0`
   - Actions: Move (pathfinding), Attack (range-checked), EX ability (cooldown-tracked)
   - `PlayerController.SetCurrentPlayer(player)` → UI updates, range highlight via `DisplayRange`

2. **Enemy Phase** (each enemy calls `EnemyBehaviour.EnemyAct()`):
   - Patrol routes or Alert/Chase patterns
   - Suspicion tracking per player (`EnemyState.suspicion[playerIdx]`)
   - Detection via `EnemyVision` raycasts; immediate Attack if detected

3. **Turn Increment** & Cooldown Reset:
   - `IngameManager.turn++` after both phases
   - PlayerEX cooldowns, buff durations decrement

### State Management Patterns

**Character State** (base class for player/enemy):
- HP, accuracy, moveRange, damage, attackRange, critRate
- Buff system: `BuffInfo` tracks stat, duration, value modifiers
- `GetStatValue(stats.damage)` applies active buffs

**PlayerState** (extends CharacterState):
- `playerIndex` (0-2), `playerName`, equipped weapon reference
- `canMove`, `canAttack`, `EXcooldown` (action limit counters)
- `enemyDetectedPlayer`: list of enemies that saw this player
- Methods: `TakeDamage()`, `SetState()`, `Move(GridCell)`

**EnemyState** (extends CharacterState):
- `suspicion[]` array tracking per-player suspicion (0-100, causes Alert at 50)
- `routeNum` for patrol route selection
- `faceDir`: direction-based vision cone

### Communication Patterns
- **Singleton Access**: `IngameManager.Instance`, `GameManager.Instance`, `PrepareManager.Instance`
- **Component Queries**: Heavy use of `GetComponent<>()` for stat/state retrieval on GameObjects
- **Prefab Instantiation**: Player/Enemy prefabs instantiated at runtime with state configured via `.SetState()`
- **Event Callbacks**: Button click listeners via `OnClickListener`, coroutines for async actions (`yield return WaitUntil()`)

## Key Development Workflows

### Adding New Player/Weapon
1. Create `PlayerStat` or `WeaponStat` ScriptableObject in `Assets/Resources/Data/`
2. Add to `PlayerDatabase.playerlist` or `WeaponDatabase.weaponlist`
3. Update UI button generation in `BuyPlayer.cs` or `BuyWeapon.cs` (uses `Instantiate()` + `GetComponent<Button>()`)

### New Enemy Ability / EX Skill
1. Create new class in `Assets/Scripts/Outgame/DatabaseFolder/EXSkills/` extending `PlayerEX` (namespace: `Ingame`)
2. Implement `Activate(GameObject target)` method
3. Register in `EXDatabase.exSkillList`
4. Trigger from `InputManager.cs` player input handler

### Map/Level Design
- `MapData` (ScriptableObject) defines walkable tiles, forbidden zones, patrol routes
- `MapCreator` procedurally instantiates `GridCell` prefabs at runtime
- `GridCell` tracks occupants, visual color state, walkability
- Patrol routes defined in `MapData.routes[]`, assigned to enemies on spawn

### Debugging Gameplay
- `IngameManager.turn` shows current turn counter
- `IngameManager.isEnemyPhase` flag distinguishes phases
- `PlayerState.enemyDetectedPlayer` indicates visibility state
- `ControlUI` panel displays action availability (Move/Attack/EX buttons enabled if counters > 0)
- Enable `DisplayRange` visuals to see movement/attack ranges

## Code Patterns & Conventions

**State Queries**:
```csharp
// Standard pattern: Get component, then query state
PlayerState playerState = player.GetComponent<PlayerState>();
if (playerState.canMove > 0) { /* allow action */ }
```

**Coroutine Sequencing** (IngameManager):
```csharp
// Wait for async enemy action to complete
yield return new WaitUntil(() => enemies[i].GetComponent<EnemyBehaviour>().actFinished);
```

**UI Updates via Prefab Cloning**:
```csharp
// Dynamic button generation in shops/selection screens
GameObject btn = Instantiate(prefabTemplate);
btn.GetComponentInChildren<TextMeshProUGUI>().text = itemName;
btn.GetComponent<Button>().onClick.AddListener(() => OnItemSelected(index));
```

**Buff Application**:
```csharp
// CharacterState tracks active buffs; stat queries apply modifiers
buffs.Add(new BuffInfo(stats.damage, duration, multiplier));
// GetStatValue() loops buffs and returns base + (base * modifier)
```

## Important Notes

1. **Unity Version**: 2021.3.17f1 — avoid newer syntax; use traditional coroutines/callbacks
2. **No Tests**: Gameplay validation is manual in-editor; focus on deterministic state changes
3. **Serialization**: Save/load uses `JsonUtility` (not Newtonsoft); fields must be `[System.Serializable]`
4. **Resource Loading**: Static asset references via `Resources.Load<>()` (cached in GameManager)
5. **Grid System**: World positions use Unity transform; logical grid (int x, y) via `GridCell` lookup in `mapManager.tile[x,y]`
6. **Detection**: No NavMesh; pathfinding is manual tile-based using A* (in `PlayerController`)
7. **Language**: Code comments in Korean; variable names in English; follow existing naming (camelCase for locals, PascalCase for classes)
