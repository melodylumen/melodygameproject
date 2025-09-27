
# Implementation Plan: Rhythm Solitaire

**Branch**: `001-rhythm-solitaire-a` | **Date**: 2025-09-26 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/001-rhythm-solitaire-a/spec.md`

## Execution Flow (/plan command scope)
```
1. Load feature spec from Input path
   → If not found: ERROR "No feature spec at {path}"
2. Fill Technical Context (scan for NEEDS CLARIFICATION)
   → Detect Project Type from file system structure or context (web=frontend+backend, mobile=app+api)
   → Set Structure Decision based on project type
3. Fill the Constitution Check section based on the content of the constitution document.
4. Evaluate Constitution Check section below
   → If violations exist: Document in Complexity Tracking
   → If no justification possible: ERROR "Simplify approach first"
   → Update Progress Tracking: Initial Constitution Check
5. Execute Phase 0 → research.md
   → If NEEDS CLARIFICATION remain: ERROR "Resolve unknowns"
6. Execute Phase 1 → contracts, data-model.md, quickstart.md, agent-specific template file (e.g., `CLAUDE.md` for Claude Code, `.github/copilot-instructions.md` for GitHub Copilot, `GEMINI.md` for Gemini CLI, `QWEN.md` for Qwen Code or `AGENTS.md` for opencode).
7. Re-evaluate Constitution Check section
   → If new violations: Refactor design, return to Phase 1
   → Update Progress Tracking: Post-Design Constitution Check
