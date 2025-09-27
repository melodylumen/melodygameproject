// Power Card System Interface Contract
// Supports FR-008
// Constitutional compliance: Modular Architecture, Real-Time Performance

using System;
using System.Collections.Generic;

namespace RhythmSolitaire.Contracts
{
    /// <summary>
    /// Power card system interface for special abilities
    /// CONSTITUTIONAL REQUIREMENT: Rhythm-based charging, modular design
    /// </summary>
    public interface IPowerCardSystem
    {
        // FR-008: Power cards that charge based on rhythm combo performance
        /// <summary>
        /// Updates power card charge based on rhythm accuracy
        /// PERFORMANCE: Must complete within <2ms for real-time updates
        /// </summary>
        void UpdateChargeLevel(PowerCardType cardType, TimingAccuracy accuracy, int comboCount);

        /// <summary>
        /// Attempts to activate a power card with timing validation
        /// RHYTHM: Optimal activation on strong beats provides bonuses
        /// </summary>
        PowerCardResult ActivatePowerCard(PowerCardType cardType, DateTime activationTime);

        /// <summary>
        /// Gets current charge levels for all available power cards
        /// UI: Required for visual charge indicators
        /// </summary>
        IReadOnlyDictionary<PowerCardType, float> GetChargeLevels();

        /// <summary>
        /// Checks if a power card is ready for activation
        /// VALIDATION: Includes charge level and cooldown checks
        /// </summary>
        bool IsPowerCardReady(PowerCardType cardType);

        /// <summary>
        /// Resets all power card states for new game session
        /// MEMORY: Must reuse pre-allocated objects
        /// </summary>
        void ResetSession();

        /// <summary>
        /// Configures available power cards based on level progression
        /// PROGRESSION: Some power cards unlock at higher levels
        /// </summary>
        void ConfigureAvailablePowerCards(int playerLevel, LevelConfiguration levelConfig);

        // Events for loose coupling with audio and visual systems
        event Action<PowerCardEvent> OnPowerCardCharged;
        event Action<PowerCardEvent> OnPowerCardActivated;
        event Action<PowerCardEvent> OnPowerCardCooldownComplete;
    }

    public enum PowerCardType
    {
        // Solitaire-focused abilities
        Shuffle,        // Shuffle remaining deck cards
        Reveal,         // Reveal next N hidden cards
        Undo,           // Undo last move
        Hint,           // Highlight best available move

        // Rhythm-focused abilities
        TempoSlow,      // Temporarily slow down beat timing
        ComboExtend,    // Extend combo window duration
        ScoreMultiply,  // Double score for next N moves
        BeatSkip        // Allow one off-beat move without penalty
    }

    public struct PowerCardResult
    {
        public bool WasActivated;
        public PowerCardEffect Effect;
        public float RhythmBonus;          // Bonus for perfect timing
        public DateTime CooldownUntil;
        public string StatusMessage;
    }

    public struct PowerCardEffect
    {
        public PowerCardType Type;
        public int Duration;               // Effect duration in beats or moves
        public float Intensity;            // Effect strength (0.0 - 1.0)
        public Card[] AffectedCards;       // Cards modified by effect
        public bool RequiresAudioSync;     // Whether effect needs audio coordination
    }

    public struct PowerCardEvent
    {
        public PowerCardType Type;
        public float ChargeLevel;
        public DateTime Timestamp;
        public bool IsRhythmSynced;        // Whether event occurred on beat
    }

    // Configuration data structure
    public struct PowerCardConfiguration
    {
        public PowerCardType Type;
        public float MaxChargeLevel;
        public float ChargeRate;           // Charge gained per perfect timing
        public float CooldownSeconds;
        public int UnlockLevel;            // Player level required
        public bool RequiresBeatTiming;    // Whether activation needs rhythm sync
    }
}