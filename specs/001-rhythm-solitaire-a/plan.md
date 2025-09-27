
# Implementation Plan: Rhythm Solitaire

**Branch**: `001-rhythm-solitaire-a` | **Date**: 2025-09-26 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `C:\Users\pdbro\IdeaProjects\MelodyGameProject\specs\001-rhythm-solitaire-a\spec.md`

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
Rhythm Solitaire combines traditional solitaire strategy with rhythm game mechanics, requiring players to move cards in sync with music beats. Players clear solitaire-style layouts while building dynamic soundtracks through their actions, featuring adaptive timing tolerance, star-based progression, and cross-platform deployment with offline-first design.

## Technical Context
**Language/Version**: C# 9.0+ with Unity 6 LTS
**Primary Dependencies**: FMOD Studio 2.02+, Unity Test Framework, Unity Input System
**Storage**: Local file storage for saves, optional cloud sync for progression
**Testing**: Unity Test Framework with specialized timing tests
**Target Platform**: Cross-platform (PC, Nintendo Switch, iOS/Android)
**Project Type**: Unity game - single project with modular architecture
**Performance Goals**: 60fps stable, <20ms audio latency, <10ms audio-video sync
**Constraints**: <20ms audio latency, zero allocation in audio threads, offline-first design
**Scale/Scope**: Single-player rhythm game with multiple game modes and progressive difficulty

## Constitution Check
*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

**I. Audio Quality First**: ✅ PASS
- FMOD Studio 2.02+ provides professional audio processing
- Signal chain fidelity maintained throughout audio pipeline
- Audio latency testing planned in test framework

**II. Real-Time Performance (NON-NEGOTIABLE)**: ✅ PASS
- 60fps target with <10ms audio-video sync tolerance specified
- Zero allocation in audio threads explicitly required
- Frame rate monitoring and audio thread performance testing planned

**III. Test-Driven Development (NON-NEGOTIABLE)**: ✅ PASS
- Unity Test Framework chosen for timing-critical code testing
- Specialized timing tests planned for audio synchronization
- TDD workflow planned for all audio and rhythm components

**IV. Modular Architecture**: ✅ PASS
- Interface-based design with IAudioEngine, ICardGameEngine, etc.
- Loosely coupled components specified in architecture
- Clear responsibility boundaries between audio, game logic, and UI

**V. Accessibility & Usability**: ✅ PASS
- Visual indicators for audio cues specified in FR-007
- Configurable difficulty levels with adaptive timing tolerance
- WCAG 2.1 AA compliance planned for UI components

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
Assets/
├── Scripts/
│   ├── Audio/          # FMOD integration, rhythm engine
│   ├── Cards/          # Solitaire game logic
│   ├── UI/             # Interface components
│   ├── GameModes/      # Campaign, Endless, Challenge
│   ├── Progression/    # Star system, unlocks
│   ├── PowerCards/     # Special abilities
│   └── Core/           # Shared utilities, managers
├── Tests/
│   ├── EditMode/       # Unit tests for game logic
│   ├── PlayMode/       # Integration tests for timing
│   └── Performance/    # Audio latency and frame rate tests
├── Audio/              # FMOD banks and audio assets
├── Prefabs/            # UI and game object prefabs
└── Scenes/             # Game scenes and levels

Packages/               # Unity package dependencies
ProjectSettings/        # Unity project configuration
```

**Structure Decision**: Unity 6 LTS single project structure with modular script organization. Test assemblies separated by type (Edit Mode for logic, Play Mode for integration, Performance for timing validation). Audio assets managed through FMOD Studio integration.

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
- Generate tasks from Phase 1 artifacts: 4 interface contracts, 7 data entities, 7 quickstart scenarios
- Interface contracts → Unity C# interface definition tasks [P]
- Data entities → Unity ScriptableObject/MonoBehaviour model tasks [P]
- Quickstart scenarios → Unity Test Framework integration test tasks
- FMOD integration → Audio engine implementation tasks
- UI components → Unity UI system implementation tasks
- Performance validation → Constitutional compliance testing tasks

**Specific Task Categories**:
1. **Interface Implementation** (4 tasks): IAudioEngine, ICardGameEngine, IPowerCardSystem, IPlayerProgressSystem
2. **Data Model Creation** (7 tasks): GameSession, Card, MusicTrack, PowerCard, PlayerProfile, Level, BeatGrid
3. **Core System Implementation** (8 tasks): Audio synchronization, card game logic, rhythm detection, combo system
4. **UI Component Development** (6 tasks): Beat indicators, card display, score UI, accessibility features
5. **Integration Testing** (7 tasks): Based on quickstart scenarios 1-7
6. **Performance Validation** (4 tasks): Audio latency, frame rate, memory allocation, timing accuracy

**Ordering Strategy**:
- TDD order: Test frameworks first, then interface tests, then implementations
- Dependency order: Core interfaces → Data models → Audio engine → Game logic → UI → Integration tests
- Constitutional compliance: Performance tests throughout development
- Mark [P] for parallel execution where Unity assemblies are independent

**Unity-Specific Considerations**:
- Assembly definition files for modular compilation
- FMOD Studio integration tasks sequenced after audio interface
- Unity Test Framework setup for Edit Mode and Play Mode testing
- Cross-platform build tasks for Nintendo Switch, PC, mobile

**Estimated Output**: 32-36 numbered, dependency-ordered tasks in tasks.md

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
- [ ] Complexity deviations documented

---
*Based on Constitution v1.0.0 - See `/memory/constitution.md`*
