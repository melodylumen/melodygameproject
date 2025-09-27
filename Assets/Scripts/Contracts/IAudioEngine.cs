// Audio Engine Interface Contract
// Supports FR-002, FR-003, FR-004, FR-006, FR-007
// Constitutional compliance: Audio Quality First, Real-Time Performance

using System;
using UnityEngine;

namespace RhythmSolitaire.Contracts
{
    /// <summary>
    /// Audio engine interface for rhythm synchronization and dynamic music
    /// CONSTITUTIONAL REQUIREMENT: <20ms latency, no memory allocation in audio threads
    /// </summary>
    public interface IAudioEngine
    {
        // FR-002: Visual beat indicator support
        /// <summary>
        /// Gets the current beat timing information for visual indicators
        /// TIMING: Must complete within <5ms
        /// </summary>
        BeatTimingData GetCurrentBeatTiming();

        // FR-003: Track player move timing
        /// <summary>
        /// Validates move timing against current beat
        /// TIMING: Must complete within <1ms for real-time feedback
        /// </summary>
        TimingAccuracy ValidateMoveTiming(DateTime moveTime);

        // FR-004: Dynamic music building and layering
        /// <summary>
        /// Adds or removes music layers based on player performance
        /// MEMORY: Must use pre-allocated layer objects only
        /// </summary>
        void UpdateMusicLayers(int comboCount, PowerCardType activePower);

        // FR-006: Multiple music genres
        /// <summary>
        /// Loads and initializes a music track for the session
        /// TIMING: Must complete initialization within 100ms
        /// </summary>
        bool LoadMusicTrack(MusicGenre genre, DifficultyLevel difficulty);

        // FR-007: Visual indicators for accessibility
        /// <summary>
        /// Provides visual beat data for hearing-impaired players
        /// </summary>
        VisualBeatData GetVisualBeatIndicators();

        // Constitutional real-time performance requirement
        /// <summary>
        /// Gets current audio latency measurement
        /// REQUIREMENT: Must return <20ms for constitutional compliance
        /// </summary>
        float GetCurrentLatencyMs();

        // Event notifications (fire-and-forget for performance)
        event Action<BeatEvent> OnBeatTrigger;
        event Action<ComboEvent> OnComboMilestone;
        event Action<LatencyWarning> OnLatencyExceeded;
    }

    public struct BeatTimingData
    {
        public double CurrentBeatTime;
        public double NextBeatTime;
        public double BeatDuration;
        public float Tempo;
        public int BeatIndex;
    }

    public struct VisualBeatData
    {
        public float BeatIntensity;
        public float NextBeatProgress;
        public bool IsStrongBeat;
        public Color BeatColor;
    }

    public enum TimingAccuracy
    {
        Perfect,    // Within perfect timing window
        Good,       // Within good timing window
        Missed      // Outside timing windows
    }

    public enum MusicGenre
    {
        LoFi,
        EDM,
        Orchestral,
        Electronic,
        Ambient
    }

    public enum DifficultyLevel
    {
        Easy,
        Normal,
        Hard,
        Expert
    }

    public class BeatEvent
    {
        public int BeatIndex;
        public double BeatTime;
        public bool IsStrongBeat;
    }

    public class ComboEvent
    {
        public int ComboCount;
        public int LayersActivated;
        public double Timestamp;
    }

    public class LatencyWarning
    {
        public float LatencyMs;
        public double Timestamp;
    }
}