8. Plan Phase 2 → Describe task generation approach (DO NOT create tasks.md)
9. STOP - Ready for /tasks command
```

**IMPORTANT**: The /plan command STOPS at step 7. Phases 2-4 are executed by other commands:
- Phase 2: /tasks command creates tasks.md
- Phase 3-4: Implementation execution (manual or via tools)

## Summary
Rhythm Solitaire combines traditional solitaire gameplay with rhythm game mechanics, requiring players to move cards in sync with dynamic music. The game features adaptive timing tolerances, star-based progression, multiple game modes, and cross-platform deployment (PC, Nintendo Switch, Mobile). Technical approach leverages game engine architecture for real-time audio processing, modular component design, and offline-first data persistence with optional online sync.

## Technical Context
**Language/Version**: C# 9.0+ (Unity) or GDScript/C# (Godot) - NEEDS CLARIFICATION: specific game engine choice
**Primary Dependencies**: Game Engine (Unity/Godot), Audio processing library, Cross-platform input handling - NEEDS CLARIFICATION
**Storage**: Local file system for save data, optional cloud storage for cross-device sync
**Testing**: Engine-specific testing frameworks, automated timing accuracy tests - NEEDS CLARIFICATION
**Target Platform**: PC (Windows/Mac/Linux), Nintendo Switch, iOS/Android
**Project Type**: Cross-platform game - single codebase with platform-specific builds
**Performance Goals**: 60fps stable, <20ms audio latency, <10ms timing synchronization tolerance
**Constraints**: Memory usage within platform limits (1GB mobile, 4GB desktop), offline-capable core gameplay
**Scale/Scope**: Single-player focused, ~50 levels across 5-10 worlds, 3 game modes, multiple music genres

## Constitution Check
*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

**I. Audio Quality First**: ✅ PASS (Post-Design Verification)
- `IAudioEngine.GetCurrentLatencyMs()` contract enforces <20ms constitutional requirement
- `BeatTimingData` struct provides precise timing with <5ms completion requirement
- Signal quality testing implemented via `VisualBeatData` for validation
- FMOD Studio research confirms professional-grade audio processing capability

**II. Real-Time Performance (NON-NEGOTIABLE)**: ✅ PASS (Post-Design Verification)
- All interface methods specify performance constraints: <1-5ms completion times
- `IAudioEngine.ValidateMoveTiming()` must complete within <1ms for real-time feedback
- Memory allocation explicitly forbidden in contracts: "Must use pre-allocated layer objects only"
- Pre-allocated object pools documented in data-model.md memory management section

**III. Test-Driven Development (NON-NEGOTIABLE)**: ✅ PASS (Post-Design Verification)
- Interface contracts designed for testability with deterministic requirements
- `ICardGameEngine.ExecuteMove()` specifies "Must be deterministic for automated testing"
- Quickstart.md provides comprehensive test scenarios for validation
- Performance validation tools specified: Unity Profiler, FMOD profiler, LatencyMon

**IV. Modular Architecture**: ✅ PASS (Post-Design Verification)
- Four independent interface contracts created: IAudioEngine, ICardGameEngine, IPowerCardSystem, IPlayerProgressSystem
- Each interface has clearly defined boundaries and responsibilities
- Event-based communication prevents tight coupling between modules
- Data model shows clear entity relationships without implementation dependencies

**V. Accessibility & Usability**: ✅ PASS (Post-Design Verification)
- `VisualBeatData` struct provides visual beat indicators for hearing-impaired players
- `AccessibilityPreferences` in data model includes comprehensive accessibility settings
- Quickstart Scenario 5 specifically validates visual-only gameplay
- Adaptive timing tolerance implemented via `accuracyWindows` in BeatGrid entity

## Project Structure

### Documentation (this feature)
```
specs/[###-feature]/
├── plan.md              # This file (/plan command output)
├── research.md          # Phase 0 output (/plan command)
├── data-model.md        # Phase 1 output (/plan command)
├── quickstart.md        # Phase 1 output (/plan command)
├── contracts/           # Phase 1 output (/plan command)
└── tasks.md             # Phase 2 output (/tasks command - NOT created by /plan)
```

### Source Code (repository root)
```
Assets/                          # Game engine assets directory
├── Scripts/
│   ├── Audio/                   # Audio processing and rhythm engine
│   ├── Cards/                   # Card game logic and models
│   ├── UI/                      # User interface components
│   ├── GameModes/               # Campaign, Endless, Challenge modes
│   ├── Progression/             # Star system and level management
│   ├── PowerCards/              # Special abilities and effects
│   └── Core/                    # Shared utilities and managers
├── Scenes/                      # Game scenes and levels
├── Audio/                       # Music tracks and sound effects
├── Art/                         # Visual assets (cards, UI, backgrounds)
├── Data/                        # Level definitions and configuration
└── Prefabs/                     # Reusable game objects

Tests/
├── PlayMode/                    # Integration tests that run in game mode
├── EditMode/                    # Unit tests for editor-time components
├── Performance/                 # Audio latency and timing accuracy tests
└── Accessibility/               # WCAG compliance and visual indicator tests

ProjectSettings/                 # Platform-specific build configurations
└── [engine-specific settings]
```

**Structure Decision**: Cross-platform game engine architecture selected. Game logic organized by domain (Audio, Cards, UI, etc.) with clear separation of concerns. Testing structure supports both unit tests (EditMode) and integration tests (PlayMode) as required by constitutional TDD mandate.

## Phase 0: Outline & Research
1. **Extract unknowns from Technical Context** above:
   - For each NEEDS CLARIFICATION → research task
   - For each dependency → best practices task
   - For each integration → patterns task

2. **Generate and dispatch research agents**:
   ```
   For each unknown in Technical Context:
     Task: "Research {unknown} for {feature context}"
   For each technology choice:
     Task: "Find best practices for {tech} in {domain}"
   ```

3. **Consolidate findings** in `research.md` using format:
   - Decision: [what was chosen]
   - Rationale: [why chosen]
   - Alternatives considered: [what else evaluated]

**Output**: research.md with all NEEDS CLARIFICATION resolved

## Phase 1: Design & Contracts
*Prerequisites: research.md complete*

1. **Extract entities from feature spec** → `data-model.md`:
   - Entity name, fields, relationships
   - Validation rules from requirements
   - State transitions if applicable

2. **Generate API contracts** from functional requirements:
   - For each user action → endpoint
   - Use standard REST/GraphQL patterns
   - Output OpenAPI/GraphQL schema to `/contracts/`

3. **Generate contract tests** from contracts:
   - One test file per endpoint
   - Assert request/response schemas
   - Tests must fail (no implementation yet)

4. **Extract test scenarios** from user stories:
   - Each story → integration test scenario
   - Quickstart test = story validation steps

5. **Update agent file incrementally** (O(1) operation):
   - Run `.specify/scripts/powershell/update-agent-context.ps1 -AgentType claude`
     **IMPORTANT**: Execute it exactly as specified above. Do not add or remove any arguments.
   - If exists: Add only NEW tech from current plan
   - Preserve manual additions between markers
   - Update recent changes (keep last 3)
   - Keep under 150 lines for token efficiency
   - Output to repository root

**Output**: data-model.md, /contracts/*, failing tests, quickstart.md, agent-specific file

## Phase 2: Task Planning Approach
*This section describes what the /tasks command will do - DO NOT execute during /plan*

**Task Generation Strategy**:
- Load `.specify/templates/tasks-template.md` as base template
- Generate tasks from completed Phase 1 design artifacts:
  - `data-model.md`: 7 entities → 7 model creation tasks [P]
  - `contracts/`: 4 interfaces → 4 contract test tasks [P]
  - `quickstart.md`: 7 scenarios → 7 integration test tasks
  - Research findings: Unity/FMOD setup tasks

**Specific Task Categories**:
1. **Setup Tasks** (Sequential):
   - Unity 6 LTS project initialization
   - FMOD Studio integration setup
   - Constitutional compliance tooling setup

2. **Contract Test Tasks** [P] (Parallel):
   - IAudioEngine contract tests
   - ICardGameEngine contract tests
   - IPowerCardSystem contract tests
   - IPlayerProgressSystem contract tests

3. **Model Creation Tasks** [P] (Parallel):
   - GameSession, Card, MusicTrack, PowerCard entities
   - PlayerProfile, Level/World, BeatGrid entities

4. **Implementation Tasks** (TDD Sequential):
   - Audio engine implementation (constitutional memory constraints)
   - Card game engine implementation
   - Power card system implementation
   - Player progress system implementation

5. **Integration Test Tasks**:
   - Quickstart scenario automation (7 scenarios)
   - Performance validation tests (latency, frame rate)
   - Accessibility compliance tests

**Ordering Strategy**:
- **Phase A**: Setup tasks (1-3) - Sequential dependency chain
- **Phase B**: Contract tests + Models (4-11) - [P] parallel execution
- **Phase C**: Core implementations (12-20) - TDD sequential order
- **Phase D**: Integration validation (21-28) - Depends on implementations

**Constitutional Compliance Integration**:
- Each implementation task includes constitutional verification steps
- Performance tasks validate <20ms audio latency and 60fps requirements
- TDD tasks ensure test-first development approach
- Memory management tasks validate zero audio thread allocation

**Estimated Task Count**: 28-32 numbered, ordered tasks

**Dependencies Map**:
- Audio engine → Card game engine → Power card system → Player progress
- All contract tests independent [P]
- All model creation independent [P]
- Integration tests depend on all implementations

**IMPORTANT**: This phase is executed by the /tasks command, NOT by /plan

## Phase 3+: Future Implementation
*These phases are beyond the scope of the /plan command*

**Phase 3**: Task execution (/tasks command creates tasks.md)  
**Phase 4**: Implementation (execute tasks.md following constitutional principles)  
**Phase 5**: Validation (run tests, execute quickstart.md, performance validation)

## Complexity Tracking
*Fill ONLY if Constitution Check has violations that must be justified*

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| [e.g., 4th project] | [current need] | [why 3 projects insufficient] |
| [e.g., Repository pattern] | [specific problem] | [why direct DB access insufficient] |


## Progress Tracking
*This checklist is updated during execution flow*

**Phase Status**:
- [x] Phase 0: Research complete (/plan command)
- [x] Phase 1: Design complete (/plan command)
- [x] Phase 2: Task planning complete (/plan command - describe approach only)
- [ ] Phase 3: Tasks generated (/tasks command)
- [ ] Phase 4: Implementation complete
- [ ] Phase 5: Validation passed

**Gate Status**:
- [x] Initial Constitution Check: PASS
- [x] Post-Design Constitution Check: PASS
- [x] All NEEDS CLARIFICATION resolved
- [x] Complexity deviations documented (none required)

---
*Based on Constitution v1.0.0 - See `/memory/constitution.md`*
