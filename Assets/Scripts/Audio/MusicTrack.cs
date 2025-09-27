using System;
using System.Collections.Generic;
using UnityEngine;

namespace RhythmSolitaire.Audio
{
    /// <summary>
    /// Music Track entity model - represents adaptive soundtrack with dynamic layering
    /// Constitutional compliance: Audio Quality First, Real-Time Performance
    /// </summary>
    [CreateAssetMenu(fileName = "NewMusicTrack", menuName = "Rhythm Solitaire/Music Track")]
    public class MusicTrack : ScriptableObject
    {
        #region Core Fields

        [SerializeField]
        private string _trackId;

        [SerializeField]
        private MusicGenre _genre;

        [SerializeField]
        private float _baseTempo;

        [SerializeField]
        private AudioClip[] _currentLayers;

        [SerializeField]
        private BeatSynchronizationData _synchronizationState;

        [SerializeField]
        private AdaptiveElement[] _adaptiveElements;

        [SerializeField]
        private float _duration;

        [SerializeField]
        private bool _isLooping;

        #endregion

        #region Properties

        /// <summary>
        /// Unique identifier for this music track
        /// </summary>
        public string TrackId
        {
            get => _trackId;
            private set => _trackId = value;
        }

        /// <summary>
        /// Music genre (lo-fi, EDM, orchestral)
        /// </summary>
        public MusicGenre Genre
        {
            get => _genre;
            private set => _genre = value;
        }

        /// <summary>
        /// Base beats per minute for this track
        /// Constitutional requirement: Must maintain stable tempo for rhythm accuracy
        /// </summary>
        public float BaseTempo
        {
            get => _baseTempo;
            private set => _baseTempo = value;
        }

        /// <summary>
        /// Currently active music layers array
        /// Constitutional requirement: Pre-allocated array to avoid allocation in audio thread
        /// </summary>
        public AudioClip[] CurrentLayers
        {
            get => _currentLayers;
            private set => _currentLayers = value;
        }

        /// <summary>
        /// Beat timing and phase information for synchronization
        /// </summary>
        public BeatSynchronizationData SynchronizationState
        {
            get => _synchronizationState;
            set => _synchronizationState = value;
        }

        /// <summary>
        /// Dynamic music components that respond to gameplay
        /// </summary>
        public AdaptiveElement[] AdaptiveElements
        {
            get => _adaptiveElements;
            private set => _adaptiveElements = value;
        }

        /// <summary>
        /// Total duration of the track in seconds
        /// </summary>
        public float Duration
        {
            get => _duration;
            private set => _duration = value;
        }

        /// <summary>
        /// Whether the track should loop continuously
        /// </summary>
        public bool IsLooping
        {
            get => _isLooping;
            set => _isLooping = value;
        }

        /// <summary>
        /// Current number of active layers
        /// </summary>
        public int ActiveLayerCount => _currentLayers?.Length ?? 0;

        /// <summary>
        /// Beat interval in seconds (derived from tempo)
        /// </summary>
        public float BeatInterval => 60f / _baseTempo;

        #endregion

        #region Relationships

        /// <summary>
        /// Game sessions using this music track (used by multiple)
        /// </summary>
        public List<Core.GameSession> UsingSessions { get; private set; } = new List<Core.GameSession>();

        /// <summary>
        /// Beat grid references for timing validation (contains multiple)
        /// </summary>
        public List<BeatGrid> BeatGridReferences { get; private set; } = new List<BeatGrid>();

        /// <summary>
        /// Audio asset file references for FMOD integration
        /// </summary>
        public string[] AudioAssetPaths { get; set; }

        #endregion

        #region State Management

        /// <summary>
        /// Current track state for state machine management
        /// </summary>
        public MusicTrackState CurrentState { get; private set; } = MusicTrackState.Loading;

