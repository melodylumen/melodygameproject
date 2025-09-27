using System;
using UnityEngine;
using RhythmSolitaire.Contracts;
using RhythmSolitaire.Core;

namespace RhythmSolitaire.Audio
{
    /// <summary>
    /// Audio Engine implementation - provides rhythm synchronization and dynamic music
    /// Constitutional compliance: Audio Quality First, Real-Time Performance
    /// CRITICAL: <20ms latency, zero allocation in audio threads
    /// </summary>
    public class AudioEngine : MonoBehaviour, IAudioEngine
    {
        #region Constitutional Requirements

        private const float CONSTITUTIONAL_MAX_LATENCY_MS = 20f;
        private const float BEAT_TIMING_MAX_DURATION_MS = 5f;
        private const float MOVE_VALIDATION_MAX_DURATION_MS = 1f;
        private const float TRACK_LOAD_MAX_DURATION_MS = 100f;

        #endregion

        #region Pre-allocated Memory Pools (Constitutional Requirement)

        // Pre-allocated arrays to avoid GC allocation in audio threads
        private readonly MusicLayer[] _preallocatedLayers = new MusicLayer[8];
        private readonly BeatEvent[] _beatEventPool = new BeatEvent[32];
        private readonly ComboEvent[] _comboEventPool = new ComboEvent[16];
        private readonly LatencyWarning[] _latencyWarningPool = new LatencyWarning[8];

        // Pool indices for lock-free access
        private int _beatEventPoolIndex = 0;
        private int _comboEventPoolIndex = 0;
        private int _latencyWarningPoolIndex = 0;

        #endregion

        #region Core State

        [SerializeField] private BeatGrid _currentBeatGrid;
        [SerializeField] private MusicTrack _currentMusicTrack;
        [SerializeField] private float _currentLatencyMs = 0f;
        [SerializeField] private int _activeLayers = 0;
        [SerializeField] private bool _isInitialized = false;

        // Timing state (updated in audio thread)
        private double _lastBeatTime = 0.0;
        private double _nextBeatTime = 0.0;
        private float _currentTempo = 120f;
        private int _currentBeatIndex = 0;

        #endregion

        #region Events (Constitutional: Fire-and-forget)

        public event Action<BeatEvent> OnBeatTrigger;
        public event Action<ComboEvent> OnComboMilestone;
        public event Action<LatencyWarning> OnLatencyExceeded;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            InitializeAudioEngine();
        }

        private void Update()
        {
            // Constitutional requirement: Validate frame rate and audio performance
            ConstitutionalCompliance.ValidateFrameRate();
            ConstitutionalCompliance.ValidateAudioLatency(_currentLatencyMs);

            UpdateAudioTiming();
        }

        private void OnAudioFilterRead(float[] data, int channels)
        {
            // CONSTITUTIONAL CRITICAL: This runs in audio thread - ZERO allocation allowed
            // All operations here must use pre-allocated memory only

            // Measure audio latency for constitutional compliance
            double currentTime = AudioSettings.dspTime;
            _currentLatencyMs = (float)((currentTime - _lastBeatTime) * 1000.0);

            // Constitutional compliance check
            if (_currentLatencyMs > CONSTITUTIONAL_MAX_LATENCY_MS)
            {
                // Use pre-allocated warning from pool (no allocation)
                var warning = GetNextLatencyWarning();
                warning.LatencyMs = _currentLatencyMs;
                warning.Timestamp = currentTime;
                OnLatencyExceeded?.Invoke(warning);
            }
        }

        #endregion

        #region IAudioEngine Implementation

        /// <summary>
        /// Gets current beat timing information for visual indicators
        /// Constitutional requirement: Must complete within <5ms
        /// </summary>
        public BeatTimingData GetCurrentBeatTiming()
        {
            var startTime = Time.realtimeSinceStartup;

            // Use current audio timing state (no allocation)
            var beatTiming = new BeatTimingData
            {
                CurrentBeatTime = _lastBeatTime,
                NextBeatTime = _nextBeatTime,
                BeatDuration = 60.0 / _currentTempo,
                Tempo = _currentTempo,
                BeatIndex = _currentBeatIndex
            };

            // Constitutional compliance: Validate execution time
            var executionTime = (Time.realtimeSinceStartup - startTime) * 1000f;
            if (executionTime > BEAT_TIMING_MAX_DURATION_MS)
            {
                Debug.LogError($"CONSTITUTIONAL VIOLATION: GetCurrentBeatTiming() took {executionTime}ms, must be <{BEAT_TIMING_MAX_DURATION_MS}ms");
            }

            return beatTiming;
        }

