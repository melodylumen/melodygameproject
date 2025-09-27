using System;
using UnityEngine;
using RhythmSolitaire.Core;

namespace RhythmSolitaire.Cards
{
    /// <summary>
    /// Card entity model - represents individual playing cards with solitaire and rhythm properties
    /// Constitutional compliance: Modular Architecture, Test-Driven Development
    /// </summary>
    [System.Serializable]
    public class Card : MonoBehaviour
    {
        #region Core Fields

        [SerializeField]
        private string _cardId;

        [SerializeField]
        private CardSuit _suit;

        [SerializeField]
        private CardRank _rank;

        [SerializeField]
        private Vector2Int _position;

        [SerializeField]
        private bool _isAvailable;

        [SerializeField]
        private bool _isRevealed;

        [SerializeField]
        private int _stackPosition;

        #endregion

        #region Properties

        /// <summary>
        /// Unique identifier for this card
        /// </summary>
        public string CardId
        {
            get => _cardId;
            private set => _cardId = value;
        }

        /// <summary>
        /// Card suit (Hearts, Diamonds, Clubs, Spades)
        /// </summary>
        public CardSuit Suit
        {
            get => _suit;
            private set => _suit = value;
        }

        /// <summary>
        /// Card rank (Ace, 2-10, Jack, Queen, King)
        /// </summary>
        public CardRank Rank
        {
            get => _rank;
            private set => _rank = value;
        }

        /// <summary>
        /// Current table position coordinates
        /// </summary>
        public Vector2Int Position
        {
            get => _position;
            set
            {
                var oldPosition = _position;
                _position = value;
                OnPositionChanged?.Invoke(this, oldPosition, value);

                // Update Unity transform to match logical position
                UpdateVisualPosition();
            }
        }

        /// <summary>
        /// Whether card can be moved based on solitaire rules
        /// </summary>
        public bool IsAvailable
        {
            get => _isAvailable;
            set
            {
                if (_isAvailable != value)
                {
                    _isAvailable = value;
                    OnAvailabilityChanged?.Invoke(this, value);
                    UpdateVisualAvailability();
                }
            }
        }

        /// <summary>
        /// Whether card is face-up and visible
        /// </summary>
        public bool IsRevealed
        {
            get => _isRevealed;
            set
            {
                if (_isRevealed != value)
                {
                    _isRevealed = value;
                    OnRevealedChanged?.Invoke(this, value);
                    UpdateVisualRevealed();
                }
            }
        }

        /// <summary>
        /// Position within a stack (if applicable)
        /// </summary>
        public int StackPosition
        {
            get => _stackPosition;
            set => _stackPosition = value;
        }

        /// <summary>
        /// Numeric value of the card for game logic
        /// </summary>
        public int NumericValue => GetNumericValue(_rank);

        /// <summary>
        /// Whether the card is red (Hearts or Diamonds)
        /// </summary>
        public bool IsRed => _suit == CardSuit.Hearts || _suit == CardSuit.Diamonds;

        /// <summary>
        /// Whether the card is black (Clubs or Spades)
        /// </summary>
        public bool IsBlack => !IsRed;

        #endregion

        #region Relationships

        /// <summary>
        /// Reference to the game session this card belongs to
        /// </summary>
        public GameSession GameSession { get; set; }

        /// <summary>
        /// Cards that this card is blocking (may block other cards)
        /// </summary>
        public Card[] BlockedCards { get; set; } = new Card[0];

        /// <summary>
        /// Cards that are blocking this card (may be blocked by other cards)
        /// </summary>
        public Card[] BlockingCards { get; set; } = new Card[0];

        #endregion

        #region Events

        /// <summary>
        /// Fired when card position changes (card, oldPosition, newPosition)
        /// Constitutional requirement: Must not allocate in performance-critical paths
        /// </summary>
        public event Action<Card, Vector2Int, Vector2Int> OnPositionChanged;

        /// <summary>
        /// Fired when availability changes (card, isAvailable)
        /// </summary>
        public event Action<Card, bool> OnAvailabilityChanged;

        /// <summary>
        /// Fired when reveal state changes (card, isRevealed)
        /// </summary>
        public event Action<Card, bool> OnRevealedChanged;

        /// <summary>
        /// Fired when card is selected for move (card)
        /// </summary>
        public event Action<Card> OnCardSelected;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            // Initialize card with default state
            if (string.IsNullOrEmpty(_cardId))
            {
                _cardId = System.Guid.NewGuid().ToString();
            }
        }

