<!--
Sync Impact Report:
Version change: INITIAL → 1.0.0
Initial constitution creation for MelodyGame project
Added sections: All core sections established
Templates requiring updates: ✅ Updated .specify/templates/plan-template.md (constitution version reference)
Follow-up TODOs: None
-->

# MelodyGame Constitution

## Core Principles

### I. Audio Quality First
Audio processing MUST maintain highest fidelity throughout the signal chain. All audio components MUST be tested for latency, distortion, and quality degradation. Real-time audio processing MUST prioritize consistency over complexity.

**Rationale**: Music games depend on precise audio reproduction for gameplay accuracy and user experience.

### II. Real-Time Performance (NON-NEGOTIABLE)
Game loops MUST maintain consistent frame rates with audio-video synchronization within 10ms tolerance. Audio processing MUST complete within allocated time slices without blocking. Memory allocation in audio threads is FORBIDDEN.

**Rationale**: Timing precision is fundamental to music game mechanics and user experience.

### III. Test-Driven Development (NON-NEGOTIABLE)
TDD mandatory: Tests written → Requirements validated → Tests fail → Implementation begins. All timing-critical code MUST have automated timing tests. Audio components MUST have signal quality tests.

**Rationale**: Music games require precise behavior that cannot be verified through manual testing alone.

### IV. Modular Architecture
Game components MUST be independently testable and loosely coupled. Audio engine, game logic, and UI MUST communicate through well-defined interfaces. Each module MUST have clear responsibility boundaries.

**Rationale**: Enables parallel development, easier testing, and component reusability across different game modes.

### V. Accessibility & Usability
MUST support visual indicators for audio cues. MUST provide configurable difficulty levels and control schemes. UI MUST follow accessibility guidelines (WCAG 2.1 AA minimum). Audio MUST support hearing accessibility features.

**Rationale**: Music games should be enjoyable and accessible to players with varying abilities and preferences.

## Performance Standards

Audio latency MUST NOT exceed 20ms from input to output. Game MUST maintain 60fps minimum on target platforms. Memory usage MUST stay within platform constraints (1GB for mobile, 4GB for desktop). Audio dropouts or stutters are considered critical bugs.

## Development Workflow

Code reviews MUST verify audio performance impact and accessibility compliance. All audio-related changes MUST include listening tests. Performance regression tests MUST pass before merge. Documentation MUST include audio setup instructions and troubleshooting guides.

## Governance

Constitution supersedes all other development practices. Amendments require documentation of impact, approval from project maintainers, and migration plan for existing code. All PRs MUST verify compliance with relevant principles. Complexity MUST be justified against simpler alternatives.

**Version**: 1.0.0 | **Ratified**: 2025-09-26 | **Last Amended**: 2025-09-26