        /// <summary>
        /// Validates move timing against current beat
        /// Constitutional requirement: Must complete within <1ms
        /// </summary>
        public TimingAccuracy ValidateMoveTiming(DateTime moveTime)
        {
            var startTime = Time.realtimeSinceStartup;

            // Convert DateTime to audio time (simplified - real implementation would be more precise)
            double moveTimeSeconds = AudioSettings.dspTime;
            double timeDifference = Math.Abs(moveTimeSeconds - _nextBeatTime);

            TimingAccuracy accuracy;
            if (_currentBeatGrid != null)
            {
                accuracy = _currentBeatGrid.ValidateMoveTiming(moveTime);
            }
            else
            {
                // Fallback timing validation without beat grid
                if (timeDifference <= 0.1) // 100ms perfect window
                    accuracy = TimingAccuracy.Perfect;
                else if (timeDifference <= 0.2) // 200ms good window
                    accuracy = TimingAccuracy.Good;
                else
                    accuracy = TimingAccuracy.Missed;
            }

            // Constitutional compliance: Validate execution time
            var executionTime = (Time.realtimeSinceStartup - startTime) * 1000f;
            if (executionTime > MOVE_VALIDATION_MAX_DURATION_MS)
            {
                Debug.LogError($"CONSTITUTIONAL VIOLATION: ValidateMoveTiming() took {executionTime}ms, must be <{MOVE_VALIDATION_MAX_DURATION_MS}ms");
            }

            return accuracy;
        }

        /// <summary>
        /// Updates music layers based on player performance
        /// Constitutional requirement: Must use pre-allocated layer objects only
        /// </summary>
        public void UpdateMusicLayers(int comboCount, PowerCardType activePower)
        {
            // Constitutional requirement: Use pre-allocated memory only
            int targetLayers = Mathf.Min(comboCount / 5, _preallocatedLayers.Length);

            // Update active layer count without allocation
            if (targetLayers != _activeLayers)
            {
                _activeLayers = targetLayers;

                // Update music track if available
                if (_currentMusicTrack != null)
                {
                    // This would integrate with FMOD for actual audio processing
                    Debug.Log($"Updated music layers: {_activeLayers} active");
                }

                // Trigger combo milestone event using pre-allocated event
                if (comboCount % 10 == 0) // Every 10th combo
                {
                    var comboEvent = GetNextComboEvent();
                    comboEvent.ComboCount = comboCount;
                    comboEvent.LayersActivated = _activeLayers;
                    OnComboMilestone?.Invoke(comboEvent);
                }
            }
        }

