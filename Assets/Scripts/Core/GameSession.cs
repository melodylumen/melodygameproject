using System;
using System.Collections.Generic;
using UnityEngine;
using RhythmSolitaire.Cards;
using RhythmSolitaire.Audio;
using RhythmSolitaire.Progression;

namespace RhythmSolitaire.Core
{
    /// <summary>
    /// Game Session entity model - represents a single playthrough instance
    /// Constitutional compliance: Modular Architecture, Real-Time Performance
    /// </summary>
    [System.Serializable]
    public class GameSession : ScriptableObject
    {
        #region Core Fields

        [SerializeField]
        private string _sessionId;

        [SerializeField]
        private int _currentScore;

        [SerializeField]
        private int _comboCount;

        [SerializeField]
        private CardLayoutState _cardLayoutState;

        [SerializeField]
        private MusicProgressionState _musicProgression;

        [SerializeField]
        private DateTime _startTime;

        [SerializeField]
        private bool _isActive;

        [SerializeField]
        private DifficultyLevel _difficulty;

        [SerializeField]
        private GameSessionState _currentState;

        #endregion

        #region Properties

        /// <summary>
        /// Unique identifier for this game session
        /// </summary>
        public string SessionId
        {
            get => _sessionId;
            private set => _sessionId = value;
        }

        /// <summary>
        /// Real-time score accumulation
        /// Constitutional requirement: Must update without allocation
        /// </summary>
        public int CurrentScore
        {
            get => _currentScore;
            set
            {
                var oldScore = _currentScore;
                _currentScore = value;
                OnScoreChanged?.Invoke(oldScore, value);
            }
        }

        /// <summary>
        /// Current combo chain length for rhythm scoring
        /// </summary>
        public int ComboCount
        {
            get => _comboCount;
            set
            {
                var oldCombo = _comboCount;
                _comboCount = Mathf.Max(0, value); // Ensure non-negative
                OnComboChanged?.Invoke(oldCombo, _comboCount);
            }
        }

        /// <summary>
        /// Current positions and availability of all cards
        /// </summary>
        public CardLayoutState CardLayoutState
        {
            get => _cardLayoutState;
            set => _cardLayoutState = value;
        }

        /// <summary>
        /// Current music layers and tempo state
        /// </summary>
        public MusicProgressionState MusicProgression
        {
            get => _musicProgression;
            set => _musicProgression = value;
        }

        /// <summary>
        /// Session start timestamp for analytics and timing
        /// </summary>
        public DateTime StartTime
        {
            get => _startTime;
            private set => _startTime = value;
        }

        /// <summary>
        /// Whether session is currently playable
        /// </summary>
        public bool IsActive
        {
            get => _isActive;
            private set => _isActive = value;
        }

        /// <summary>
        /// Selected difficulty level affecting timing tolerance
        /// </summary>
        public DifficultyLevel Difficulty
        {
            get => _difficulty;
            private set => _difficulty = value;
        }

        /// <summary>
        /// Current session state for state machine management
        /// </summary>
        public GameSessionState CurrentState
        {
            get => _currentState;
            private set => _currentState = value;
        }

        /// <summary>
        /// Elapsed time since session start
        /// </summary>
        public TimeSpan ElapsedTime => DateTime.UtcNow - _startTime;

        #endregion

        #region Relationships

        /// <summary>
        /// Reference to the player profile (has one relationship)
        /// </summary>
        public PlayerProfile PlayerProfile { get; set; }

        /// <summary>
        /// Collection of cards in this session (contains multiple)
        /// </summary>
        public List<Card> Cards { get; private set; } = new List<Card>();

        /// <summary>
        /// Reference to the music track (references one)
        /// </summary>
        public MusicTrack MusicTrack { get; set; }

        /// <summary>
        /// Available power cards in this session (may contain multiple)
        /// </summary>
        public List<PowerCard> PowerCards { get; private set; } = new List<PowerCard>();

