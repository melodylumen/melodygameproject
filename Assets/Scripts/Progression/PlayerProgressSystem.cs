using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using RhythmSolitaire.Contracts;
using RhythmSolitaire.Core;

namespace RhythmSolitaire.Progression
{
    /// <summary>
    /// Player Progress System implementation - handles progression and persistence with offline-first design
    /// Constitutional compliance: Offline-first, Modular Architecture
    /// </summary>
    public class PlayerProgressSystem : MonoBehaviour, IPlayerProgressSystem
    {
        #region Constitutional Requirements

        private const float LOAD_PROFILE_MAX_DURATION_MS = 500f;
        private const string PLAYER_DATA_FILENAME = "player_progress.json";
        private const string BACKUP_DATA_FILENAME = "player_progress_backup.json";
        private const string LEADERBOARD_CACHE_FILENAME = "leaderboard_cache.json";

        #endregion

        #region Core State

        [SerializeField] private PlayerProfile _currentProfile;
        [SerializeField] private bool _isOnline = false;
        [SerializeField] private bool _autoSaveEnabled = true;
        [SerializeField] private float _autoSaveInterval = 30f; // seconds

        // Cached data for offline operation
        private Dictionary<LevelId, LeaderboardData> _leaderboardCache = new Dictionary<LevelId, LeaderboardData>();
        private Queue<ScoreSubmission> _pendingSubmissions = new Queue<ScoreSubmission>();

        // Content unlock requirements (configured data)
        private readonly Dictionary<ContentId, UnlockRequirement> _unlockRequirements = new Dictionary<ContentId, UnlockRequirement>
        {
            { new ContentId("world_2"), new UnlockRequirement { StarsRequired = 15, ContentType = ContentType.World } },
            { new ContentId("world_3"), new UnlockRequirement { StarsRequired = 30, ContentType = ContentType.World } },
            { new ContentId("world_4"), new UnlockRequirement { StarsRequired = 50, ContentType = ContentType.World } },
            { new ContentId("level_2_1"), new UnlockRequirement { StarsRequired = 5, ContentType = ContentType.Level } },
            { new ContentId("level_2_2"), new UnlockRequirement { StarsRequired = 8, ContentType = ContentType.Level } },
            { new ContentId("challenge_mode"), new UnlockRequirement { StarsRequired = 25, ContentType = ContentType.GameMode } },
        };

        // Auto-save timer
        private float _autoSaveTimer = 0f;

        #endregion

        #region Events (Constitutional: Loose coupling)

        public event Action<ProgressEvent> OnProgressUpdated;
        public event Action<UnlockEvent> OnContentUnlocked;
        public event Action<SyncEvent> OnSyncStatusChanged;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            InitializeProgressSystem();
        }

