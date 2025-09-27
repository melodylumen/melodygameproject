// Player Progress System Interface Contract
// Supports FR-010, FR-012, FR-013
// Constitutional compliance: Offline-first, Modular Architecture

using System;
using System.Collections.Generic;

namespace RhythmSolitaire.Contracts
{
    /// <summary>
    /// Player progress and persistence interface
    /// CONSTITUTIONAL REQUIREMENT: Offline-first with optional cloud sync
    /// </summary>
    public interface IPlayerProgressSystem
    {
        // FR-010: Persist player progress, high scores, and unlocked content
        /// <summary>
        /// Saves current session progress to local storage
        /// OFFLINE: Must work without internet connection
        /// </summary>
        bool SaveProgress(GameSession session);

        /// <summary>
        /// Loads player profile from local storage
        /// STARTUP: Must complete within <500ms for good UX
        /// </summary>
        PlayerProfile LoadPlayerProfile();

        /// <summary>
        /// Updates high score if current score qualifies
        /// VALIDATION: Must verify score integrity
        /// </summary>
        bool UpdateHighScore(LevelId levelId, int score, StarRating stars);

        // FR-012: Star-based progression system
        /// <summary>
        /// Calculates star rating based on score and performance metrics
        /// CONSISTENCY: Must be deterministic across sessions
        /// </summary>
        StarRating CalculateStarRating(int score, TimeSpan completionTime, int comboCount);

        /// <summary>
        /// Checks if player has enough stars to unlock content
        /// PROGRESSION: Enforces unlock requirements
        /// </summary>
        bool CanUnlockContent(ContentId contentId);

        /// <summary>
        /// Unlocks new level or world based on star requirements
        /// VALIDATION: Must verify star count before unlocking
        /// </summary>
        bool UnlockContent(ContentId contentId);

        // FR-013: Optional online sync for leaderboards and cloud saves
        /// <summary>
        /// Synchronizes local progress with cloud storage
        /// OPTIONAL: Must gracefully handle offline scenarios
        /// </summary>
        SyncResult SynchronizeWithCloud();

        /// <summary>
        /// Gets leaderboard data for a specific level
        /// ONLINE: Returns cached data if offline
        /// </summary>
        LeaderboardData GetLeaderboard(LevelId levelId, LeaderboardType type);

        /// <summary>
        /// Submits score to online leaderboard
        /// OPTIONAL: Must queue for later if offline
        /// </summary>
        bool SubmitScore(LevelId levelId, ScoreSubmission submission);

        /// <summary>
        /// Exports player data for backup or transfer
        /// ACCESSIBILITY: Supports data portability
        /// </summary>
        string ExportPlayerData();

        /// <summary>
        /// Imports player data from backup
        /// VALIDATION: Must verify data integrity
        /// </summary>
        bool ImportPlayerData(string backupData);

        // Events for UI updates
        event Action<ProgressEvent> OnProgressUpdated;
        event Action<UnlockEvent> OnContentUnlocked;
        event Action<SyncEvent> OnSyncStatusChanged;
    }

    public struct PlayerProfile
    {
        public string PlayerId;
        public string PlayerName;
        public Dictionary<LevelId, LevelProgress> LevelProgress;
        public Dictionary<LevelId, int> HighScores;
        public HashSet<ContentId> UnlockedContent;
        public PlayerStatistics Statistics;
        public AccessibilityPreferences Preferences;
        public DateTime LastPlayTime;
        public DateTime LastSyncTime;
    }

    public struct LevelProgress
    {
        public LevelId LevelId;
        public StarRating BestStarRating;
        public int BestScore;
        public TimeSpan BestTime;
        public int TimesPlayed;
        public DateTime FirstCompleted;
        public DateTime LastPlayed;
    }

    public struct PlayerStatistics
    {
        public int TotalGamesPlayed;
        public int TotalStarsEarned;
        public TimeSpan TotalPlayTime;
        public int PerfectRhythmMoves;
        public int MaxComboAchieved;
        public float AverageRhythmAccuracy;
        public Dictionary<PowerCardType, int> PowerCardUsage;
    }

    public struct AccessibilityPreferences
    {
        public bool VisualBeatIndicatorsEnabled;
        public float AudioVolumeMultiplier;
        public bool HighContrastMode;
        public bool ReducedMotionMode;
        public float TimingToleranceAdjustment;
        public bool VibrationEnabled;
    }

    public struct ScoreSubmission
    {
        public int Score;
        public StarRating Stars;
        public TimeSpan CompletionTime;
        public int ComboCount;
        public float RhythmAccuracy;
        public DateTime AchievedAt;
        public string PlayerName;
    }

    public struct LeaderboardData
    {
        public LevelId LevelId;
        public LeaderboardEntry[] Entries;
        public int PlayerRank;
        public DateTime LastUpdated;
        public bool IsOnline;
    }

    public struct LeaderboardEntry
    {
        public string PlayerName;
        public int Score;
        public StarRating Stars;
        public TimeSpan CompletionTime;
        public DateTime AchievedAt;
    }

    public enum StarRating
    {
        None = 0,
        OneStar = 1,
        TwoStar = 2,
        ThreeStar = 3
    }

    public enum LeaderboardType
    {
        HighScore,
        FastestTime,
        BestRhythm,
        MostStars
    }

    public enum SyncResult
    {
        Success,
        Failed,
        Offline,
        Conflict,
        Unauthorized
    }

    // Event data structures
    public struct ProgressEvent
    {
        public LevelId LevelId;
        public int NewScore;
        public StarRating NewStars;
        public bool IsNewRecord;
    }

    public struct UnlockEvent
    {
        public ContentId ContentId;
        public string ContentName;
        public int StarsRequired;
        public int StarsEarned;
    }

    public struct SyncEvent
    {
        public SyncResult Result;
        public string Message;
        public DateTime Timestamp;
    }

    // ID types for strong typing
    public readonly struct LevelId
    {
        public readonly string Value;
        public LevelId(string value) => Value = value;
        public override string ToString() => Value;
    }

    public readonly struct ContentId
    {
        public readonly string Value;
        public ContentId(string value) => Value = value;
        public override string ToString() => Value;
    }
}