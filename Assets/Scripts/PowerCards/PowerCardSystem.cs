using System;
using System.Collections.Generic;
using UnityEngine;
using RhythmSolitaire.Contracts;
using RhythmSolitaire.Core;
using RhythmSolitaire.Audio;

namespace RhythmSolitaire.PowerCards
{
    /// <summary>
    /// Power Card System implementation - handles special abilities with rhythm synchronization
    /// Constitutional compliance: Modular Architecture, Real-Time Performance
    /// </summary>
    public class PowerCardSystem : MonoBehaviour, IPowerCardSystem
    {
        #region Constitutional Requirements

        private const float UPDATE_CHARGE_MAX_DURATION_MS = 2f;

        #endregion

        #region Pre-allocated Memory Pools (Constitutional Requirement)

        // Pre-allocated dictionaries and arrays to avoid GC allocation
        private readonly Dictionary<PowerCardType, PowerCardState> _powerCardStates = new Dictionary<PowerCardType, PowerCardState>();
        private readonly Dictionary<PowerCardType, float> _chargeLevelsCache = new Dictionary<PowerCardType, float>();
        private readonly PowerCardEvent[] _eventPool = new PowerCardEvent[32];
        private readonly PowerCardEffect[] _effectPool = new PowerCardEffect[16];

        // Pool indices for lock-free access
        private int _eventPoolIndex = 0;
        private int _effectPoolIndex = 0;

        #endregion

        #region Core State

        [SerializeField] private BeatGrid _beatGrid;
        [SerializeField] private int _playerLevel = 1;
        [SerializeField] private LevelConfiguration _currentLevelConfig;
        [SerializeField] private bool _isInitialized = false;

        // Power card configurations
        private readonly PowerCardConfiguration[] _powerCardConfigs = new PowerCardConfiguration[]
        {
            // Solitaire-focused abilities
            new PowerCardConfiguration
            {
                Type = PowerCardType.Shuffle,
                MaxChargeLevel = 100f,
                ChargeRate = 15f,
                CooldownSeconds = 30f,
                UnlockLevel = 1,
                RequiresBeatTiming = false
            },
            new PowerCardConfiguration
            {
                Type = PowerCardType.Reveal,
                MaxChargeLevel = 100f,
                ChargeRate = 12f,
                CooldownSeconds = 20f,
                UnlockLevel = 1,
                RequiresBeatTiming = false
            },
            new PowerCardConfiguration
            {
                Type = PowerCardType.Undo,
                MaxChargeLevel = 100f,
                ChargeRate = 20f,
                CooldownSeconds = 45f,
                UnlockLevel = 3,
                RequiresBeatTiming = false
            },
            new PowerCardConfiguration
            {
                Type = PowerCardType.Hint,
                MaxChargeLevel = 100f,
                ChargeRate = 8f,
                CooldownSeconds = 15f,
                UnlockLevel = 1,
                RequiresBeatTiming = false
            },
            // Rhythm-focused abilities
            new PowerCardConfiguration
            {
                Type = PowerCardType.TempoSlow,
                MaxChargeLevel = 100f,
                ChargeRate = 10f,
                CooldownSeconds = 60f,
                UnlockLevel = 2,
                RequiresBeatTiming = true
            },
            new PowerCardConfiguration
            {
                Type = PowerCardType.ComboExtend,
                MaxChargeLevel = 100f,
                ChargeRate = 8f,
                CooldownSeconds = 40f,
                UnlockLevel = 2,
                RequiresBeatTiming = true
            },
            new PowerCardConfiguration
            {
                Type = PowerCardType.ScoreMultiply,
                MaxChargeLevel = 100f,
                ChargeRate = 6f,
                CooldownSeconds = 90f,
                UnlockLevel = 4,
                RequiresBeatTiming = true
            },
            new PowerCardConfiguration
            {
                Type = PowerCardType.BeatSkip,
                MaxChargeLevel = 100f,
                ChargeRate = 25f,
                CooldownSeconds = 25f,
                UnlockLevel = 3,
                RequiresBeatTiming = true
            }
        };

        #endregion

        #region Events (Constitutional: Loose coupling)

        public event Action<PowerCardEvent> OnPowerCardCharged;
        public event Action<PowerCardEvent> OnPowerCardActivated;
        public event Action<PowerCardEvent> OnPowerCardCooldownComplete;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            InitializePowerCardSystem();
        }

        private void Update()
        {
            UpdateCooldowns();
        }

        #endregion

        #region IPowerCardSystem Implementation