        /// <summary>
        /// Reference to the level/world (belongs to one)
        /// </summary>
        public Level Level { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// Fired when score changes (oldScore, newScore)
        /// Constitutional requirement: Must not allocate in audio thread
        /// </summary>
        public event Action<int, int> OnScoreChanged;

        /// <summary>
        /// Fired when combo count changes (oldCombo, newCombo)
        /// </summary>
        public event Action<int, int> OnComboChanged;

        /// <summary>
        /// Fired when session state transitions
        /// </summary>
        public event Action<GameSessionState, GameSessionState> OnStateChanged;

        #endregion

        #region Public Methods

        /// <summary>
        /// Initializes a new game session
        /// Constitutional requirement: Deterministic initialization for testing
        /// </summary>
        public static GameSession CreateSession(string sessionId, DifficultyLevel difficulty, Level level)
        {
            var session = CreateInstance<GameSession>();
            session.SessionId = sessionId;
            session.Difficulty = difficulty;
            session.Level = level;
            session.StartTime = DateTime.UtcNow;
            session.CurrentState = GameSessionState.Created;
            session.IsActive = false;

            return session;
        }

        /// <summary>
        /// Starts the game session
        /// State transition: CREATED → ACTIVE
        /// </summary>
        public void StartSession()
        {
            if (CurrentState != GameSessionState.Created)
            {
                Debug.LogWarning($"Cannot start session from state {CurrentState}");
                return;
            }

            TransitionToState(GameSessionState.Active);
            IsActive = true;
            StartTime = DateTime.UtcNow;
        }

        /// <summary>
        /// Pauses the active session
        /// State transition: ACTIVE → PAUSED
        /// </summary>
        public void PauseSession()
        {
            if (CurrentState != GameSessionState.Active)
            {
                Debug.LogWarning($"Cannot pause session from state {CurrentState}");
                return;
            }

            TransitionToState(GameSessionState.Paused);
            IsActive = false;

            // Constitutional requirement: Pausing breaks combo chain
            ComboCount = 0;
        }

        /// <summary>
        /// Resumes the paused session
        /// State transition: PAUSED → ACTIVE
        /// </summary>
        public void ResumeSession()
        {
            if (CurrentState != GameSessionState.Paused)
            {
                Debug.LogWarning($"Cannot resume session from state {CurrentState}");
                return;
            }

            TransitionToState(GameSessionState.Active);
            IsActive = true;
        }

        /// <summary>
        /// Completes the session successfully
        /// State transition: ACTIVE → COMPLETED
        /// </summary>
        public void CompleteSession()
        {
            if (CurrentState != GameSessionState.Active)
            {
                Debug.LogWarning($"Cannot complete session from state {CurrentState}");
                return;
            }

            TransitionToState(GameSessionState.Completed);
            IsActive = false;
        }

        /// <summary>
        /// Fails the session (no valid moves)
        /// State transition: ACTIVE → FAILED
        /// </summary>
        public void FailSession()
        {
            if (CurrentState != GameSessionState.Active)
            {
                Debug.LogWarning($"Cannot fail session from state {CurrentState}");
                return;
            }

            TransitionToState(GameSessionState.Failed);
            IsActive = false;
        }

        /// <summary>
        /// Updates the session with timing-sensitive operations
        /// Constitutional requirement: Must complete quickly for real-time performance
        /// </summary>
        public void UpdateSession(float deltaTime)
        {
            if (!IsActive) return;

            // Update music progression based on current state
            if (MusicProgression != null)
            {
                MusicProgression.Update(deltaTime);
            }

            // Validate constitutional compliance
            ConstitutionalCompliance.ValidateFrameRate();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Handles state transitions with validation
        /// </summary>
        private void TransitionToState(GameSessionState newState)
        {
            var oldState = CurrentState;

            // Validate state transition
            if (!IsValidStateTransition(oldState, newState))
            {
                Debug.LogError($"Invalid state transition from {oldState} to {newState}");
                return;
            }

            CurrentState = newState;
            OnStateChanged?.Invoke(oldState, newState);

            Debug.Log($"Session {SessionId} transitioned from {oldState} to {newState}");
        }

        /// <summary>
        /// Validates whether a state transition is allowed
        /// </summary>
        private bool IsValidStateTransition(GameSessionState from, GameSessionState to)
        {
            return (from, to) switch
            {
                (GameSessionState.Created, GameSessionState.Active) => true,
                (GameSessionState.Active, GameSessionState.Paused) => true,
                (GameSessionState.Paused, GameSessionState.Active) => true,
                (GameSessionState.Active, GameSessionState.Completed) => true,
                (GameSessionState.Active, GameSessionState.Failed) => true,
                _ => false
            };
        }

        #endregion
    }

    /// <summary>
    /// Session state enumeration for state machine management
    /// </summary>
    public enum GameSessionState
    {
        Created,
        Active,
        Paused,
        Completed,
        Failed
    }

    /// <summary>
    /// Card layout state data structure
    /// </summary>
    [System.Serializable]
    public class CardLayoutState
    {
        public Vector2Int[] CardPositions;
        public bool[] CardAvailability;
        public bool[] CardRevealed;
        public int[] StackPositions;
    }

    /// <summary>
    /// Music progression state data structure
    /// </summary>
    [System.Serializable]
    public class MusicProgressionState
    {
        public float CurrentTempo;
        public int ActiveLayers;
        public float[] LayerVolumes;
        public bool IsPlaying;

        public void Update(float deltaTime)
        {
            // Update music progression logic
            // Constitutional requirement: No allocation during update
        }
    }
}