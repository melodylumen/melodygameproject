# Tasks: Rhythm Solitaire

**Input**: Design documents from `/specs/001-rhythm-solitaire-a/`
**Prerequisites**: plan.md ✅, research.md ✅, data-model.md ✅, contracts/ ✅, quickstart.md ✅

## Execution Flow (main)
```
1. Load plan.md from feature directory ✅
   → Extract: Unity 6 LTS, FMOD Studio, C# 9.0+, cross-platform
2. Load design documents ✅:
   → data-model.md: 7 entities → 7 model tasks
   → contracts/: 4 interfaces → 4 contract test tasks
   → quickstart.md: 7 scenarios → 7 integration test tasks
   → research.md: Unity/FMOD decisions → setup tasks
3. Generate tasks by category:
   → Setup: Unity project, FMOD integration, constitutional tooling
   → Tests: contract tests, integration tests (TDD requirement)
   → Core: models, engines, systems
   → Integration: audio synchronization, UI, persistence
   → Polish: performance validation, accessibility tests
4. Apply constitutional compliance:
   → Audio Quality First: <20ms latency verification
   → Real-Time Performance: Memory allocation constraints
   → Test-Driven Development: Tests before implementation
   → Modular Architecture: Interface-based design
   → Accessibility: Visual indicators and WCAG compliance
5. Number tasks sequentially (T001, T002...)
6. Generate dependency graph with parallel execution
7. Validate constitutional compliance integration
```

## Format: `[ID] [P?] Description`
- **[P]**: Can run in parallel (different files, no dependencies)
- Include exact file paths in descriptions

## Path Conventions
**Unity Game Engine Structure**:
- **Scripts**: `Assets/Scripts/` (organized by domain)
- **Tests**: `Tests/EditMode/` and `Tests/PlayMode/`
- **Assets**: `Assets/Audio/`, `Assets/Art/`, `Assets/Data/`

## Phase 3.1: Setup & Constitutional Foundation
- [x] T001 Create Unity 6 LTS project structure per implementation plan in project root
- [ ] T002 Install and configure FMOD Studio 2.02+ integration package in Unity
- [x] T003 [P] Setup constitutional compliance tooling (Unity Profiler, LatencyMon integration)
- [x] T004 [P] Configure Unity Test Framework 1.1.33+ for EditMode and PlayMode testing
- [x] T005 [P] Setup project structure: Assets/Scripts/{Audio,Cards,UI,GameModes,Progression,PowerCards,Core}

## Phase 3.2: Contract Tests First (TDD) ⚠️ MUST COMPLETE BEFORE 3.3
**CRITICAL: These tests MUST be written and MUST FAIL before ANY implementation**
- [x] T006 [P] Contract test IAudioEngine interface in Tests/EditMode/Audio/AudioEngineContractTests.cs
- [x] T007 [P] Contract test ICardGameEngine interface in Tests/EditMode/Cards/CardGameEngineContractTests.cs
- [x] T008 [P] Contract test IPowerCardSystem interface in Tests/EditMode/PowerCards/PowerCardSystemContractTests.cs
- [x] T009 [P] Contract test IPlayerProgressSystem interface in Tests/EditMode/Progression/PlayerProgressSystemContractTests.cs
- [x] T010 [P] Constitutional compliance test: Audio latency <20ms validation in Tests/Performance/AudioLatencyTests.cs
- [x] T011 [P] Constitutional compliance test: Memory allocation verification in Tests/Performance/MemoryAllocationTests.cs

## Phase 3.3: Entity Models (ONLY after contract tests are failing)
- [x] T012 [P] GameSession entity model in Assets/Scripts/Core/GameSession.cs
- [x] T013 [P] Card entity model in Assets/Scripts/Cards/Card.cs
- [x] T014 [P] MusicTrack entity model in Assets/Scripts/Audio/MusicTrack.cs
- [x] T015 [P] PowerCard entity model in Assets/Scripts/PowerCards/PowerCard.cs
- [x] T016 [P] PlayerProfile entity model in Assets/Scripts/Progression/PlayerProfile.cs
- [x] T017 [P] Level/World entity models in Assets/Scripts/Progression/Level.cs and World.cs
- [x] T018 [P] BeatGrid entity model in Assets/Scripts/Audio/BeatGrid.cs

