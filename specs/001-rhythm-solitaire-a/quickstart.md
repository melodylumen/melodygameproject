# Quickstart Guide: Rhythm Solitaire

**Feature**: 001-rhythm-solitaire-a
**Purpose**: Validate implementation against user scenarios
**Source**: [spec.md](./spec.md) user scenarios and acceptance criteria

## Test Scenarios Overview

This quickstart guide provides step-by-step validation scenarios derived from the feature specification. Each scenario validates specific functional requirements and ensures the implementation meets user expectations.

## Prerequisites

Before running these scenarios:

1. ✅ Unity 6 LTS project setup complete
2. ✅ FMOD Studio integration configured
3. ✅ All interface contracts implemented
4. ✅ Audio latency <20ms verified
5. ✅ Visual beat indicators functional
6. ✅ Card game engine operational

## Scenario 1: Basic Rhythm Gameplay

**Goal**: Validate core rhythm-solitaire mechanics (FR-001, FR-002, FR-003)

### Steps:
1. **Launch** the game and select "Tutorial" mode
2. **Observe** visual beat indicator showing rhythm timing
3. **Wait** for the beat indicator to show the next beat approaching
4. **Move** a valid card exactly on the beat (when indicator shows perfect timing)
5. **Verify** the following immediate feedback:
   - ✅ Card move succeeds
   - ✅ Visual feedback confirms perfect timing
   - ✅ Audio confirmation plays
   - ✅ Combo counter increases to 1
   - ✅ Score increases with timing bonus

### Expected Outcome:
Player successfully moves card with rhythm timing, receives positive feedback, and understands the basic rhythm-solitaire connection.

**Validates**: FR-001 (card layouts), FR-002 (beat indicator), FR-003 (timing tracking)

## Scenario 2: Combo Chain Building

**Goal**: Validate combo mechanics and power card charging (FR-008, FR-004)

### Steps:
1. **Continue** from Scenario 1
2. **Execute** 9 more perfect-timed card moves in sequence
3. **Observe** combo counter reaching 10+
4. **Complete** a foundation pile (place all 4 cards of same suit)
5. **Verify** the following chain reaction:
   - ✅ Musical "drop" occurs (dramatic music change)
   - ✅ Visual effects sync with the music
   - ✅ Power card charges up (visual indicator shows charge)
   - ✅ Music layers become richer and more complex
   - ✅ Score multiplier increases significantly

### Expected Outcome:
Player experiences the satisfying combination of strategic play and musical reward that defines the game's core appeal.

**Validates**: FR-004 (dynamic music), FR-008 (power card charging), audio-visual synchronization

## Scenario 3: Off-Beat Move Handling

**Goal**: Validate timing tolerance and combo breaking (FR-003, FR-011)

### Steps:
1. **Build** a combo of 5+ moves using perfect timing
2. **Intentionally** move a card off-beat (between beat indicators)
3. **Verify** the following behavior:
   - ✅ Card move still succeeds (valid solitaire move)
   - ✅ Combo counter resets to 0
   - ✅ Score multiplier resets to base level
   - ✅ Visual feedback indicates timing was off
   - ✅ Music layers remain but no new layering occurs
   - ✅ No power card charge gained

### Expected Outcome:
Player learns that strategic moves are always allowed, but rhythm timing provides significant bonuses.

**Validates**: FR-003 (timing validation), FR-011 (adaptive tolerance), combo mechanics

## Scenario 4: Power Card Activation

**Goal**: Validate power card timing and effects (FR-008)

### Steps:
1. **Charge** a power card to 100% through perfect timing moves
2. **Wait** for a strong beat (usually every 4th beat)
3. **Activate** the power card exactly on the strong beat
4. **Verify** the following synchronized effects:
   - ✅ Power card activates successfully
   - ✅ Special visual effects sync with music
   - ✅ Strategic advantage is provided (cards revealed, shuffled, etc.)
   - ✅ Audio effect perfectly aligns with musical beat
   - ✅ Rhythm bonus applied to activation
   - ✅ Cooldown timer starts

