using System;
using UnityEngine;
using RhythmSolitaire.Core;
using RhythmSolitaire.Audio;

namespace RhythmSolitaire.PowerCards
{
    /// <summary>
    /// Power Card entity model - represents special abilities that charge based on rhythm performance
    /// Constitutional compliance: Real-Time Performance, Modular Architecture
    /// </summary>
    [CreateAssetMenu(fileName = "NewPowerCard", menuName = "Rhythm Solitaire/Power Card")]
    public class PowerCard : ScriptableObject
    {
        #region Core Fields

        [SerializeField]
        private string _powerCardId;

        [SerializeField]
        private PowerCardType _effectType;

        [SerializeField]
        private float _chargeLevel;

        [SerializeField]
        private ActivationCondition[] _activationConditions;

        [SerializeField]
        private float _cooldownTime;

        [SerializeField]
        private bool _isActive;

        [SerializeField]
        private float _lastActivationTime;

        #endregion

        #region Properties

        public string PowerCardId
        {
            get => _powerCardId;
            private set => _powerCardId = value;
        }

        public PowerCardType EffectType
        {
            get => _effectType;
            private set => _effectType = value;
        }

        public float ChargeLevel
        {
            get => _chargeLevel;
            set
            {
                var oldCharge = _chargeLevel;
                _chargeLevel = Mathf.Clamp(value, 0f, 100f);
                OnChargeChanged?.Invoke(this, oldCharge, _chargeLevel);
            }
        }

        public ActivationCondition[] ActivationConditions
        {
            get => _activationConditions;
            private set => _activationConditions = value;
        }

        public float CooldownTime
        {
            get => _cooldownTime;
            private set => _cooldownTime = value;
        }

        public bool IsActive
        {
            get => _isActive;
            private set => _isActive = value;
        }

        public bool IsReady => ChargeLevel >= 100f && !IsOnCooldown;
        public bool IsOnCooldown => Time.time - _lastActivationTime < _cooldownTime;
        public float RemainingCooldown => Mathf.Max(0f, _cooldownTime - (Time.time - _lastActivationTime));

        #endregion

        #region Relationships

        public GameSession GameSession { get; set; }
        public BeatGrid BeatGrid { get; set; }

        #endregion

        #region Events

        public event Action<PowerCard, float, float> OnChargeChanged;
        public event Action<PowerCard> OnActivated;
        public event Action<PowerCard> OnCooldownStarted;

        #endregion

        #region Public Methods

        public static PowerCard CreatePowerCard(string cardId, PowerCardType effectType, float cooldownTime)
        {
            var powerCard = CreateInstance<PowerCard>();
            powerCard.PowerCardId = cardId;
            powerCard.EffectType = effectType;
            powerCard.CooldownTime = cooldownTime;
            powerCard.ChargeLevel = 0f;
            powerCard.IsActive = true;

            return powerCard;
        }

        public void UpdateCharge(TimingAccuracy accuracy, int comboCount)
        {
            if (!IsActive) return;

            float chargeAmount = accuracy switch
            {
                TimingAccuracy.Perfect => 10f + (comboCount * 0.5f),
                TimingAccuracy.Good => 5f + (comboCount * 0.25f),
                TimingAccuracy.Missed => 0f,
                _ => 0f
            };

            ChargeLevel += chargeAmount;
        }

        public PowerCardResult TryActivate(DateTime activationTime)
        {
            if (!IsReady)
            {
                return new PowerCardResult
                {
                    Result = ActivationResult.InsufficientCharge,
                    RhythmBonus = 0f,
                    EffectDescription = "Power card not ready for activation"
                };
            }

            if (IsOnCooldown)
            {
                return new PowerCardResult
                {
                    Result = ActivationResult.OnCooldown,
                    RhythmBonus = 0f,
                    EffectDescription = $"Power card on cooldown for {RemainingCooldown:F1}s"
                };
            }

            // Activate power card
            ChargeLevel = 0f;
            _lastActivationTime = Time.time;

            OnActivated?.Invoke(this);
            OnCooldownStarted?.Invoke(this);

            return new PowerCardResult
            {
                Result = ActivationResult.Success,
                RhythmBonus = CalculateRhythmBonus(activationTime),
                EffectDescription = GetEffectDescription()
            };
        }

        #endregion

        #region Private Methods

        private float CalculateRhythmBonus(DateTime activationTime)
        {
            // Calculate rhythm bonus based on timing accuracy
            // This would integrate with beat timing system
            return 1.0f; // Base bonus
        }

        private string GetEffectDescription()
        {
            return EffectType switch
            {
                PowerCardType.Shuffle => "Shuffles available cards",
                PowerCardType.Reveal => "Reveals hidden cards",
                PowerCardType.Multiply => "Multiplies score temporarily",
                PowerCardType.Tempo => "Slows down tempo temporarily",
                _ => "Unknown effect"
            };
        }

        #endregion
    }

    public enum PowerCardType
    {
        Shuffle,
        Reveal,
        Multiply,
        Tempo
    }

    public enum TimingAccuracy
    {
        Perfect,
        Good,
        Missed
    }

    public enum ActivationResult
    {
        Success,
        InsufficientCharge,
        OnCooldown,
        InvalidTiming,
        Failed
    }

    [System.Serializable]
    public class ActivationCondition
    {
        public string ConditionName;
        public float RequiredValue;
        public bool IsMet;
    }

    public class PowerCardResult
    {
        public ActivationResult Result { get; set; }
        public float RhythmBonus { get; set; }
        public string EffectDescription { get; set; }
    }
}