        private void Update()
        {
            UpdateAutoSave();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus && _autoSaveEnabled)
            {
                SaveCurrentProfile();
            }
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus && _autoSaveEnabled)
            {
                SaveCurrentProfile();
            }
        }

        #endregion

        #region IPlayerProgressSystem Implementation

        /// <summary>
        /// Saves current session progress to local storage
        /// Constitutional requirement: Must work without internet connection
        /// </summary>
        public bool SaveProgress(GameSession session)
        {
            try
            {
                // Update player profile with session data
                var levelId = new LevelId(session.Level?.LevelId ?? "unknown");

                // Update level progress
                if (!_currentProfile.LevelProgress.ContainsKey(levelId))
                {
                    _currentProfile.LevelProgress[levelId] = new LevelProgress
                    {
                        LevelId = levelId,
                        TimesPlayed = 0,
                        FirstCompleted = DateTime.MinValue
                    };
                }

                var progress = _currentProfile.LevelProgress[levelId];
                progress.TimesPlayed++;
                progress.LastPlayed = DateTime.UtcNow;

                // Update best score if improved
                if (session.CurrentScore > progress.BestScore)
                {
                    progress.BestScore = session.CurrentScore;

                    // Calculate star rating
                    var starRating = CalculateStarRating(session.CurrentScore, session.ElapsedTime, session.ComboCount);

                    if (starRating > progress.BestStarRating)
                    {
                        progress.BestStarRating = starRating;

                        // Fire progress event
                        var progressEvent = new ProgressEvent
                        {
                            LevelId = levelId,
                            NewScore = session.CurrentScore,
                            NewStars = starRating,
                            IsNewRecord = true
                        };
                        OnProgressUpdated?.Invoke(progressEvent);
                    }
                }

                // Update completion time if better
                if (session.ElapsedTime < progress.BestTime || progress.BestTime == TimeSpan.Zero)
                {
                    progress.BestTime = session.ElapsedTime;
                }

                // Mark first completion
                if (progress.FirstCompleted == DateTime.MinValue)
                {
                    progress.FirstCompleted = DateTime.UtcNow;
                }

                _currentProfile.LevelProgress[levelId] = progress;

                // Update statistics
                UpdatePlayerStatistics(session);

                // Save to local storage (constitutional: offline-first)
                bool saveSuccess = SaveCurrentProfile();

                if (saveSuccess)
                {
                    Debug.Log($"Progress saved for level {levelId}");
                }

                return saveSuccess;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to save progress: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Loads player profile from local storage
        /// Constitutional requirement: Must complete within <500ms for good UX
        /// </summary>
        public PlayerProfile LoadPlayerProfile()
        {
            var startTime = Time.realtimeSinceStartup;

            try
            {
                string dataPath = GetPlayerDataPath();

                if (File.Exists(dataPath))
                {
                    string jsonData = File.ReadAllText(dataPath);
                    _currentProfile = JsonUtility.FromJson<PlayerProfile>(jsonData);

                    // Initialize collections if null (JsonUtility limitation)
                    if (_currentProfile.LevelProgress == null)
                        _currentProfile.LevelProgress = new Dictionary<LevelId, LevelProgress>();
                    if (_currentProfile.HighScores == null)
                        _currentProfile.HighScores = new Dictionary<LevelId, int>();
                    if (_currentProfile.UnlockedContent == null)
                        _currentProfile.UnlockedContent = new HashSet<ContentId>();
                }
                else
                {
                    // Create new profile
                    _currentProfile = CreateNewPlayerProfile();
                }

                // Constitutional compliance: Validate execution time
                var executionTime = (Time.realtimeSinceStartup - startTime) * 1000f;
                if (executionTime > LOAD_PROFILE_MAX_DURATION_MS)
                {
                    Debug.LogError($"CONSTITUTIONAL VIOLATION: LoadPlayerProfile() took {executionTime}ms, must be <{LOAD_PROFILE_MAX_DURATION_MS}ms");
                }

                Debug.Log($"Player profile loaded in {executionTime:F1}ms");
                return _currentProfile;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to load player profile: {ex.Message}");
                _currentProfile = CreateNewPlayerProfile();
                return _currentProfile;
            }
        }

        /// <summary>
        /// Updates high score if current score qualifies
        /// Constitutional requirement: Must verify score integrity
        /// </summary>
        public bool UpdateHighScore(LevelId levelId, int score, StarRating stars)
        {
            // Validate score integrity
            if (score < 0)
            {
                Debug.LogError("Invalid score: negative values not allowed");
                return false;
            }

            if (!ValidateScoreIntegrity(levelId, score, stars))
            {
                Debug.LogError("Score validation failed: integrity check");
                return false;
            }

            // Check if it's a new high score
            bool isNewHighScore = false;
            if (!_currentProfile.HighScores.ContainsKey(levelId) || _currentProfile.HighScores[levelId] < score)
            {
                _currentProfile.HighScores[levelId] = score;
                isNewHighScore = true;

                // Update last play time
                _currentProfile.LastPlayTime = DateTime.UtcNow;

                Debug.Log($"New high score for {levelId}: {score}");
            }

            return isNewHighScore;
        }

        /// <summary>
        /// Calculates star rating based on score and performance metrics
        /// Constitutional requirement: Must be deterministic across sessions
        /// </summary>
        public StarRating CalculateStarRating(int score, TimeSpan completionTime, int comboCount)
        {
            // Deterministic star rating calculation
            // Base thresholds (these would be configured per level in real implementation)
            int oneStarThreshold = 1000;
            int twoStarThreshold = 2500;
            int threeStarThreshold = 5000;

            // Time bonus (faster completion = higher effective score)
            double timeBonus = Math.Max(0, 300 - completionTime.TotalSeconds) * 10; // Up to 300 bonus points

            // Combo bonus
            double comboBonus = comboCount * 50;

            // Calculate effective score
            double effectiveScore = score + timeBonus + comboBonus;

            // Determine star rating deterministically
            if (effectiveScore >= threeStarThreshold)
                return StarRating.ThreeStar;
            else if (effectiveScore >= twoStarThreshold)
                return StarRating.TwoStar;
            else if (effectiveScore >= oneStarThreshold)
                return StarRating.OneStar;
            else
                return StarRating.None;
        }

        /// <summary>
        /// Checks if player has enough stars to unlock content
        /// Constitutional requirement: Enforces unlock requirements
        /// </summary>
        public bool CanUnlockContent(ContentId contentId)
        {
            if (!_unlockRequirements.ContainsKey(contentId))
            {
                // Content doesn't require unlocking
                return true;
            }

            var requirement = _unlockRequirements[contentId];
            int totalStars = GetTotalStarsEarned();

            return totalStars >= requirement.StarsRequired;
        }

        /// <summary>
        /// Unlocks new level or world based on star requirements
        /// Constitutional requirement: Must verify star count before unlocking
        /// </summary>
        public bool UnlockContent(ContentId contentId)
        {
            // Check if already unlocked
            if (_currentProfile.UnlockedContent.Contains(contentId))
            {
                return true;
            }

            // Verify star requirements
            if (!CanUnlockContent(contentId))
            {
                var requirement = _unlockRequirements[contentId];
                Debug.Log($"Cannot unlock {contentId}: need {requirement.StarsRequired} stars, have {GetTotalStarsEarned()}");
                return false;
            }

            // Unlock the content
            _currentProfile.UnlockedContent.Add(contentId);

            // Fire unlock event
            var unlockEvent = new UnlockEvent
            {
                ContentId = contentId,
                ContentName = contentId.Value,
                StarsRequired = _unlockRequirements.ContainsKey(contentId) ? _unlockRequirements[contentId].StarsRequired : 0,
                StarsEarned = GetTotalStarsEarned()
            };
            OnContentUnlocked?.Invoke(unlockEvent);

            Debug.Log($"Unlocked content: {contentId}");
            return true;
        }

        /// <summary>
        /// Synchronizes local progress with cloud storage
        /// Constitutional requirement: Must gracefully handle offline scenarios
        /// </summary>
        public SyncResult SynchronizeWithCloud()
        {
            try
            {
                // Check internet connectivity (simplified)
                if (!_isOnline)
                {
                    var offlineEvent = new SyncEvent
                    {
                        Result = SyncResult.Offline,
                        Message = "No internet connection available",
                        Timestamp = DateTime.UtcNow
                    };
                    OnSyncStatusChanged?.Invoke(offlineEvent);
                    return SyncResult.Offline;
                }

                // In a real implementation, this would sync with cloud service
                // For now, simulate successful sync
                _currentProfile.LastSyncTime = DateTime.UtcNow;

                var syncEvent = new SyncEvent
                {
                    Result = SyncResult.Success,
                    Message = "Successfully synchronized with cloud",
                    Timestamp = DateTime.UtcNow
                };
                OnSyncStatusChanged?.Invoke(syncEvent);

                Debug.Log("Cloud sync completed successfully");
                return SyncResult.Success;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Cloud sync failed: {ex.Message}");

                var failureEvent = new SyncEvent
                {
                    Result = SyncResult.Failed,
                    Message = ex.Message,
                    Timestamp = DateTime.UtcNow
                };
                OnSyncStatusChanged?.Invoke(failureEvent);

                return SyncResult.Failed;
            }
        }

        /// <summary>
        /// Gets leaderboard data for a specific level
        /// Constitutional requirement: Returns cached data if offline
        /// </summary>
        public LeaderboardData GetLeaderboard(LevelId levelId, LeaderboardType type)
        {
            // Try to get from cache first (offline support)
            if (_leaderboardCache.ContainsKey(levelId))
            {
                var cachedData = _leaderboardCache[levelId];
                cachedData.IsOnline = _isOnline;
                return cachedData;
            }

            // Return empty leaderboard if not cached
            return new LeaderboardData
            {
                LevelId = levelId,
                Entries = new LeaderboardEntry[0],
                PlayerRank = 0,
                LastUpdated = DateTime.MinValue,
                IsOnline = false
            };
        }

        /// <summary>
        /// Submits score to online leaderboard
        /// Constitutional requirement: Must queue for later if offline
        /// </summary>
        public bool SubmitScore(LevelId levelId, ScoreSubmission submission)
        {
            if (!_isOnline)
            {
                // Queue for later submission when online
                _pendingSubmissions.Enqueue(submission);
                Debug.Log($"Score queued for later submission: {submission.Score}");
                return true;
            }

            try
            {
                // In a real implementation, this would submit to online service
                // For now, simulate successful submission
                Debug.Log($"Score submitted to leaderboard: {submission.Score}");
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to submit score: {ex.Message}");
                _pendingSubmissions.Enqueue(submission);
                return false;
            }
        }

        /// <summary>
        /// Exports player data for backup or transfer
        /// Constitutional requirement: Supports data portability
        /// </summary>
        public string ExportPlayerData()
        {
            try
            {
                var exportData = new PlayerDataExport
                {
                    Profile = _currentProfile,
                    ExportVersion = "1.0",
                    ExportedAt = DateTime.UtcNow,
                    GameVersion = Application.version
                };

                return JsonUtility.ToJson(exportData, true);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to export player data: {ex.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// Imports player data from backup
        /// Constitutional requirement: Must verify data integrity
        /// </summary>
        public bool ImportPlayerData(string backupData)
        {
            try
            {
                if (string.IsNullOrEmpty(backupData))
                {
                    Debug.LogError("Import failed: backup data is empty");
                    return false;
                }

                var importData = JsonUtility.FromJson<PlayerDataExport>(backupData);

                // Validate data integrity
                if (!ValidateImportData(importData))
                {
                    Debug.LogError("Import failed: data integrity validation failed");
                    return false;
                }

                // Create backup of current data
                CreateBackup();

                // Import the data
                _currentProfile = importData.Profile;

                // Save imported data
                bool saveSuccess = SaveCurrentProfile();

                if (saveSuccess)
                {
                    Debug.Log("Player data imported successfully");
                }

                return saveSuccess;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to import player data: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Initializes the progress system with constitutional compliance
        /// </summary>
        private void InitializeProgressSystem()
        {
            // Load player profile
            LoadPlayerProfile();

            // Check internet connectivity (simplified)
            _isOnline = Application.internetReachability != NetworkReachability.NotReachable;

            Debug.Log($"PlayerProgressSystem initialized (Online: {_isOnline})");
        }

        /// <summary>
        /// Creates a new player profile with default values
        /// </summary>
        private PlayerProfile CreateNewPlayerProfile()
        {
            var newProfile = new PlayerProfile
            {
                PlayerId = System.Guid.NewGuid().ToString(),
                PlayerName = "Player",
                LevelProgress = new Dictionary<LevelId, LevelProgress>(),
                HighScores = new Dictionary<LevelId, int>(),
                UnlockedContent = new HashSet<ContentId>(),
                Statistics = new PlayerStatistics
                {
                    PowerCardUsage = new Dictionary<PowerCardType, int>()
                },
                Preferences = new AccessibilityPreferences
                {
                    AudioVolumeMultiplier = 1f,
                    TimingToleranceAdjustment = 0f
                },
                LastPlayTime = DateTime.UtcNow,
                LastSyncTime = DateTime.MinValue
            };

            // Unlock default content
            newProfile.UnlockedContent.Add(new ContentId("world_1"));
            newProfile.UnlockedContent.Add(new ContentId("level_1_1"));

            return newProfile;
        }

        /// <summary>
        /// Saves current profile to local storage
        /// </summary>
        private bool SaveCurrentProfile()
        {
            try
            {
                string dataPath = GetPlayerDataPath();
                string jsonData = JsonUtility.ToJson(_currentProfile, true);
                File.WriteAllText(dataPath, jsonData);
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to save player profile: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Gets the file path for player data storage
        /// </summary>
        private string GetPlayerDataPath()
        {
            return Path.Combine(Application.persistentDataPath, PLAYER_DATA_FILENAME);
        }

        /// <summary>
        /// Updates player statistics based on session data
        /// </summary>
        private void UpdatePlayerStatistics(GameSession session)
        {
            var stats = _currentProfile.Statistics;
            stats.TotalGamesPlayed++;
            stats.TotalPlayTime += session.ElapsedTime;

            // Calculate stars for this session
            var sessionStars = CalculateStarRating(session.CurrentScore, session.ElapsedTime, session.ComboCount);
            stats.TotalStarsEarned += (int)sessionStars;

            _currentProfile.Statistics = stats;
        }

        /// <summary>
        /// Gets total stars earned across all levels
        /// </summary>
        private int GetTotalStarsEarned()
        {
            int totalStars = 0;
            foreach (var progress in _currentProfile.LevelProgress.Values)
            {
                totalStars += (int)progress.BestStarRating;
            }
            return totalStars;
        }

        /// <summary>
        /// Validates score integrity to prevent cheating
        /// </summary>
        private bool ValidateScoreIntegrity(LevelId levelId, int score, StarRating stars)
        {
            // Basic validation - in real implementation would be more sophisticated
            if (score < 0) return false;
            if (score > 999999) return false; // Reasonable maximum

            // Validate star rating matches score roughly
            var calculatedStars = CalculateStarRating(score, TimeSpan.FromMinutes(5), 10);
            return (int)stars <= (int)calculatedStars + 1; // Allow some tolerance
        }

        /// <summary>
        /// Validates imported data integrity
        /// </summary>
        private bool ValidateImportData(PlayerDataExport importData)
        {
            if (importData.Profile.PlayerId == null) return false;
            if (importData.ExportVersion == null) return false;
            if (importData.ExportedAt > DateTime.UtcNow.AddDays(1)) return false; // Future dates not allowed

            return true;
        }

        /// <summary>
        /// Creates a backup of current player data
        /// </summary>
        private void CreateBackup()
        {
            try
            {
                string backupPath = Path.Combine(Application.persistentDataPath, BACKUP_DATA_FILENAME);
                string currentData = JsonUtility.ToJson(_currentProfile, true);
                File.WriteAllText(backupPath, currentData);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to create backup: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates auto-save timer and saves if needed
        /// </summary>
        private void UpdateAutoSave()
        {
            if (!_autoSaveEnabled) return;

            _autoSaveTimer += Time.deltaTime;
            if (_autoSaveTimer >= _autoSaveInterval)
            {
                _autoSaveTimer = 0f;
                SaveCurrentProfile();
            }
        }

        #endregion
    }

    #region Supporting Data Structures

    /// <summary>
    /// Unlock requirement configuration
    /// </summary>
    [System.Serializable]
    public struct UnlockRequirement
    {
        public int StarsRequired;
        public ContentType ContentType;
    }

    /// <summary>
    /// Content type enumeration
    /// </summary>
    public enum ContentType
    {
        Level,
        World,
        GameMode,
        PowerCard
    }

    /// <summary>
    /// Player data export structure for backup/transfer
    /// </summary>
    [System.Serializable]
    public struct PlayerDataExport
    {
        public PlayerProfile Profile;
        public string ExportVersion;
        public DateTime ExportedAt;
        public string GameVersion;
    }

    #endregion
}