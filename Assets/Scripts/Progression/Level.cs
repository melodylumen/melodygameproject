using System;
using UnityEngine;
using RhythmSolitaire.Audio;

namespace RhythmSolitaire.Progression
{
    [CreateAssetMenu(fileName = "NewLevel", menuName = "Rhythm Solitaire/Level")]
    public class Level : ScriptableObject
    {
        [SerializeField] private string _levelId;
        [SerializeField] private string _worldId;
        [SerializeField] private DifficultySettings _difficultySettings;
        [SerializeField] private MusicGenre _musicTheme;
        [SerializeField] private CompletionCriteria _completionCriteria;
        [SerializeField] private int _unlockRequirement;
        [SerializeField] private CardLayoutPattern _cardLayout;

        public string LevelId => _levelId;
        public string WorldId => _worldId;
        public DifficultySettings DifficultySettings => _difficultySettings;
        public MusicGenre MusicTheme => _musicTheme;
        public CompletionCriteria CompletionCriteria => _completionCriteria;
        public int UnlockRequirement => _unlockRequirement;
        public CardLayoutPattern CardLayout => _cardLayout;

        public static Level CreateLevel(string levelId, string worldId, DifficultyLevel difficulty)
        {
            var level = CreateInstance<Level>();
            level._levelId = levelId;
            level._worldId = worldId;
            level._difficultySettings = new DifficultySettings { Difficulty = difficulty };
            return level;
        }
    }

    [System.Serializable]
    public class DifficultySettings
    {
        public DifficultyLevel Difficulty;
        public float TimingTolerance;
        public int CardComplexity;
    }

    [System.Serializable]
    public class CompletionCriteria
    {
        public int OneStarScore;
        public int TwoStarScore;
        public int ThreeStarScore;
    }

    [System.Serializable]
    public class CardLayoutPattern
    {
        public Vector2Int[] InitialPositions;
        public bool[] InitialRevealed;
    }

    public enum DifficultyLevel
    {
        Easy,
        Normal,
        Hard,
        Expert
    }
}