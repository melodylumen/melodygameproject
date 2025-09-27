using System;
using System.Collections.Generic;
using UnityEngine;

namespace RhythmSolitaire.Progression
{
    /// <summary>
    /// Player Profile entity model - represents user progress and personalization data
    /// Constitutional compliance: Offline-first, Modular Architecture
    /// </summary>
    [CreateAssetMenu(fileName = "NewPlayerProfile", menuName = "Rhythm Solitaire/Player Profile")]
    public class PlayerProfile : ScriptableObject
    {
        #region Core Fields

        [SerializeField]
        private string _playerId;

        [SerializeField]
        private Dictionary<string, int> _highScores = new Dictionary<string, int>();

        [SerializeField]
        private HashSet<string> _unlockedContent = new HashSet<string>();

        [SerializeField]
        private AccessibilityPreferences _accessibilityPreferences;

        [SerializeField]
        private PlayStatistics _playStatistics;

        [SerializeField]
        private DateTime _lastSyncTime;

        [SerializeField]
        private bool _isCloudSyncEnabled;

        #endregion

        #region Properties

        public string PlayerId
        {
            get => _playerId;
            private set => _playerId = value;
        }

        public Dictionary<string, int> HighScores
        {
            get => _highScores;
            private set => _highScores = value;
        }

        public HashSet<string> UnlockedContent
        {
            get => _unlockedContent;
            private set => _unlockedContent = value;
        }

        public AccessibilityPreferences AccessibilityPreferences
        {
            get => _accessibilityPreferences;
            set => _accessibilityPreferences = value;
        }

        public PlayStatistics PlayStatistics
        {
            get => _playStatistics;
            set => _playStatistics = value;
        }

        public DateTime LastSyncTime
        {
            get => _lastSyncTime;
            private set => _lastSyncTime = value;
        }

        public bool IsCloudSyncEnabled
        {
            get => _isCloudSyncEnabled;
            set => _isCloudSyncEnabled = value;
        }

        public int TotalStars => CalculateTotalStars();

        #endregion

        #region Relationships

        public List<Core.GameSession> GameSessions { get; private set; } = new List<Core.GameSession>();
        public List<Level> UnlockedLevels { get; private set; } = new List<Level>();
        public Dictionary<string, MusicGenre> MusicPreferences { get; private set; } = new Dictionary<string, MusicGenre>();

        #endregion

        #region Events

        public event Action<string, int, int> OnHighScoreUpdated;
        public event Action<string> OnContentUnlocked;
        public event Action<PlayStatistics> OnStatisticsUpdated;

        #endregion

        #region Public Methods

        public static PlayerProfile CreateProfile(string playerId)
        {
            var profile = CreateInstance<PlayerProfile>();
            profile.PlayerId = playerId;
            profile.AccessibilityPreferences = new AccessibilityPreferences();
            profile.PlayStatistics = new PlayStatistics();
            profile.IsCloudSyncEnabled = false;
            profile.LastSyncTime = DateTime.UtcNow;

            return profile;
        }

        public bool UpdateHighScore(string levelId, int newScore)
        {
            if (!_highScores.ContainsKey(levelId) || _highScores[levelId] < newScore)
            {
                var oldScore = _highScores.ContainsKey(levelId) ? _highScores[levelId] : 0;
                _highScores[levelId] = newScore;
                OnHighScoreUpdated?.Invoke(levelId, oldScore, newScore);
                return true;
            }
            return false;
        }

        public void UnlockContent(string contentId, int starsEarned)
        {
            if (!_unlockedContent.Contains(contentId))
            {
                _unlockedContent.Add(contentId);
                OnContentUnlocked?.Invoke(contentId);
            }
        }

        public bool IsContentUnlocked(string contentId)
        {
            return _unlockedContent.Contains(contentId);
        }

        public void SaveToLocal()
        {
            // Constitutional requirement: Offline-first storage
            var json = JsonUtility.ToJson(this, true);
            var path = GetLocalSavePath();
            System.IO.File.WriteAllText(path, json);
        }

        public static PlayerProfile LoadFromLocal(string playerId)
        {
            var path = GetLocalSavePathForPlayer(playerId);
            if (System.IO.File.Exists(path))
            {
                var json = System.IO.File.ReadAllText(path);
                return JsonUtility.FromJson<PlayerProfile>(json);
            }
            return CreateProfile(playerId);
        }

        #endregion

        #region Private Methods

        private int CalculateTotalStars()
        {
            // Calculate total stars from level completions
            return _unlockedContent.Count; // Simplified calculation
        }

        private string GetLocalSavePath()
        {
            return GetLocalSavePathForPlayer(_playerId);
        }

        private static string GetLocalSavePathForPlayer(string playerId)
        {
            return System.IO.Path.Combine(Application.persistentDataPath, $"player_{playerId}.json");
        }

        #endregion
    }

    [System.Serializable]
    public class AccessibilityPreferences
    {
        public bool EnableVisualBeatIndicators = false;
        public bool EnableColorBlindSupport = false;
        public float AudioVolume = 1.0f;
        public float SFXVolume = 1.0f;
        public TimingToleranceLevel TimingTolerance = TimingToleranceLevel.Normal;
    }

    [System.Serializable]
    public class PlayStatistics
    {
        public int TotalGamesPlayed = 0;
        public TimeSpan TotalPlayTime = TimeSpan.Zero;
        public float AverageScore = 0f;
        public int BestCombo = 0;
        public int PerfectTimingCount = 0;
        public int GoodTimingCount = 0;
        public int MissedTimingCount = 0;
    }

    public enum MusicGenre
    {
        LoFi,
        EDM,
        Orchestral,
        Electronic,
        Ambient
    }

    public enum TimingToleranceLevel
    {
        Strict,
        Normal,
        Relaxed
    }
}