namespace MonsterTradingCardGame.Domain.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; private set; }
    public string PasswordHash { get; private set; }
    public DateTime CreatedAt { get; private set; }
    private List<Card> _stack;
    private List<Card> _deck;
    public int Coins { get; private set; }
    public int Elo { get; private set; }

    public IReadOnlyList<Card> Stack => _stack.AsReadOnly();
    public IReadOnlyList<Card> Deck => _deck.AsReadOnly();

    public User(
        string username, 
        string passwordHash, 
        int id = 0, 
        DateTime? createdAt = null, 
        int coins = 20, 
        int elo = 100)
    {
        Id = id;
        Username = username;
        PasswordHash = passwordHash;
        CreatedAt = createdAt ?? DateTime.UtcNow;
        _stack = new List<Card>();
        _deck = new List<Card>(4);
        Coins = coins;
        Elo = elo;
    }

    public void AddCardToStack(Card card)
    {
        _stack.Add(card);
    }

    public void AddCardToDeck(Card card)
    {
        if (_deck.Count >= 4)
        {
            throw new InvalidOperationException("Deck is already full");
        }

        if (!_stack.Contains(card))
        {
            throw new InvalidOperationException("Card is not in the user's stack");
        }

        _deck.Add(card);
        _stack.Remove(card); 
    }

    public void RemoveCardFromDeck(Card card)
    {
        if (!_deck.Remove(card))
        {
            throw new InvalidOperationException("Card is not in the deck");
        }

        _stack.Add(card);
    }

    public void UpdateElo(int newAmount)
    {
        Elo = newAmount;
    }

    public void ClearDeck()
    {
        _stack.AddRange(_deck);
        _deck.Clear();
    }

    public void UpdateCoins(int newAmount)
    {
        Coins = newAmount;
    }

    public void SetStack(List<Card> cards) => _stack = cards;
    public void SetDeck(List<Card> cards) => _deck = cards;
}