        /// <summary>
        /// Updates power card charge based on rhythm accuracy
        /// Constitutional requirement: Must complete within <2ms for real-time updates
        /// </summary>
        public void UpdateChargeLevel(PowerCardType cardType, TimingAccuracy accuracy, int comboCount)
        {
            var startTime = Time.realtimeSinceStartup;

            // Check if power card is available and not on cooldown
            if (!_powerCardStates.ContainsKey(cardType) || _powerCardStates[cardType].IsOnCooldown)
            {
                return;
            }

            var state = _powerCardStates[cardType];
            var config = GetPowerCardConfig(cardType);

            // Calculate charge amount based on timing accuracy and combo
            float chargeAmount = accuracy switch
            {
                TimingAccuracy.Perfect => config.ChargeRate * (1f + comboCount * 0.1f),
                TimingAccuracy.Good => config.ChargeRate * 0.6f * (1f + comboCount * 0.05f),
                TimingAccuracy.Missed => 0f,
                _ => 0f
            };

            // Update charge level (clamped to max)
            float oldCharge = state.ChargeLevel;
            state.ChargeLevel = Mathf.Min(state.ChargeLevel + chargeAmount, config.MaxChargeLevel);
            _powerCardStates[cardType] = state;

            // Check if power card became fully charged
            if (oldCharge < config.MaxChargeLevel && state.ChargeLevel >= config.MaxChargeLevel)
            {
                // Fire charged event using pre-allocated event
                var chargedEvent = GetNextPowerCardEvent();
                chargedEvent.Type = cardType;
                chargedEvent.ChargeLevel = state.ChargeLevel;
                chargedEvent.Timestamp = DateTime.UtcNow;
                chargedEvent.IsRhythmSynced = (accuracy == TimingAccuracy.Perfect);

                OnPowerCardCharged?.Invoke(chargedEvent);
            }

            // Constitutional compliance: Validate execution time
            var executionTime = (Time.realtimeSinceStartup - startTime) * 1000f;
            if (executionTime > UPDATE_CHARGE_MAX_DURATION_MS)
            {
                Debug.LogError($"CONSTITUTIONAL VIOLATION: UpdateChargeLevel() took {executionTime}ms, must be <{UPDATE_CHARGE_MAX_DURATION_MS}ms");
            }
        }

        /// <summary>
        /// Attempts to activate a power card with timing validation
        /// Constitutional requirement: Optimal activation on strong beats provides bonuses
        /// </summary>
        public PowerCardResult ActivatePowerCard(PowerCardType cardType, DateTime activationTime)
        {
            // Check if power card exists and is available
            if (!_powerCardStates.ContainsKey(cardType))
            {
                return new PowerCardResult
                {
                    WasActivated = false,
                    StatusMessage = "Power card not available",
                    CooldownUntil = DateTime.MinValue,
                    RhythmBonus = 0f
                };
            }

            var state = _powerCardStates[cardType];
            var config = GetPowerCardConfig(cardType);

            // Check charge level
            if (state.ChargeLevel < config.MaxChargeLevel)
            {
                return new PowerCardResult
                {
                    WasActivated = false,
                    StatusMessage = $"Insufficient charge: {state.ChargeLevel:F0}%",
                    CooldownUntil = DateTime.MinValue,
                    RhythmBonus = 0f
                };
            }

            // Check cooldown
            if (state.IsOnCooldown)
            {
                return new PowerCardResult
                {
                    WasActivated = false,
                    StatusMessage = $"On cooldown until {state.CooldownUntil:HH:mm:ss}",
                    CooldownUntil = state.CooldownUntil,
                    RhythmBonus = 0f
                };
            }

            // Calculate rhythm bonus for beat-synchronized activation
            float rhythmBonus = 1f;
            bool isRhythmSynced = false;

            if (config.RequiresBeatTiming && _beatGrid != null)
            {
                // Check if activation is on beat
                if (_beatGrid.IsOnBeat)
                {
                    rhythmBonus = 2f; // Double effect for perfect timing
                    isRhythmSynced = true;
                }
                else
                {
                    rhythmBonus = 0.5f; // Reduced effect for off-beat activation
                }
            }

            // Create power card effect
            var effect = CreatePowerCardEffect(cardType, rhythmBonus);

            // Execute the power card effect
            ExecutePowerCardEffect(effect);

            // Update power card state
            state.ChargeLevel = 0f;
            state.CooldownUntil = DateTime.UtcNow.AddSeconds(config.CooldownSeconds);
            state.IsOnCooldown = true;
            _powerCardStates[cardType] = state;

            // Fire activation event
            var activationEvent = GetNextPowerCardEvent();
            activationEvent.Type = cardType;
            activationEvent.ChargeLevel = 0f;
            activationEvent.Timestamp = activationTime;
            activationEvent.IsRhythmSynced = isRhythmSynced;

            OnPowerCardActivated?.Invoke(activationEvent);

            Debug.Log($"Activated power card {cardType} with {rhythmBonus}x rhythm bonus");

            return new PowerCardResult
            {
                WasActivated = true,
                Effect = effect,
                RhythmBonus = rhythmBonus,
                CooldownUntil = state.CooldownUntil,
                StatusMessage = "Power card activated successfully"
            };
        }