### Expected Outcome:
Player experiences the satisfaction of perfectly timed power activation and understands the rhythm strategy connection.

**Validates**: FR-008 (power card system), audio-visual synchronization, rhythm-strategy integration

## Scenario 5: Accessibility Features

**Goal**: Validate visual indicators for hearing-impaired players (FR-007)

### Steps:
1. **Navigate** to Settings menu
2. **Enable** "Visual Beat Indicators" accessibility option
3. **Disable** game audio completely
4. **Start** a new game level
5. **Play** using only visual cues for timing
6. **Verify** the following visual feedback:
   - ✅ Beat timing clearly visible without audio
   - ✅ Strong beats visually distinguished
   - ✅ Perfect timing windows clearly indicated
   - ✅ Move timing feedback provided visually
   - ✅ Combo and score feedback visible
   - ✅ Power card charge status clear

### Expected Outcome:
Player can successfully play and enjoy the game using only visual cues, ensuring accessibility compliance.

**Validates**: FR-007 (visual indicators), accessibility requirements (WCAG 2.1 AA)

## Scenario 6: Pause and Resume

**Goal**: Validate combo handling during pause (Edge Case from spec)

### Steps:
1. **Build** a combo of 8+ moves
2. **Pause** the game mid-combo
3. **Wait** 10 seconds in pause state
4. **Resume** the game
5. **Verify** the following behavior:
   - ✅ Combo counter reset to 0
   - ✅ Score accumulated before pause is preserved
   - ✅ Music resumes from appropriate position
   - ✅ Card layout exactly as it was before pause
   - ✅ Power card charges preserved
   - ✅ Beat timing restarts cleanly

### Expected Outcome:
Player understands that pausing breaks the flow but doesn't penalize overall progress, balancing accessibility with rhythm game integrity.

**Validates**: Edge case handling, combo mechanics, session state management

## Scenario 7: Rapid Move Handling

**Goal**: Validate rapid successive moves beyond beat timing (Edge Case from spec)

### Steps:
1. **Start** a new level
2. **Attempt** to make 3 card moves in rapid succession (faster than beat timing allows)
3. **Verify** the following behavior:
   - ✅ All valid solitaire moves are allowed
   - ✅ Combo counter resets after first off-beat move
   - ✅ Rhythm scoring stops for rapid moves
   - ✅ Base solitaire scoring continues
   - ✅ Music layers don't advance
   - ✅ Player can return to rhythm timing afterward

### Expected Outcome:
Player learns they can always play pure solitaire strategy but miss out on rhythm bonuses, providing strategic choice.

**Validates**: Edge case handling, rhythm vs. strategy balance, player choice

## Performance Validation

### Constitutional Requirements Check:

1. **Audio Latency**: Measure and verify <20ms from input to audio output
2. **Frame Rate**: Confirm stable 60fps during all scenarios
3. **Timing Accuracy**: Verify <10ms synchronization between audio and visual beats
4. **Memory**: Confirm no memory allocation during audio thread execution

### Tools for Performance Validation:
- Unity Profiler for frame rate monitoring
- FMOD profiler for audio latency measurement
- LatencyMon for system-level audio validation
- Custom timing accuracy tests

## Completion Criteria

All scenarios must pass with the following success rates:

- ✅ **100%** - Basic functionality (card moves, beat detection)
- ✅ **100%** - Accessibility features (visual indicators work)
- ✅ **95%** - Timing accuracy (within constitutional requirements)
- ✅ **100%** - Edge case handling (pause, rapid moves)
- ✅ **90%** - Audio-visual synchronization quality

## Troubleshooting Common Issues

### Beat Timing Feels Off
- Check audio driver latency settings
- Verify FMOD buffer configuration
- Test with different audio devices
- Calibrate timing offset in settings

### Visual Indicators Not Clear
- Increase contrast in accessibility settings
- Verify proper color coding for beat strength
- Check animation timing matches audio

### Performance Issues
- Profile audio thread allocation
- Verify 60fps stability
- Check memory pool sizing
- Validate constitutional compliance

---

**Next Steps**: After successful quickstart validation, proceed to full automated test suite execution and performance benchmarking.