        private void Start()
        {
            // Ensure visual state matches logical state
            UpdateVisualPosition();
            UpdateVisualAvailability();
            UpdateVisualRevealed();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a new card with specified suit and rank
        /// Constitutional requirement: Deterministic creation for testing
        /// </summary>
        public static Card CreateCard(CardSuit suit, CardRank rank, Vector2Int position)
        {
            var cardGameObject = new GameObject($"Card_{suit}_{rank}");
            var card = cardGameObject.AddComponent<Card>();

            card.Suit = suit;
            card.Rank = rank;
            card.Position = position;
            card.IsRevealed = false;
            card.IsAvailable = false;
            card.StackPosition = 0;

            return card;
        }

        /// <summary>
        /// Validates if this card can be moved to a target position
        /// Performance requirement: Must complete quickly for real-time gameplay
        /// </summary>
        public bool CanMoveTo(Vector2Int targetPosition)
        {
            // Basic validation - must be available to move
            if (!IsAvailable)
                return false;

            // Additional solitaire rule validation would go here
            // For now, just check that position is different
            return targetPosition != Position;
        }

        /// <summary>
        /// Validates if this card can be placed on another card (solitaire rules)
        /// </summary>
        public bool CanPlaceOn(Card targetCard)
        {
            if (targetCard == null) return false;

            // Standard solitaire rules: descending rank, alternating color
            bool isDescending = NumericValue == targetCard.NumericValue - 1;
            bool isAlternatingColor = IsRed != targetCard.IsRed;

            return isDescending && isAlternatingColor;
        }

        /// <summary>
        /// Updates card availability based on blocking relationships
        /// Constitutional requirement: Must handle blocking logic efficiently
        /// </summary>
        public void UpdateAvailability()
        {
            // Card is available if not blocked by other cards and is revealed
            bool hasBlockingCards = BlockingCards != null && BlockingCards.Length > 0;
            IsAvailable = IsRevealed && !hasBlockingCards;
        }

        /// <summary>
        /// Reveals the card (flips face-up)
        /// </summary>
        public void Reveal()
        {
            if (!IsRevealed)
            {
                IsRevealed = true;
                UpdateAvailability(); // Revealing may make card available
            }
        }

        /// <summary>
        /// Handles card selection for moves
        /// </summary>
        public void SelectCard()
        {
            if (IsAvailable)
            {
                OnCardSelected?.Invoke(this);
            }
        }

        /// <summary>
        /// Gets display name for the card
        /// </summary>
        public string GetDisplayName()
        {
            return $"{GetRankSymbol(_rank)}{GetSuitSymbol(_suit)}";
        }

        #endregion

        #region Validation Rules

        /// <summary>
        /// Validates solitaire move according to traditional rules
        /// Constitutional requirement: Deterministic validation for testing
        /// </summary>
        public static bool ValidateSolitaireMove(Card sourceCard, Card targetCard)
        {
            if (sourceCard == null || targetCard == null)
                return false;

            // Cannot move card onto itself
            if (sourceCard == targetCard)
                return false;

            // Source card must be available
            if (!sourceCard.IsAvailable)
                return false;

            // Apply solitaire placement rules
            return sourceCard.CanPlaceOn(targetCard);
        }

        /// <summary>
        /// Validates that position changes respect game physics
        /// </summary>
        public bool ValidatePositionPhysics(Vector2Int newPosition)
        {
            // Basic physics validation - position must be within game bounds
            // This would be expanded with actual game board dimensions
            return newPosition.x >= 0 && newPosition.y >= 0;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Updates the visual position to match logical position
        /// Constitutional requirement: Efficient visual updates
        /// </summary>
        private void UpdateVisualPosition()
        {
            // Convert logical grid position to world position
            // This would be configured based on actual game layout
            float cardWidth = 1.0f;
            float cardHeight = 1.4f;

            Vector3 worldPosition = new Vector3(
                Position.x * cardWidth,
                Position.y * cardHeight,
                0f
            );

            transform.position = worldPosition;
        }

        /// <summary>
        /// Updates visual indicators for card availability
        /// </summary>
        private void UpdateVisualAvailability()
        {
            // Update visual indicators (highlighting, etc.)
            // This would integrate with UI system
            var renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                // Example: Highlight available cards
                renderer.material.color = IsAvailable ? Color.white : Color.gray;
            }
        }

        /// <summary>
        /// Updates visual state for revealed/hidden cards
        /// </summary>
        private void UpdateVisualRevealed()
        {
            // Update card face visibility
            // This would show front/back of card
            gameObject.name = IsRevealed ? GetDisplayName() : "Card_Hidden";
        }

        /// <summary>
        /// Gets numeric value for card rank
        /// </summary>
        private int GetNumericValue(CardRank rank)
        {
            return rank switch
            {
                CardRank.Ace => 1,
                CardRank.Two => 2,
                CardRank.Three => 3,
                CardRank.Four => 4,
                CardRank.Five => 5,
                CardRank.Six => 6,
                CardRank.Seven => 7,
                CardRank.Eight => 8,
                CardRank.Nine => 9,
                CardRank.Ten => 10,
                CardRank.Jack => 11,
                CardRank.Queen => 12,
                CardRank.King => 13,
                _ => 0
            };
        }

        /// <summary>
        /// Gets display symbol for card rank
        /// </summary>
        private string GetRankSymbol(CardRank rank)
        {
            return rank switch
            {
                CardRank.Ace => "A",
                CardRank.Jack => "J",
                CardRank.Queen => "Q",
                CardRank.King => "K",
                _ => ((int)GetNumericValue(rank)).ToString()
            };
        }

        /// <summary>
        /// Gets display symbol for card suit
        /// </summary>
        private string GetSuitSymbol(CardSuit suit)
        {
            return suit switch
            {
                CardSuit.Hearts => "♥",
                CardSuit.Diamonds => "♦",
                CardSuit.Clubs => "♣",
                CardSuit.Spades => "♠",
                _ => "?"
            };
        }

        #endregion
    }

    /// <summary>
    /// Card suit enumeration
    /// </summary>
    public enum CardSuit
    {
        Hearts,
        Diamonds,
        Clubs,
        Spades
    }

    /// <summary>
    /// Card rank enumeration
    /// </summary>
    public enum CardRank
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
}