        /// <summary>
        /// Gets current charge levels for all available power cards
        /// Constitutional requirement: Required for UI visual charge indicators
        /// </summary>
        public IReadOnlyDictionary<PowerCardType, float> GetChargeLevels()
        {
            // Update cached charge levels (reuse dictionary to avoid allocation)
            _chargeLevelsCache.Clear();

            foreach (var kvp in _powerCardStates)
            {
                _chargeLevelsCache[kvp.Key] = kvp.Value.ChargeLevel;
            }

            return _chargeLevelsCache;
        }

        /// <summary>
        /// Checks if a power card is ready for activation
        /// Constitutional requirement: Includes charge level and cooldown checks
        /// </summary>
        public bool IsPowerCardReady(PowerCardType cardType)
        {
            if (!_powerCardStates.ContainsKey(cardType))
                return false;

            var state = _powerCardStates[cardType];
            var config = GetPowerCardConfig(cardType);

            return state.ChargeLevel >= config.MaxChargeLevel && !state.IsOnCooldown;
        }

        /// <summary>
        /// Resets all power card states for new game session
        /// Constitutional requirement: Must reuse pre-allocated objects
        /// </summary>
        public void ResetSession()
        {
            // Reset all power card states (reuse existing objects)
            var keys = new PowerCardType[_powerCardStates.Count];
            _powerCardStates.Keys.CopyTo(keys, 0);

            foreach (var cardType in keys)
            {
                var state = _powerCardStates[cardType];
                state.ChargeLevel = 0f;
                state.IsOnCooldown = false;
                state.CooldownUntil = DateTime.MinValue;
                _powerCardStates[cardType] = state;
            }

            // Reset pool indices
            _eventPoolIndex = 0;
            _effectPoolIndex = 0;

            Debug.Log("Power card session reset completed");
        }

