# Claude Code Context: MelodyGame Project

## Project Overview
**MelodyGame** is a Unity-based rhythm game framework with modular architecture and cross-platform deployment. Current feature: Rhythm Solitaire - a hybrid game combining solitaire strategy with rhythm game mechanics.

## Constitutional Principles (CRITICAL)
1. **Audio Quality First**: <20ms latency, signal chain fidelity, quality testing
2. **Real-Time Performance**: 60fps stable, <10ms audio-video sync, NO allocation in audio threads
3. **Test-Driven Development**: TDD mandatory for timing-critical code
4. **Modular Architecture**: Loosely coupled, well-defined interfaces
5. **Accessibility**: Visual indicators, WCAG 2.1 AA compliance

## Current Technology Stack
- **Engine**: Unity 6 LTS (selected for Nintendo Switch support)
- **Audio**: FMOD Studio 2.02+ with custom beat synchronization
- **Testing**: Unity Test Framework with specialized timing tests
- **Platform**: Cross-platform (PC, Nintendo Switch, iOS/Android)
- **Language**: C# 9.0+ with constitutional memory management

## Architecture Overview
```
Assets/Scripts/
├── Audio/          # FMOD integration, rhythm engine
├── Cards/          # Solitaire game logic
├── UI/             # Interface components
├── GameModes/      # Campaign, Endless, Challenge
├── Progression/    # Star system, unlocks
├── PowerCards/     # Special abilities
└── Core/           # Shared utilities, managers
```

## Key Interfaces (Phase 1 Complete)
- `IAudioEngine`: Real-time audio processing, <20ms latency requirement
- `ICardGameEngine`: Solitaire mechanics, score calculation
- `IPowerCardSystem`: Rhythm-based special abilities
- `IPlayerProgressSystem`: Offline-first progression, optional cloud sync

## Current Feature: Rhythm Solitaire (001-rhythm-solitaire-a)
**Status**: Phase 1 design complete, ready for tasks generation

**Core Mechanics**:
- Solitaire card layouts with rhythm timing requirements
- Dynamic music layering based on player performance
- Combo system with power card charging
- Adaptive timing tolerance (difficulty-based)
- Star-based progression (1-3 stars per level)

**Performance Targets**:
- 60fps stable gameplay
- <20ms audio latency
- <10ms timing synchronization tolerance
- Offline-first with optional online sync

## Development Workflow
1. **TDD Required**: Write tests before implementation
2. **Constitutional Compliance**: Verify against all 5 principles
3. **Modular Design**: Independent, testable components
4. **Performance Validation**: Audio latency and frame rate testing

## Memory Management (CRITICAL)
- **Audio Thread**: Zero allocation during execution
- **Game Objects**: Pre-allocated object pools
- **Temporary Data**: Stack-based allocation only
- **Constitutional Violation**: Any audio thread allocation

## Testing Strategy
- **Edit Mode**: Unit tests for game logic
- **Play Mode**: Integration tests for timing accuracy
- **Performance**: Audio latency and frame rate validation
- **Accessibility**: WCAG compliance testing

## Recent Changes (Last 3)
1. **2025-09-26**: Created constitutional framework for MelodyGame
2. **2025-09-26**: Completed Rhythm Solitaire specification with clarifications
3. **2025-09-26**: Finished Phase 1 design with contracts and data model

## Key Files
- `.specify/memory/constitution.md`: Project constitutional principles
- `specs/001-rhythm-solitaire-a/spec.md`: Feature specification
- `specs/001-rhythm-solitaire-a/research.md`: Technology research findings
- `specs/001-rhythm-solitaire-a/contracts/`: Interface definitions
- `specs/001-rhythm-solitaire-a/data-model.md`: Entity relationships

## Build and Test Commands
```bash
# Unity build (check project for specific commands)
# Run timing accuracy tests
# Audio latency validation
# Constitutional compliance check
```

## Important Notes
- **Nintendo Switch**: Requires Unity Pro licensing for commercial deployment
- **Audio Latency**: Use AudioSettings.dspTime for sample-accurate timing
- **Cross-Platform**: Single codebase with platform-specific optimizations
- **Offline-First**: Core gameplay must work without internet connection

## Next Phase
Ready for **Phase 2**: Task generation using `/tasks` command to create detailed implementation tasks based on completed design artifacts.

---
**Constitutional Version**: 1.0.0 | **Last Updated**: 2025-09-26
**Agent Compatibility**: Claude Code | **Token Budget**: <150 lines maintained
