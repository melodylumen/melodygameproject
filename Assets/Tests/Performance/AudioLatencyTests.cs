using System;
using System.Collections;
using System.Diagnostics;
using NUnit.Framework;
using RhythmSolitaire.Core;
using RhythmSolitaire.Audio;
using UnityEngine;
using UnityEngine.TestTools;

namespace RhythmSolitaire.Tests.Performance
{
    /// <summary>
    /// Constitutional compliance test: Audio latency <20ms validation
    /// CONSTITUTIONAL REQUIREMENT: Audio Quality First principle
    /// These tests MUST validate that audio latency never exceeds 20ms
    /// </summary>
    [TestFixture]
    public class AudioLatencyTests
    {
        private IAudioEngine _audioEngine;
        private const float CONSTITUTIONAL_MAX_LATENCY_MS = 20f;

        [SetUp]
        public void SetUp()
        {
            // This will FAIL until AudioEngine is implemented
            // CONSTITUTIONAL REQUIREMENT: This test MUST fail first (TDD)
            try
            {
                _audioEngine = new AudioEngine(); // This class doesn't exist yet - MUST FAIL
            }
            catch (Exception)
            {
                _audioEngine = null; // Expected to fail during TDD phase
            }
        }

        [Test]
        public void AudioLatency_MustNeverExceed20ms_ConstitutionalRequirement()
        {
            // CONSTITUTIONAL REQUIREMENT: This test MUST fail until implementation exists
            Assert.IsNotNull(_audioEngine, "AudioEngine implementation does not exist yet - TDD requirement");

            if (_audioEngine == null)
            {
                Assert.Fail("CONSTITUTIONAL VIOLATION: AudioEngine must be implemented to validate latency compliance");
                return;
            }

            // Constitutional Principle I: Audio Quality First
            // CRITICAL: Audio latency must be <20ms at all times
            var currentLatency = _audioEngine.GetCurrentLatencyMs();

            // This is a CONSTITUTIONAL REQUIREMENT - failure means project non-compliance
            Assert.That(currentLatency, Is.LessThan(CONSTITUTIONAL_MAX_LATENCY_MS),
                "CONSTITUTIONAL VIOLATION: Audio latency {0}ms exceeds constitutional maximum {1}ms. " +
                "This violates Constitutional Principle I: Audio Quality First",
                currentLatency, CONSTITUTIONAL_MAX_LATENCY_MS);

            // Additional validation: Latency should be positive and reasonable
            Assert.That(currentLatency, Is.GreaterThan(0f),
                "Audio latency measurement must be positive");
            Assert.That(currentLatency, Is.LessThan(5f),
                "Audio latency should ideally be <5ms for optimal experience");
        }

        [UnityTest]
        public IEnumerator AudioLatency_UnderLoad_MustMaintainConstitutionalCompliance()
        {
            // CONSTITUTIONAL REQUIREMENT: This test MUST fail until implementation exists
            Assert.IsNotNull(_audioEngine, "AudioEngine implementation does not exist yet - TDD requirement");

            if (_audioEngine == null) yield break;

            // Test latency stability under typical game load
            var measurements = new float[60]; // 1 second at 60fps
            var violations = 0;

            for (int frame = 0; frame < 60; frame++)
            {
                // Simulate game load
                yield return null; // Wait one frame

                var latency = _audioEngine.GetCurrentLatencyMs();
                measurements[frame] = latency;

                if (latency >= CONSTITUTIONAL_MAX_LATENCY_MS)
                {
                    violations++;
                }

                // Constitutional compliance: Validate real-time performance
                ConstitutionalCompliance.ValidateAudioLatency(latency);
            }

            // Calculate statistics
            var avgLatency = 0f;
            var maxLatency = 0f;
            foreach (var measurement in measurements)
            {
                avgLatency += measurement;
                if (measurement > maxLatency) maxLatency = measurement;
            }
            avgLatency /= measurements.Length;

            // Constitutional validation: Zero tolerance for violations
            Assert.That(violations, Is.EqualTo(0),
                "CONSTITUTIONAL VIOLATION: {0} latency violations detected during load test. " +
                "Max latency: {1}ms, Avg latency: {2}ms", violations, maxLatency, avgLatency);

            Assert.That(maxLatency, Is.LessThan(CONSTITUTIONAL_MAX_LATENCY_MS),
                "CONSTITUTIONAL VIOLATION: Peak latency {0}ms exceeds constitutional maximum {1}ms",
                maxLatency, CONSTITUTIONAL_MAX_LATENCY_MS);
        }