        /// <summary>
        /// Loads and initializes a music track
        /// Constitutional requirement: Must complete within 100ms
        /// </summary>
        public bool LoadMusicTrack(MusicGenre genre, DifficultyLevel difficulty)
        {
            var startTime = Time.realtimeSinceStartup;

            try
            {
                // Create or load music track
                if (_currentMusicTrack == null)
                {
                    _currentMusicTrack = MusicTrack.CreateTrack($"track_{genre}_{difficulty}", genre, GetTempoForGenre(genre));
                }

                // Load the track
                bool loadSuccess = _currentMusicTrack.LoadTrack();
                if (!loadSuccess)
                {
                    Debug.LogError($"Failed to load music track: {genre}");
                    return false;
                }

                // Initialize beat grid for this track
                _currentBeatGrid = BeatGrid.CreateBeatGrid($"grid_{genre}", _currentMusicTrack.BaseTempo);
                _currentBeatGrid.AdaptTodifficulty(difficulty);

                // Update timing state
                _currentTempo = _currentMusicTrack.BaseTempo;
                _lastBeatTime = AudioSettings.dspTime;
                _nextBeatTime = _lastBeatTime + (60.0 / _currentTempo);

                // Constitutional compliance: Validate execution time
                var executionTime = (Time.realtimeSinceStartup - startTime) * 1000f;
                if (executionTime > TRACK_LOAD_MAX_DURATION_MS)
                {
                    Debug.LogError($"CONSTITUTIONAL VIOLATION: LoadMusicTrack() took {executionTime}ms, must be <{TRACK_LOAD_MAX_DURATION_MS}ms");
                    return false;
                }

                Debug.Log($"Successfully loaded music track: {genre} at {_currentTempo}BPM");
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Exception loading music track: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Provides visual beat data for hearing-impaired players
        /// Constitutional requirement: Accessibility compliance
        /// </summary>
        public VisualBeatData GetVisualBeatIndicators()
        {
            double currentTime = AudioSettings.dspTime;
            double timeToBeat = _nextBeatTime - currentTime;
            double beatInterval = 60.0 / _currentTempo;

            return new VisualBeatData
            {
                BeatIntensity = Mathf.Clamp01((float)(1.0 - Math.Abs(timeToBeat) / (beatInterval * 0.5))),
                NextBeatProgress = Mathf.Clamp01((float)(1.0 - timeToBeat / beatInterval)),
                IsStrongBeat = (_currentBeatIndex % 4) == 0, // Every 4th beat is strong
                BeatColor = GetBeatColor(_currentBeatIndex)
            };
        }

        /// <summary>
        /// Gets current audio latency measurement
        /// Constitutional requirement: Must return <20ms
        /// </summary>
        public float GetCurrentLatencyMs()
        {
            // Return measured latency with constitutional compliance
            if (_currentLatencyMs > CONSTITUTIONAL_MAX_LATENCY_MS)
            {
                Debug.LogError($"CONSTITUTIONAL VIOLATION: Audio latency {_currentLatencyMs}ms exceeds maximum {CONSTITUTIONAL_MAX_LATENCY_MS}ms");
            }

            return _currentLatencyMs;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Initializes the audio engine with constitutional compliance
        /// </summary>
        private void InitializeAudioEngine()
        {
            // Initialize pre-allocated memory pools
            for (int i = 0; i < _preallocatedLayers.Length; i++)
            {
                _preallocatedLayers[i] = new MusicLayer { LayerIndex = i, IsActive = false };
            }

            for (int i = 0; i < _beatEventPool.Length; i++)
            {
                _beatEventPool[i] = new BeatEvent();
            }

            for (int i = 0; i < _comboEventPool.Length; i++)
            {
                _comboEventPool[i] = new ComboEvent();
            }

            for (int i = 0; i < _latencyWarningPool.Length; i++)
            {
                _latencyWarningPool[i] = new LatencyWarning();
            }

            // Set initial timing state
            _lastBeatTime = AudioSettings.dspTime;
            _nextBeatTime = _lastBeatTime + 0.5; // 120 BPM default
            _currentTempo = 120f;
            _currentBeatIndex = 0;

            _isInitialized = true;
            Debug.Log("AudioEngine initialized with constitutional compliance");
        }

        /// <summary>
        /// Updates audio timing state (called every frame)
        /// Constitutional requirement: Efficient updates without allocation
        /// </summary>
        private void UpdateAudioTiming()
        {
            if (!_isInitialized) return;

            double currentTime = AudioSettings.dspTime;

            // Check if we've passed the next beat
            if (currentTime >= _nextBeatTime)
            {
                _currentBeatIndex++;
                _lastBeatTime = _nextBeatTime;
                _nextBeatTime += (60.0 / _currentTempo);

                // Trigger beat event using pre-allocated event
                var beatEvent = GetNextBeatEvent();
                beatEvent.BeatIndex = _currentBeatIndex;
                beatEvent.BeatTime = _lastBeatTime;
                beatEvent.IsStrongBeat = (_currentBeatIndex % 4) == 0;
                OnBeatTrigger?.Invoke(beatEvent);

                // Update music track if available
                if (_currentMusicTrack != null)
                {
                    _currentMusicTrack.UpdateTrack(currentTime);
                }
            }
        }

        /// <summary>
        /// Gets tempo for music genre (constitutional: no allocation)
        /// </summary>
        private float GetTempoForGenre(MusicGenre genre)
        {
            return genre switch
            {
                MusicGenre.LoFi => 85f,
                MusicGenre.EDM => 128f,
                MusicGenre.Orchestral => 100f,
                MusicGenre.Electronic => 140f,
                MusicGenre.Ambient => 70f,
                _ => 120f
            };
        }

        /// <summary>
        /// Gets beat color for visual indicators (constitutional: no allocation)
        /// </summary>
        private Color GetBeatColor(int beatIndex)
        {
            return (beatIndex % 4) switch
            {
                0 => Color.white,    // Strong beat (downbeat)
                1 => Color.yellow,   // Weak beat
                2 => Color.cyan,     // Medium beat
                3 => Color.yellow,   // Weak beat
                _ => Color.gray
            };
        }

        /// <summary>
        /// Gets next beat event from pre-allocated pool (constitutional: no allocation)
        /// </summary>
        private BeatEvent GetNextBeatEvent()
        {
            var index = _beatEventPoolIndex;
            _beatEventPoolIndex = (_beatEventPoolIndex + 1) % _beatEventPool.Length;
            return _beatEventPool[index];
        }

        /// <summary>
        /// Gets next combo event from pre-allocated pool (constitutional: no allocation)
        /// </summary>
        private ComboEvent GetNextComboEvent()
        {
            var index = _comboEventPoolIndex;
            _comboEventPoolIndex = (_comboEventPoolIndex + 1) % _comboEventPool.Length;
            return _comboEventPool[index];
        }

        /// <summary>
        /// Gets next latency warning from pre-allocated pool (constitutional: no allocation)
        /// </summary>
        private LatencyWarning GetNextLatencyWarning()
        {
            var index = _latencyWarningPoolIndex;
            _latencyWarningPoolIndex = (_latencyWarningPoolIndex + 1) % _latencyWarningPool.Length;
            return _latencyWarningPool[index];
        }

        #endregion
    }

    #region Supporting Data Structures

    /// <summary>
    /// Pre-allocated music layer structure (constitutional: no allocation)
    /// </summary>
    public class MusicLayer
    {
        public int LayerIndex;
        public bool IsActive;
        public float Volume;
    }


    #endregion
}