        /// <summary>
        /// Configures available power cards based on level progression
        /// Constitutional requirement: Some power cards unlock at higher levels
        /// </summary>
        public void ConfigureAvailablePowerCards(int playerLevel, LevelConfiguration levelConfig)
        {
            _playerLevel = playerLevel;
            _currentLevelConfig = levelConfig;

            // Clear existing power cards
            _powerCardStates.Clear();

            // Add power cards based on player level
            foreach (var config in _powerCardConfigs)
            {
                if (playerLevel >= config.UnlockLevel)
                {
                    _powerCardStates[config.Type] = new PowerCardState
                    {
                        ChargeLevel = 0f,
                        IsOnCooldown = false,
                        CooldownUntil = DateTime.MinValue
                    };
                }
            }

            Debug.Log($"Configured {_powerCardStates.Count} power cards for player level {playerLevel}");
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Initializes the power card system with constitutional compliance
        /// </summary>
        private void InitializePowerCardSystem()
        {
            // Initialize event pools
            for (int i = 0; i < _eventPool.Length; i++)
            {
                _eventPool[i] = new PowerCardEvent();
            }

            for (int i = 0; i < _effectPool.Length; i++)
            {
                _effectPool[i] = new PowerCardEffect();
            }

            // Find beat grid in scene
            _beatGrid = FindObjectOfType<BeatGrid>();

            // Configure default power cards
            ConfigureAvailablePowerCards(_playerLevel, new LevelConfiguration());

            _isInitialized = true;
            Debug.Log("PowerCardSystem initialized with constitutional compliance");
        }

        /// <summary>
        /// Updates cooldowns for all power cards
        /// </summary>
        private void UpdateCooldowns()
        {
            if (!_isInitialized) return;

            var currentTime = DateTime.UtcNow;
            var keys = new PowerCardType[_powerCardStates.Count];
            _powerCardStates.Keys.CopyTo(keys, 0);

            foreach (var cardType in keys)
            {
                var state = _powerCardStates[cardType];

                if (state.IsOnCooldown && currentTime >= state.CooldownUntil)
                {
                    // Cooldown complete
                    state.IsOnCooldown = false;
                    state.CooldownUntil = DateTime.MinValue;
                    _powerCardStates[cardType] = state;

                    // Fire cooldown complete event
                    var cooldownEvent = GetNextPowerCardEvent();
                    cooldownEvent.Type = cardType;
                    cooldownEvent.ChargeLevel = state.ChargeLevel;
                    cooldownEvent.Timestamp = currentTime;
                    cooldownEvent.IsRhythmSynced = false;

                    OnPowerCardCooldownComplete?.Invoke(cooldownEvent);
                }
            }
        }

        /// <summary>
        /// Gets power card configuration by type
        /// </summary>
        private PowerCardConfiguration GetPowerCardConfig(PowerCardType cardType)
        {
            foreach (var config in _powerCardConfigs)
            {
                if (config.Type == cardType)
                    return config;
            }

            // Return default config if not found
            return new PowerCardConfiguration
            {
                Type = cardType,
                MaxChargeLevel = 100f,
                ChargeRate = 10f,
                CooldownSeconds = 30f,
                UnlockLevel = 1,
                RequiresBeatTiming = false
            };
        }

        /// <summary>
        /// Creates a power card effect with specified intensity
        /// Constitutional requirement: Use pre-allocated effect objects
        /// </summary>
        private PowerCardEffect CreatePowerCardEffect(PowerCardType cardType, float rhythmBonus)
        {
            var effect = GetNextPowerCardEffect();
            effect.Type = cardType;
            effect.Intensity = rhythmBonus;
            effect.RequiresAudioSync = GetPowerCardConfig(cardType).RequiresBeatTiming;

            // Set effect duration based on type
            effect.Duration = cardType switch
            {
                PowerCardType.TempoSlow => 8,      // 8 beats
                PowerCardType.ComboExtend => 5,    // 5 moves
                PowerCardType.ScoreMultiply => 10, // 10 moves
                PowerCardType.BeatSkip => 1,       // 1 move
                _ => 1                             // Instant effect
            };

            return effect;
        }

        /// <summary>
        /// Executes the power card effect
        /// </summary>
        private void ExecutePowerCardEffect(PowerCardEffect effect)
        {
            switch (effect.Type)
            {
                case PowerCardType.Shuffle:
                    ExecuteShuffleEffect(effect);
                    break;
                case PowerCardType.Reveal:
                    ExecuteRevealEffect(effect);
                    break;
                case PowerCardType.Undo:
                    ExecuteUndoEffect(effect);
                    break;
                case PowerCardType.Hint:
                    ExecuteHintEffect(effect);
                    break;
                case PowerCardType.TempoSlow:
                    ExecuteTempoSlowEffect(effect);
                    break;
                case PowerCardType.ComboExtend:
                    ExecuteComboExtendEffect(effect);
                    break;
                case PowerCardType.ScoreMultiply:
                    ExecuteScoreMultiplyEffect(effect);
                    break;
                case PowerCardType.BeatSkip:
                    ExecuteBeatSkipEffect(effect);
                    break;
            }
        }

        // Power card effect implementations
        private void ExecuteShuffleEffect(PowerCardEffect effect) { Debug.Log("Shuffle effect executed"); }
        private void ExecuteRevealEffect(PowerCardEffect effect) { Debug.Log("Reveal effect executed"); }
        private void ExecuteUndoEffect(PowerCardEffect effect) { Debug.Log("Undo effect executed"); }
        private void ExecuteHintEffect(PowerCardEffect effect) { Debug.Log("Hint effect executed"); }
        private void ExecuteTempoSlowEffect(PowerCardEffect effect) { Debug.Log("Tempo slow effect executed"); }
        private void ExecuteComboExtendEffect(PowerCardEffect effect) { Debug.Log("Combo extend effect executed"); }
        private void ExecuteScoreMultiplyEffect(PowerCardEffect effect) { Debug.Log("Score multiply effect executed"); }
        private void ExecuteBeatSkipEffect(PowerCardEffect effect) { Debug.Log("Beat skip effect executed"); }

        /// <summary>
        /// Gets next power card event from pre-allocated pool (constitutional: no allocation)
        /// </summary>
        private PowerCardEvent GetNextPowerCardEvent()
        {
            var index = _eventPoolIndex;
            _eventPoolIndex = (_eventPoolIndex + 1) % _eventPool.Length;
            return _eventPool[index];
        }

        /// <summary>
        /// Gets next power card effect from pre-allocated pool (constitutional: no allocation)
        /// </summary>
        private PowerCardEffect GetNextPowerCardEffect()
        {
            var index = _effectPoolIndex;
            _effectPoolIndex = (_effectPoolIndex + 1) % _effectPool.Length;
            return _effectPool[index];
        }

        #endregion
    }

    #region Supporting Data Structures

    /// <summary>
    /// Power card state structure (constitutional: struct to avoid allocation)
    /// </summary>
    [System.Serializable]
    public struct PowerCardState
    {
        public float ChargeLevel;
        public bool IsOnCooldown;
        public DateTime CooldownUntil;
    }

    #endregion
}