        /// <summary>
        /// Events for state transitions and music changes
        /// Constitutional requirement: Events must not allocate in audio thread
        /// </summary>
        public event Action<MusicTrackState, MusicTrackState> OnStateChanged;
        public event Action<int> OnLayerAdded;
        public event Action<int> OnLayerRemoved;
        public event Action OnBeatTrigger;

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a new music track with specified parameters
        /// Constitutional requirement: Deterministic creation for testing
        /// </summary>
        public static MusicTrack CreateTrack(string trackId, MusicGenre genre, float baseTempo)
        {
            var track = CreateInstance<MusicTrack>();
            track.TrackId = trackId;
            track.Genre = genre;
            track.BaseTempo = Mathf.Clamp(baseTempo, 60f, 200f); // Reasonable BPM range
            track.CurrentState = MusicTrackState.Loading;

            // Initialize arrays with reasonable defaults
            track._currentLayers = new AudioClip[8]; // Max 8 layers for performance
            track._adaptiveElements = new AdaptiveElement[4]; // Common adaptive elements

            return track;
        }

        /// <summary>
        /// Loads and initializes the music track
        /// Constitutional requirement: Must complete initialization within 100ms
        /// </summary>
        public bool LoadTrack()
        {
            if (CurrentState != MusicTrackState.Loading)
            {
                Debug.LogWarning($"Cannot load track from state {CurrentState}");
                return false;
            }

            try
            {
                // Initialize synchronization state
                _synchronizationState = new BeatSynchronizationData
                {
                    CurrentBeat = 0,
                    BeatPhase = 0f,
                    IsPlaying = false,
                    LastBeatTime = 0.0
                };

                TransitionToState(MusicTrackState.Ready);
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to load music track {TrackId}: {ex.Message}");
                TransitionToState(MusicTrackState.Error);
                return false;
            }
        }

        /// <summary>
        /// Starts playing the music track
        /// State transition: READY → PLAYING
        /// </summary>
        public bool StartPlaying()
        {
            if (CurrentState != MusicTrackState.Ready)
            {
                Debug.LogWarning($"Cannot start playing from state {CurrentState}");
                return false;
            }

            TransitionToState(MusicTrackState.Playing);
            _synchronizationState.IsPlaying = true;
            _synchronizationState.LastBeatTime = AudioSettings.dspTime;

            return true;
        }

        /// <summary>
        /// Adds a music layer for dynamic layering
        /// Constitutional requirement: Must use pre-allocated layer slots
        /// </summary>
        public bool AddLayer(AudioClip layerClip, int layerIndex)
        {
            if (layerIndex < 0 || layerIndex >= _currentLayers.Length)
            {
                Debug.LogError($"Layer index {layerIndex} out of range");
                return false;
            }

            if (_currentLayers[layerIndex] != null)
            {
                Debug.LogWarning($"Layer {layerIndex} already occupied, replacing");
            }

            _currentLayers[layerIndex] = layerClip;
            OnLayerAdded?.Invoke(layerIndex);

            return true;
        }

        /// <summary>
        /// Removes a music layer
        /// Constitutional requirement: No allocation during layer removal
        /// </summary>
        public bool RemoveLayer(int layerIndex)
        {
            if (layerIndex < 0 || layerIndex >= _currentLayers.Length)
            {
                Debug.LogError($"Layer index {layerIndex} out of range");
                return false;
            }

            if (_currentLayers[layerIndex] == null)
            {
                Debug.LogWarning($"Layer {layerIndex} is already empty");
                return false;
            }

            _currentLayers[layerIndex] = null;
            OnLayerRemoved?.Invoke(layerIndex);

            return true;
        }

        /// <summary>
        /// Triggers a musical "drop" effect for combo completion
        /// Constitutional requirement: Sample-accurate timing
        /// </summary>
        public void TriggerMusicalDrop()
        {
            if (CurrentState != MusicTrackState.Playing)
                return;

            TransitionToState(MusicTrackState.Layering);

            // Trigger dramatic musical change
            // This would integrate with FMOD for actual audio processing
            Debug.Log($"Musical drop triggered for track {TrackId}");

            // Return to playing state after drop effect
            TransitionToState(MusicTrackState.Playing);
        }

