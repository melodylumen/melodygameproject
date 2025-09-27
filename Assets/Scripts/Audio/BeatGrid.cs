using System;
using UnityEngine;
using RhythmSolitaire.Core;

namespace RhythmSolitaire.Audio
{
    /// <summary>
    /// Beat Grid entity model - represents rhythm timing system with precise synchronization
    /// Constitutional compliance: Real-Time Performance, Audio Quality First
    /// </summary>
    public class BeatGrid : MonoBehaviour
    {
        #region Core Fields

        [SerializeField]
        private string _beatGridId;

        [SerializeField]
        private int _currentPosition;

        [SerializeField]
        private float _tempo;

        [SerializeField]
        private AccuracyWindows _accuracyWindows;

        [SerializeField]
        private double _nextBeatTime;

        [SerializeField]
        private float _syncOffset;

        #endregion

        #region Properties

        public string BeatGridId
        {
            get => _beatGridId;
            private set => _beatGridId = value;
        }

        public int CurrentPosition
        {
            get => _currentPosition;
            private set => _currentPosition = value;
        }

        public float Tempo
        {
            get => _tempo;
            set => _tempo = Mathf.Clamp(value, 60f, 200f);
        }

        public AccuracyWindows AccuracyWindows
        {
            get => _accuracyWindows;
            set => _accuracyWindows = value;
        }

        public double NextBeatTime
        {
            get => _nextBeatTime;
            private set => _nextBeatTime = value;
        }

        public float SyncOffset
        {
            get => _syncOffset;
            set => _syncOffset = value;
        }

        public double BeatInterval => 60.0 / _tempo;
        public bool IsOnBeat => Mathf.Abs((float)(AudioSettings.dspTime - _nextBeatTime)) < _accuracyWindows.PerfectWindow;

        #endregion

        #region Relationships

        public MusicTrack MusicTrack { get; set; }
        public Core.GameSession GameSession { get; set; }
        public PowerCards.PowerCard[] PowerCards { get; set; }

        #endregion

        #region Events

        public event Action<int> OnBeatTrigger;
        public event Action<TimingAccuracy> OnTimingValidated;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            if (string.IsNullOrEmpty(_beatGridId))
            {
                _beatGridId = System.Guid.NewGuid().ToString();
            }

            InitializeAccuracyWindows();
        }

        private void Update()
        {
            UpdateBeatGrid();
        }

        #endregion

        #region Public Methods

        public static BeatGrid CreateBeatGrid(string gridId, float tempo)
        {
            var gridGameObject = new GameObject($"BeatGrid_{gridId}");
            var beatGrid = gridGameObject.AddComponent<BeatGrid>();

            beatGrid.BeatGridId = gridId;
            beatGrid.Tempo = tempo;
            beatGrid.CurrentPosition = 0;
            beatGrid.NextBeatTime = AudioSettings.dspTime + beatGrid.BeatInterval;

            return beatGrid;
        }

        public TimingAccuracy ValidateMoveTiming(DateTime moveTime)
        {
            double moveTimeSeconds = ConvertToAudioTime(moveTime);
            double timeDifference = Math.Abs(moveTimeSeconds - _nextBeatTime);

            TimingAccuracy accuracy;
            if (timeDifference <= _accuracyWindows.PerfectWindow)
            {
                accuracy = TimingAccuracy.Perfect;
            }
            else if (timeDifference <= _accuracyWindows.GoodWindow)
            {
                accuracy = TimingAccuracy.Good;
            }
            else
            {
                accuracy = TimingAccuracy.Missed;
            }

            OnTimingValidated?.Invoke(accuracy);

            // Constitutional compliance: Must complete within <10ms
            ConstitutionalCompliance.ValidateAudioLatency((float)(timeDifference * 1000));

            return accuracy;
        }

        public void AdaptTodifficulty(DifficultyLevel difficulty)
        {
            _accuracyWindows = difficulty switch
            {
                DifficultyLevel.Easy => new AccuracyWindows { PerfectWindow = 0.15, GoodWindow = 0.25 },
                DifficultyLevel.Normal => new AccuracyWindows { PerfectWindow = 0.10, GoodWindow = 0.20 },
                DifficultyLevel.Hard => new AccuracyWindows { PerfectWindow = 0.075, GoodWindow = 0.15 },
                DifficultyLevel.Expert => new AccuracyWindows { PerfectWindow = 0.05, GoodWindow = 0.10 },
                _ => new AccuracyWindows { PerfectWindow = 0.10, GoodWindow = 0.20 }
            };
        }

        #endregion

        #region Private Methods

        private void UpdateBeatGrid()
        {
            double currentTime = AudioSettings.dspTime + _syncOffset;

            if (currentTime >= _nextBeatTime)
            {
                _currentPosition++;
                _nextBeatTime += BeatInterval;

                OnBeatTrigger?.Invoke(_currentPosition);

                // Validate constitutional timing accuracy
                ConstitutionalCompliance.ValidateFrameRate();
            }
        }

        private void InitializeAccuracyWindows()
        {
            _accuracyWindows = new AccuracyWindows
            {
                PerfectWindow = 0.10, // 100ms
                GoodWindow = 0.20     // 200ms
            };
        }

        private double ConvertToAudioTime(DateTime systemTime)
        {
            // Convert system DateTime to audio DSP time
            // This is a simplified conversion - real implementation would be more precise
            return AudioSettings.dspTime;
        }

        #endregion
    }

    [System.Serializable]
    public struct AccuracyWindows
    {
        public double PerfectWindow;
        public double GoodWindow;

        public bool IsWithinPerfectWindow(double timeDifference)
        {
            return Math.Abs(timeDifference) <= PerfectWindow;
        }

        public bool IsWithinGoodWindow(double timeDifference)
        {
            return Math.Abs(timeDifference) <= GoodWindow;
        }
    }

}