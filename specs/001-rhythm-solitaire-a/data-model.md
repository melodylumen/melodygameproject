# Data Model: Rhythm Solitaire

**Feature**: 001-rhythm-solitaire-a
**Generated**: 2025-09-26
**Source**: [spec.md](./spec.md)

## Core Entities

### Game Session
Represents a single playthrough instance with real-time state management.

**Fields**:
- `sessionId`: Unique identifier for the session
- `currentScore`: Real-time score accumulation
- `comboCount`: Current combo chain length
- `cardLayoutState`: Current positions and availability of all cards
- `musicProgression`: Current music layers and tempo state
- `startTime`: Session start timestamp
- `isActive`: Whether session is currently playable
- `difficulty`: Selected difficulty level affecting timing tolerance

**Relationships**:
- Has one `Player Profile`
- Contains multiple `Cards`
- References one `Music Track`
- May contain multiple `Power Cards`
- Belongs to one `Level/World`

**State Transitions**:
- CREATED → ACTIVE (game starts)
- ACTIVE → PAUSED (user pauses)
- PAUSED → ACTIVE (user resumes)
- ACTIVE → COMPLETED (level finished)
- ACTIVE → FAILED (no valid moves)

### Card
Represents individual playing cards with solitaire and rhythm game properties.

**Fields**:
- `cardId`: Unique identifier
- `suit`: Card suit (Hearts, Diamonds, Clubs, Spades)
- `rank`: Card rank (Ace, 2-10, Jack, Queen, King)
- `position`: Current table position coordinates
- `isAvailable`: Whether card can be moved based on solitaire rules
- `isRevealed`: Whether card is face-up
- `stackPosition`: Position within a stack (if applicable)

**Relationships**:
- Belongs to one `Game Session`
- May be blocked by other `Cards`
- May block other `Cards`

**Validation Rules**:
- Valid solitaire moves must follow traditional rules
- Card availability updates based on stack relationships
- Position changes must respect game physics

### Music Track
Represents the adaptive soundtrack with dynamic layering capabilities.

**Fields**:
- `trackId`: Unique identifier
- `genre`: Music genre (lo-fi, EDM, orchestral)
- `baseTempo`: Base beats per minute
- `currentLayers`: Active music layers array
- `synchronizationState`: Beat timing and phase information
- `adaptiveElements`: Dynamic music components that respond to gameplay

**Relationships**:
- Used by multiple `Game Sessions`
- Contains multiple `Beat Grid` references
- References audio asset files

**State Transitions**:
- LOADING → READY (track loaded)
- READY → PLAYING (session starts)
- PLAYING → LAYERING (combo triggers)
- LAYERING → DROPPING (power card activation)

### Power Card
Represents special abilities that charge based on rhythm performance.

**Fields**:
- `powerCardId`: Unique identifier
- `effectType`: Type of power effect (SHUFFLE, REVEAL, MULTIPLY, TEMPO)
- `chargeLevel`: Current charge (0-100)
- `activationConditions`: Requirements for use
- `cooldownTime`: Time before reuse
- `isActive`: Whether currently usable

**Relationships**:
- Belongs to one `Game Session`
- Requires `Beat Grid` timing for optimal activation

**Validation Rules**:
- Charge level increases with perfect beat timing
- Activation requires minimum charge threshold
- Effects must sync with musical beats

### Player Profile
Represents user progress and personalization data.

**Fields**:
- `playerId`: Unique identifier
- `highScores`: Best scores per level/mode
- `unlockedContent`: Available levels, worlds, and features
- `accessibilityPreferences`: Visual/audio accessibility settings
- `playStatistics`: Gameplay analytics and progress metrics
- `lastSyncTime`: Last cloud synchronization timestamp

**Relationships**:
- Has multiple `Game Sessions`
- Unlocks multiple `Level/World` instances
- Stores multiple `Music Track` preferences

**Validation Rules**:
- High scores must be validated and non-negative
- Unlock progression follows star requirements
- Accessibility settings affect UI/audio behavior

### Level/World
Represents game progression units with specific themes and requirements.

**Fields**:
- `levelId`: Unique identifier
- `worldId`: Parent world identifier
- `difficultySettings`: Timing tolerance and complexity parameters
- `musicTheme`: Associated music genre and tracks
- `completionCriteria`: Requirements for 1, 2, and 3 star ratings
- `unlockRequirements`: Star count needed to access
- `cardLayout`: Initial card arrangement pattern

**Relationships**:
- Contains multiple `Game Sessions`
- Belongs to one world group
- References specific `Music Tracks`

**Validation Rules**:
- Completion criteria must be measurable
- Unlock requirements enforce progression order
- Card layouts must be solvable

### Beat Grid
Represents the rhythm timing system with precise synchronization.

**Fields**:
- `beatGridId`: Unique identifier
- `currentPosition`: Current beat position in the grid
- `tempo`: Current beats per minute
- `accuracyWindows`: Timing tolerances for perfect/good/missed ratings
- `nextBeatTime`: Timestamp of next beat
- `syncOffset`: Audio-visual synchronization adjustment

**Relationships**:
- Referenced by `Music Track`
- Used by `Game Session` for timing validation
- Affects `Power Card` activation timing

**Validation Rules**:
- Beat timing must maintain <10ms accuracy
- Accuracy windows adapt based on difficulty level
- Sync offset compensates for platform-specific audio latency

## Data Persistence Strategy

**Local Storage** (Constitutional offline-first requirement):
- Game Sessions: Temporary session state in memory
- Player Profile: Local file system with encryption
- High Scores: Local database with backup
- Unlock Progress: Local storage with validation

**Optional Cloud Sync**:
- Player Profile synchronization
- Cross-device high score sharing
- Backup for critical progression data

## Performance Constraints

**Memory Management** (Constitutional compliance):
- Audio thread: Zero allocation during gameplay
- Game objects: Pre-allocated pools for cards and effects
- Session state: Stack-based temporary allocations only

**Real-Time Requirements**:
- Beat Grid updates: <10ms timing accuracy
- Card state changes: <16ms for 60fps consistency
- Music synchronization: <20ms audio latency