## Phase 3.4: Core Engine Implementation (TDD Sequential)
- [ ] T019 AudioEngine implementation with constitutional memory constraints in Assets/Scripts/Audio/AudioEngine.cs
- [ ] T020 CardGameEngine implementation with deterministic testing support in Assets/Scripts/Cards/CardGameEngine.cs
- [ ] T021 PowerCardSystem implementation with rhythm synchronization in Assets/Scripts/PowerCards/PowerCardSystem.cs
- [ ] T022 PlayerProgressSystem implementation with offline-first architecture in Assets/Scripts/Progression/PlayerProgressSystem.cs

## Phase 3.5: Integration Tests (Quickstart Scenarios)
- [ ] T023 [P] Integration test: Basic rhythm gameplay scenario in Tests/PlayMode/Scenarios/BasicRhythmGameplayTests.cs
- [ ] T024 [P] Integration test: Combo chain building scenario in Tests/PlayMode/Scenarios/ComboChainBuildingTests.cs
- [ ] T025 [P] Integration test: Off-beat move handling scenario in Tests/PlayMode/Scenarios/OffBeatMoveHandlingTests.cs
- [ ] T026 [P] Integration test: Power card activation scenario in Tests/PlayMode/Scenarios/PowerCardActivationTests.cs
- [ ] T027 [P] Integration test: Accessibility features scenario in Tests/PlayMode/Scenarios/AccessibilityFeaturesTests.cs
- [ ] T028 [P] Integration test: Pause and resume scenario in Tests/PlayMode/Scenarios/PauseResumeTests.cs
- [ ] T029 [P] Integration test: Rapid move handling scenario in Tests/PlayMode/Scenarios/RapidMoveHandlingTests.cs

## Phase 3.6: Audio-Visual Synchronization
- [ ] T030 FMOD Studio beat synchronization implementation in Assets/Scripts/Audio/BeatSynchronizer.cs
- [ ] T031 Visual beat indicators for accessibility in Assets/Scripts/UI/VisualBeatIndicator.cs
- [ ] T032 Dynamic music layering system in Assets/Scripts/Audio/MusicLayerManager.cs
- [ ] T033 Audio-video sync validation (<10ms tolerance) in Assets/Scripts/Audio/SyncValidator.cs

## Phase 3.7: Game Systems Integration
- [ ] T034 Game session management with state persistence in Assets/Scripts/Core/GameSessionManager.cs
- [ ] T035 UI components for score, combo, and power card display in Assets/Scripts/UI/GameplayUI.cs
- [ ] T036 Input handling with rhythm timing validation in Assets/Scripts/Core/InputManager.cs
- [ ] T037 Level progression and star rating system in Assets/Scripts/Progression/ProgressionManager.cs

## Phase 3.8: Constitutional Performance Validation
- [ ] T038 [P] 60fps stability validation across target platforms in Tests/Performance/FrameRateTests.cs
- [ ] T039 [P] Audio latency measurement and <20ms verification in Tests/Performance/AudioLatencyValidationTests.cs
- [ ] T040 [P] Memory allocation monitoring (zero allocation in audio threads) in Tests/Performance/MemoryProfileTests.cs
- [ ] T041 [P] Timing synchronization accuracy testing (<10ms tolerance) in Tests/Performance/TimingSyncTests.cs

## Phase 3.9: Accessibility & Polish
- [ ] T042 [P] WCAG 2.1 AA compliance validation in Tests/Accessibility/WCAGComplianceTests.cs
- [ ] T043 [P] Visual-only gameplay testing (no audio) in Tests/Accessibility/VisualOnlyGameplayTests.cs
- [ ] T044 [P] Adaptive timing tolerance implementation in Assets/Scripts/Audio/AdaptiveTimingManager.cs
- [ ] T045 [P] Cross-platform input handling for PC/Switch/Mobile in Assets/Scripts/Core/PlatformInputAdapter.cs
- [ ] T046 Execute all quickstart.md scenarios for manual validation
- [ ] T047 Performance benchmarking and optimization verification

