# Research Findings: Rhythm Solitaire

**Date**: 2025-09-26
**Scope**: Technology stack research for cross-platform rhythm game development

## Game Engine Selection

**Decision**: Unity 6 LTS
**Rationale**: Nintendo Switch requires official engine support for commercial deployment. Unity provides battle-tested console development tools, mature audio middleware integration (FMOD/Wwise), and extensive rhythm game community resources. While Unity Pro licensing costs ~$2,040/year, this investment is justified by reduced development time and lower deployment risk.
**Alternatives considered**:
- Godot 4.x: Free licensing and excellent PC/mobile support, but requires third-party solutions for Nintendo Switch (W4 Games commercial port at $68+/month) and limited rhythm game ecosystem
- Custom engine: Maximum control but prohibitive development time for cross-platform deployment

## Audio Processing Architecture

**Decision**: FMOD Studio with custom beat synchronization layer
**Rationale**: FMOD achieves <10ms latency with proper DSP configuration, supports real-time beat callbacks, provides dynamic music layering, and maintains cross-platform consistency. Constitutional requirement for zero memory allocation in audio threads is satisfied through pre-allocated memory pools and sample-accurate timing via `AudioSettings.dspTime`.
**Alternatives considered**:
- Wwise: Professional-grade but higher licensing costs and steeper learning curve
- Custom real-time audio engine: Maximum control and guaranteed constitutional compliance, but significantly longer development time
- Unity Audio System: Built-in convenience but limited real-time capabilities for precise rhythm synchronization

## Testing Framework Strategy

**Decision**: Unity Test Framework (UTF) with specialized timing accuracy tests
**Rationale**: Native Unity integration supports constitutional TDD mandate through NUnit-based edit mode and play mode testing. Automated beat synchronization tests ensure <10ms timing tolerance using AudioSettings.dspTime reference. Performance testing validates 60fps consistency and <20ms audio latency across platforms.
**Alternatives considered**:
- GameDriver: Advanced cross-platform testing but unnecessary complexity for game engine native testing
- Manual testing only: Insufficient for constitutional timing precision requirements
- Third-party frameworks: Additional dependencies without significant benefit over UTF

## Cross-Platform Deployment Strategy

**Decision**: Unity 6 build pipeline with platform-specific optimizations
**Rationale**: Single codebase with platform-specific builds reduces maintenance overhead while meeting constitutional performance requirements. Nintendo Switch deployment through Unity Pro licensing, mobile optimization through Unity's profiling tools, and PC deployment with advanced audio middleware support.
**Alternatives considered**:
- Multi-engine approach: Separate Unity (console) and Godot (PC/mobile) deployments would fragment development and increase testing complexity
- Platform-native development: Individual platform development would multiply effort and violate modular architecture principles

## Memory Management Architecture

**Decision**: Pre-allocated object pools with lock-free inter-thread communication
**Rationale**: Constitutional requirement forbids memory allocation in audio threads. Solution uses pre-allocated memory pools (1MB audio thread pool), lock-free ring buffers for game-to-audio thread communication, and stack-based allocators for temporary audio objects.
**Alternatives considered**:
- Garbage collection: Violates constitutional real-time performance requirements
- Dynamic allocation: Creates non-deterministic latency spikes incompatible with rhythm timing
- Static allocation only: Too restrictive for dynamic music layering requirements

## Development Dependencies

**Core Stack**:
- Unity 6 LTS (C# 9.0+)
- FMOD Studio 2.02+
- Unity Test Framework 1.1.33+
- Visual Studio 2022 or JetBrains Rider

**Platform SDKs**:
- Nintendo Switch SDK (Unity Pro required)
- iOS/Android mobile SDKs
- Windows/Mac/Linux deployment tools

**Audio Tools**:
- FMOD Studio for interactive music composition
- LatencyMon/RTL Utility for audio latency validation
- Superpowered Latency Test for mobile optimization

## Performance Targets Validation

**Confirmed Achievable**:
- 60fps stable gameplay: ✅ Unity optimization tools + constitutional modular architecture
- <20ms audio latency: ✅ FMOD DSP buffer configuration (128 samples = ~2.9ms callbacks)
- <10ms timing synchronization: ✅ AudioSettings.dspTime + pre-allocated memory management
- Cross-platform consistency: ✅ Unity build pipeline + FMOD audio middleware

**Risk Mitigation**:
- Mobile performance: Unity Profiler + platform-specific optimizations
- Nintendo Switch constraints: Official SDK + hardware-specific audio optimizations
- Memory pressure: Constitutional pre-allocation strategy + object pooling

## Constitutional Compliance Summary

All selected technologies directly support constitutional principles:

- **Audio Quality First**: FMOD Studio professional-grade audio processing
- **Real-Time Performance**: Pre-allocated memory + lock-free communication
- **Test-Driven Development**: Unity Test Framework with automated timing tests
- **Modular Architecture**: Unity component system + FMOD audio separation
- **Accessibility**: Visual indicators + WCAG 2.1 AA compliance testing

## Implementation Readiness

**NEEDS CLARIFICATION resolved**: All major technology choices finalized
**Next Phase**: Technical design and contract generation ready to proceed
**Risk Level**: Low - proven technology stack with constitutional alignment