        [Test]
        public void AudioLatency_DuringBeatSynchronization_MustRemainLowLatency()
        {
            // CONSTITUTIONAL REQUIREMENT: This test MUST fail until implementation exists
            Assert.IsNotNull(_audioEngine, "AudioEngine implementation does not exist yet - TDD requirement");

            if (_audioEngine == null) return;

            // Test latency during critical beat synchronization operations
            var stopwatch = Stopwatch.StartNew();

            // Simulate beat timing validation (critical path)
            var beatTiming = _audioEngine.GetCurrentBeatTiming();
            var moveTime = DateTime.UtcNow;
            var timingAccuracy = _audioEngine.ValidateMoveTiming(moveTime);

            stopwatch.Stop();

            // Measure end-to-end latency for critical timing operations
            var operationLatency = stopwatch.Elapsed.TotalMilliseconds;
            var systemLatency = _audioEngine.GetCurrentLatencyMs();

            // Constitutional requirement: Beat validation must be near-instantaneous
            Assert.That(operationLatency, Is.LessThan(1.0),
                "Beat synchronization operations took {0}ms, must be <1ms for real-time response",
                operationLatency);

            Assert.That(systemLatency, Is.LessThan(CONSTITUTIONAL_MAX_LATENCY_MS),
                "CONSTITUTIONAL VIOLATION: Audio latency {0}ms during beat sync exceeds {1}ms",
                systemLatency, CONSTITUTIONAL_MAX_LATENCY_MS);
        }

        [Test]
        public void AudioLatency_AcrossPlatforms_MustMeetConstitutionalStandards()
        {
            // CONSTITUTIONAL REQUIREMENT: This test MUST fail until implementation exists
            Assert.IsNotNull(_audioEngine, "AudioEngine implementation does not exist yet - TDD requirement");

            if (_audioEngine == null) return;

            // Test cross-platform latency compliance
            var currentPlatform = Application.platform;
            var latency = _audioEngine.GetCurrentLatencyMs();

            // Constitutional requirement applies to ALL platforms
            Assert.That(latency, Is.LessThan(CONSTITUTIONAL_MAX_LATENCY_MS),
                "CONSTITUTIONAL VIOLATION: Audio latency {0}ms on {1} exceeds constitutional maximum {2}ms",
                latency, currentPlatform, CONSTITUTIONAL_MAX_LATENCY_MS);

            // Platform-specific optimizations may allow for better performance
            switch (currentPlatform)
            {
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WindowsEditor:
                    Assert.That(latency, Is.LessThan(10f),
                        "Windows should achieve <10ms latency with proper ASIO/WASAPI configuration");
                    break;
                case RuntimePlatform.OSXPlayer:
                case RuntimePlatform.OSXEditor:
                    Assert.That(latency, Is.LessThan(8f),
                        "macOS should achieve <8ms latency with Core Audio");
                    break;
                case RuntimePlatform.IPhonePlayer:
                case RuntimePlatform.Android:
                    Assert.That(latency, Is.LessThan(15f),
                        "Mobile platforms should achieve <15ms latency");
                    break;
            }
        }

        [Test]
        public void ConstitutionalCompliance_AudioQualityFirst_IntegrationTest()
        {
            // Test integration with constitutional compliance system
            var complianceStatus = ConstitutionalCompliance.GetComplianceStatus();

            // If AudioEngine exists, it must be constitutionally compliant
            if (_audioEngine != null)
            {
                Assert.IsTrue(complianceStatus.IsAudioLatencyCompliant,
                    "Constitutional compliance system reports audio latency violation");
                Assert.That(complianceStatus.AudioLatencyMs, Is.LessThan(CONSTITUTIONAL_MAX_LATENCY_MS),
                    "Compliance system reports latency {0}ms, exceeds constitutional limit {1}ms",
                    complianceStatus.AudioLatencyMs, CONSTITUTIONAL_MAX_LATENCY_MS);
            }
        }

        [TearDown]
        public void TearDown()
        {
            // Validate constitutional compliance after each test
            if (_audioEngine != null)
            {
                var finalLatency = _audioEngine.GetCurrentLatencyMs();

                // Log for constitutional audit trail
                UnityEngine.Debug.Log($"Constitutional Audit: Final audio latency {finalLatency}ms " +
                    $"(Constitutional limit: {CONSTITUTIONAL_MAX_LATENCY_MS}ms)");

                _audioEngine = null;
            }
        }
    }
}