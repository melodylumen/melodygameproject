// Card Game Engine Interface Contract
// Supports FR-001, FR-005, FR-009
// Constitutional compliance: Modular Architecture, Test-Driven Development

using System;
using System.Collections.Generic;
using UnityEngine;

namespace RhythmSolitaire.Contracts
{
    /// <summary>
    /// Card game logic interface for solitaire mechanics
    /// CONSTITUTIONAL REQUIREMENT: Independently testable, loosely coupled
    /// </summary>
    public interface ICardGameEngine
    {
        // FR-001: Solitaire-style card layouts
        /// <summary>
        /// Initializes card layout for a new game session
        /// TESTING: Must be deterministic for automated testing
        /// </summary>
        CardLayout InitializeLayout(LevelConfiguration config);

        /// <summary>
        /// Gets all currently available card moves
        /// PERFORMANCE: Must complete within <5ms for 60fps requirement
        /// </summary>
        IReadOnlyList<CardMove> GetValidMoves();

        /// <summary>
        /// Executes a card move and returns the result
        /// VALIDATION: Must enforce traditional solitaire rules
        /// </summary>
        MoveResult ExecuteMove(CardMove move);

        // FR-005: Score calculation
        /// <summary>
        /// Calculates score for a move considering both strategy and timing
        /// MODULARITY: Score calculation separate from move execution
        /// </summary>
        ScoreData CalculateMoveScore(CardMove move, TimingAccuracy timing, int currentCombo);

        // FR-009: Different game modes
        /// <summary>
        /// Configures game rules based on selected mode
        /// TESTABILITY: Each mode must be independently testable
        /// </summary>
        void ConfigureGameMode(GameMode mode, DifficultyLevel difficulty);

        /// <summary>
        /// Checks if the current game state meets win conditions
        /// DETERMINISM: Must be predictable for automated testing
        /// </summary>
        GameState CheckGameState();

        /// <summary>
        /// Gets current card layout state for persistence
        /// SERIALIZATION: Must support save/load operations
        /// </summary>
        CardLayoutState GetCurrentState();

        /// <summary>
        /// Restores card layout from saved state
        /// VALIDATION: Must verify state integrity
        /// </summary>
        bool RestoreState(CardLayoutState savedState);

        // Events for loose coupling
        event Action<CardMove> OnMoveExecuted;
        event Action<GameState> OnGameStateChanged;
        event Action<ScoreData> OnScoreUpdated;
    }

    public struct CardLayout
    {
        public Card[] TableauCards;
        public Card[] FoundationCards;
        public Card[] StockCards;
        public Card[] WasteCards;
        public int TableauColumns;
    }

    public struct Card
    {
        public int CardId;
        public Suit Suit;
        public Rank Rank;
        public Position Position;
        public bool IsRevealed;
        public bool IsAvailable;
    }

    public struct CardMove
    {
        public int SourceCardId;
        public Position TargetPosition;
        public MoveType Type;
        public DateTime RequestTime;
    }

    public struct MoveResult
    {
        public bool IsValid;
        public string ValidationMessage;
        public Card[] AffectedCards;
        public bool TriggeredFoundationComplete;
    }

    public struct ScoreData
    {
        public int BasePoints;
        public int TimingBonus;
        public int ComboMultiplier;
        public int TotalScore;
        public StarRating StarEarned;
    }

    public struct Position
    {
        public int Column;
        public int Row;
        public PileType Pile;
    }

    public struct CardLayoutState
    {
        public byte[] TableauState;
        public byte[] FoundationState;
        public byte[] StockState;
        public byte[] WasteState;
        public GameMode GameMode;
        public DifficultyLevel Difficulty;
        public ScoreData CurrentScore;
    }

    public class LevelConfiguration
    {
        public GameMode GameMode;
        public DifficultyLevel Difficulty;
        public int CardCount = 52;
        public bool ShuffleCards = true;
    }

    public enum GameMode
    {
        Campaign,
        Endless,
        Challenge,
        Tutorial
    }

    public enum GameState
    {
        InProgress,
        Won,
        Lost,
        NoMovesAvailable
    }

    public enum MoveType
    {
        TableauToTableau,
        TableauToFoundation,
        StockToWaste,
        WasteToTableau,
        WasteToFoundation
    }

    public enum Suit
    {
        Hearts,
        Diamonds,
        Clubs,
        Spades
    }

    public enum Rank
    {
        Ace = 1,
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8,
        Nine = 9,
        Ten = 10,
        Jack = 11,
        Queen = 12,
        King = 13
    }

    public enum PileType
    {
        Tableau,
        Foundation,
        Stock,
        Waste
    }

    public enum StarRating
    {
        NoStar,
        OneStar,
        TwoStar,
        ThreeStar
    }
}