## Dependencies
**Constitutional Order (TDD):**
- Setup (T001-T005) before all tests
- Contract tests (T006-T011) before implementation (T012-T022)
- Entity models (T012-T018) before engine implementation (T019-T022)
- Core engines (T019-T022) before integration tests (T023-T029)
- Integration complete before audio-visual sync (T030-T033)
- Core systems (T034-T037) before performance validation (T038-T041)
- Performance validation before accessibility polish (T042-T047)

**Specific Dependencies:**
- T002 (FMOD) blocks T006, T010, T019, T030-T033
- T004 (Test Framework) blocks all test tasks (T006-T011, T023-T029, T038-T043)
- T012-T018 (Models) block T019-T022 (Engines)
- T019 (AudioEngine) blocks T030-T033 (Audio sync)
- T020 (CardGameEngine) blocks T034 (GameSessionManager)
- T021 (PowerCardSystem) blocks T035 (GameplayUI)
- T022 (PlayerProgressSystem) blocks T037 (ProgressionManager)

## Parallel Execution Examples
```
# Phase 3.1 - Setup tasks can run in parallel:
Task: "Setup constitutional compliance tooling (Unity Profiler, LatencyMon integration)"
Task: "Configure Unity Test Framework 1.1.33+ for EditMode and PlayMode testing"
Task: "Setup project structure: Assets/Scripts/{Audio,Cards,UI,GameModes,Progression,PowerCards,Core}"

# Phase 3.2 - Contract tests can run in parallel:
Task: "Contract test IAudioEngine interface in Tests/EditMode/Audio/AudioEngineContractTests.cs"
Task: "Contract test ICardGameEngine interface in Tests/EditMode/Cards/CardGameEngineContractTests.cs"
Task: "Contract test IPowerCardSystem interface in Tests/EditMode/PowerCards/PowerCardSystemContractTests.cs"
Task: "Contract test IPlayerProgressSystem interface in Tests/EditMode/Progression/PlayerProgressSystemContractTests.cs"

# Phase 3.3 - Entity models can run in parallel:
Task: "GameSession entity model in Assets/Scripts/Core/GameSession.cs"
Task: "Card entity model in Assets/Scripts/Cards/Card.cs"
Task: "MusicTrack entity model in Assets/Scripts/Audio/MusicTrack.cs"
Task: "PowerCard entity model in Assets/Scripts/PowerCards/PowerCard.cs"
```

## Constitutional Compliance Integration
**Every task includes constitutional verification:**
- **Audio Quality First**: T010, T019, T030-T033, T038-T041 enforce <20ms latency
- **Real-Time Performance**: T011, T019, T040 validate memory constraints and 60fps
- **Test-Driven Development**: T006-T011 must fail before T012-T022 implementation
- **Modular Architecture**: T019-T022 implement interface contracts with loose coupling
- **Accessibility**: T042-T044 ensure WCAG compliance and visual indicators

## Notes
- [P] tasks = different files, no dependencies, can run in parallel
- All contract tests MUST fail before implementation begins (TDD requirement)
- Constitutional compliance verification integrated into each implementation task
- Audio thread memory allocation is FORBIDDEN (constitutional violation)
- Performance targets: 60fps stable, <20ms audio latency, <10ms sync tolerance
- Cross-platform deployment: PC, Nintendo Switch, iOS/Android

## Validation Checklist
- [x] All contracts have corresponding tests (T006-T009)
- [x] All entities have model tasks (T012-T018)
- [x] All tests come before implementation (TDD order)
- [x] Parallel tasks truly independent ([P] marking verified)
- [x] Each task specifies exact file path
- [x] No task modifies same file as another [P] task
- [x] Constitutional compliance integrated throughout
- [x] Quickstart scenarios covered in integration tests (T023-T029)

**Total Tasks**: 47 tasks ordered by constitutional principles and dependencies
**Estimated Completion**: 28-32 development days with parallel execution
**Constitutional Gates**: TDD order enforced, performance validation mandatory