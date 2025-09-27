using System;
using UnityEngine;
using UnityEngine.Profiling;

namespace RhythmSolitaire.Core
{
    /// <summary>
    /// Constitutional compliance monitoring and validation system
    /// Ensures all constitutional principles are upheld during runtime
    /// </summary>
    public static class ConstitutionalCompliance
    {
        #region Constitutional Principle I: Audio Quality First

        private static float _currentAudioLatency = 0f;
        private const float MAX_AUDIO_LATENCY_MS = 20f;

        /// <summary>
        /// Monitor and validate audio latency compliance
        /// CONSTITUTIONAL REQUIREMENT: <20ms latency
        /// </summary>
        public static bool ValidateAudioLatency(float latencyMs)
        {
            _currentAudioLatency = latencyMs;
            bool isCompliant = latencyMs <= MAX_AUDIO_LATENCY_MS;

            if (!isCompliant)
            {
                Debug.LogError($"CONSTITUTIONAL VIOLATION: Audio latency {latencyMs}ms exceeds maximum {MAX_AUDIO_LATENCY_MS}ms");
            }

            return isCompliant;
        }

        #endregion

        #region Constitutional Principle II: Real-Time Performance

        private static float _currentFrameRate = 0f;
        private const float MIN_FRAME_RATE = 60f;
        private static bool _audioThreadAllocationDetected = false;

        /// <summary>
        /// Monitor frame rate stability
        /// CONSTITUTIONAL REQUIREMENT: 60fps stable
        /// </summary>
        public static bool ValidateFrameRate()
        {
            _currentFrameRate = 1f / Time.unscaledDeltaTime;
            bool isCompliant = _currentFrameRate >= MIN_FRAME_RATE;

            if (!isCompliant)
            {
                Debug.LogWarning($"Performance Warning: Frame rate {_currentFrameRate:F1}fps below target {MIN_FRAME_RATE}fps");
            }

            return isCompliant;
        }

        /// <summary>
        /// Detect forbidden memory allocation in audio threads
        /// CONSTITUTIONAL REQUIREMENT: Zero allocation in audio threads
        /// </summary>
        public static void MonitorAudioThreadAllocation()
        {
            // This would integrate with Unity Profiler API when available
            // For now, provides the interface for constitutional compliance

            if (_audioThreadAllocationDetected)
            {
                Debug.LogError("CONSTITUTIONAL VIOLATION: Memory allocation detected in audio thread");
            }
        }

        #endregion

        #region Constitutional Principle III: Test-Driven Development

        /// <summary>
        /// Validate that TDD order is maintained
        /// CONSTITUTIONAL REQUIREMENT: Tests before implementation
        /// </summary>
        public static bool ValidateTDDCompliance(string componentName, bool testsExist, bool implementationExists)
        {
            if (implementationExists && !testsExist)
            {
                Debug.LogError($"CONSTITUTIONAL VIOLATION: Implementation of {componentName} exists without tests");
                return false;
            }

            return true;
        }

        #endregion

        #region Constitutional Principle IV: Modular Architecture

        /// <summary>
        /// Validate interface-based design compliance
        /// CONSTITUTIONAL REQUIREMENT: Loosely coupled interfaces
        /// </summary>
        public static bool ValidateModularDesign(Type implementationType, Type[] requiredInterfaces)
        {
            foreach (var requiredInterface in requiredInterfaces)
            {
                if (!requiredInterface.IsAssignableFrom(implementationType))
                {
                    Debug.LogError($"CONSTITUTIONAL VIOLATION: {implementationType.Name} does not implement required interface {requiredInterface.Name}");
                    return false;
                }
            }

            return true;
        }

        #endregion

        #region Constitutional Principle V: Accessibility & Usability

        /// <summary>
        /// Validate accessibility compliance (WCAG 2.1 AA)
        /// CONSTITUTIONAL REQUIREMENT: Visual indicators for audio cues
        /// </summary>
        public static bool ValidateAccessibilityCompliance(bool hasVisualIndicators, bool hasConfigurableDifficulty)
        {
            bool isCompliant = hasVisualIndicators && hasConfigurableDifficulty;

            if (!isCompliant)
            {
                Debug.LogError("CONSTITUTIONAL VIOLATION: Missing accessibility features");
            }

            return isCompliant;
        }

        #endregion

        #region Public Status Interface

        /// <summary>
        /// Get current constitutional compliance status
        /// </summary>
        public static ConstitutionalStatus GetComplianceStatus()
        {
            return new ConstitutionalStatus
            {
                AudioLatencyMs = _currentAudioLatency,
                FrameRate = _currentFrameRate,
                IsAudioLatencyCompliant = _currentAudioLatency <= MAX_AUDIO_LATENCY_MS,
                IsFrameRateCompliant = _currentFrameRate >= MIN_FRAME_RATE,
                HasAudioThreadViolation = _audioThreadAllocationDetected
            };
        }

        #endregion
    }

    [Serializable]
    public struct ConstitutionalStatus
    {
        public float AudioLatencyMs;
        public float FrameRate;
        public bool IsAudioLatencyCompliant;
        public bool IsFrameRateCompliant;
        public bool HasAudioThreadViolation;

        public bool IsFullyCompliant => IsAudioLatencyCompliant && IsFrameRateCompliant && !HasAudioThreadViolation;
    }
}