        /// <summary>
        /// Updates the track state and synchronization
        /// Constitutional requirement: Must execute without allocation in audio thread
        /// </summary>
        public void UpdateTrack(double currentDSPTime)
        {
            if (CurrentState != MusicTrackState.Playing)
                return;

            // Update beat synchronization
            double timeSinceLastBeat = currentDSPTime - _synchronizationState.LastBeatTime;
            double beatInterval = 60.0 / _baseTempo;

            if (timeSinceLastBeat >= beatInterval)
            {
                _synchronizationState.CurrentBeat++;
                _synchronizationState.LastBeatTime = currentDSPTime;
                _synchronizationState.BeatPhase = 0f;

                // Trigger beat event (constitutional: must not allocate)
                OnBeatTrigger?.Invoke();
            }
            else
            {
                // Update beat phase (0.0 to 1.0 within beat)
                _synchronizationState.BeatPhase = (float)(timeSinceLastBeat / beatInterval);
            }

            // Update adaptive elements
            UpdateAdaptiveElements();
        }

        /// <summary>
        /// Gets current beat timing data for gameplay synchronization
        /// Constitutional requirement: Must complete within <5ms
        /// </summary>
        public BeatTimingData GetBeatTimingData()
        {
            return new BeatTimingData
            {
                CurrentBeatTime = _synchronizationState.LastBeatTime,
                NextBeatTime = _synchronizationState.LastBeatTime + BeatInterval,
                BeatDuration = BeatInterval,
                Tempo = _baseTempo,
                BeatIndex = _synchronizationState.CurrentBeat
            };
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Handles state transitions with validation
        /// </summary>
        private void TransitionToState(MusicTrackState newState)
        {
            var oldState = CurrentState;

            if (!IsValidStateTransition(oldState, newState))
            {
                Debug.LogError($"Invalid state transition from {oldState} to {newState}");
                return;
            }

            CurrentState = newState;
            OnStateChanged?.Invoke(oldState, newState);
        }

        /// <summary>
        /// Validates state transitions
        /// </summary>
        private bool IsValidStateTransition(MusicTrackState from, MusicTrackState to)
        {
            return (from, to) switch
            {
                (MusicTrackState.Loading, MusicTrackState.Ready) => true,
                (MusicTrackState.Loading, MusicTrackState.Error) => true,
                (MusicTrackState.Ready, MusicTrackState.Playing) => true,
                (MusicTrackState.Playing, MusicTrackState.Layering) => true,
                (MusicTrackState.Layering, MusicTrackState.Playing) => true,
                (MusicTrackState.Playing, MusicTrackState.Dropping) => true,
                (MusicTrackState.Dropping, MusicTrackState.Playing) => true,
                _ => false
            };
        }

        /// <summary>
        /// Updates adaptive music elements based on gameplay
        /// Constitutional requirement: Efficient updates without allocation
        /// </summary>
        private void UpdateAdaptiveElements()
        {
            if (_adaptiveElements == null) return;

            for (int i = 0; i < _adaptiveElements.Length; i++)
            {
                if (_adaptiveElements[i] != null)
                {
                    _adaptiveElements[i].Update();
                }
            }
        }

        #endregion

        #region Unity Editor Support

        private void OnValidate()
        {
            // Validate tempo range
            _baseTempo = Mathf.Clamp(_baseTempo, 60f, 200f);

            // Ensure track ID is set
            if (string.IsNullOrEmpty(_trackId))
            {
                _trackId = $"track_{GetInstanceID()}";
            }
        }

        #endregion
    }

    /// <summary>
    /// Music track state enumeration
    /// </summary>
    public enum MusicTrackState
    {
        Loading,
        Ready,
        Playing,
        Layering,
        Dropping,
        Error
    }

    /// <summary>
    /// Beat synchronization data structure
    /// Constitutional requirement: Pre-allocated struct to avoid GC allocation
    /// </summary>
    [System.Serializable]
    public struct BeatSynchronizationData
    {
        public int CurrentBeat;
        public float BeatPhase;
        public bool IsPlaying;
        public double LastBeatTime;
    }

    /// <summary>
    /// Adaptive music element for dynamic soundtrack
    /// </summary>
    [System.Serializable]
    public class AdaptiveElement
    {
        public string ElementName;
        public float Intensity;
        public bool IsActive;

        public void Update()
        {
            // Update adaptive element logic
            // Constitutional requirement: No allocation during update
        }
    }

    /// <summary>
    /// Beat timing data structure for gameplay integration
    /// </summary>
    public struct BeatTimingData
    {
        public double CurrentBeatTime;
        public double NextBeatTime;
        public double BeatDuration;
        public float Tempo;
        public int BeatIndex;
    }
}