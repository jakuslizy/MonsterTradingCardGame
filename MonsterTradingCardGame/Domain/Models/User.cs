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
        _deck = new List<Card>();
        Coins = coins;
        Elo = elo;
    }

public void SetDeck(List<Card> cards)
{
    if (cards == null)
    {
        throw new ArgumentNullException(nameof(cards));
    }

    if (cards.Count != 4)
    {
        throw new InvalidOperationException("Deck must contain exactly 4 cards");
    }

    _deck = new List<Card>(cards);
}

public void ClearDeck()
{
    _stack.AddRange(_deck);  
    _deck.Clear();
}

    public void AddCardToStack(Card card)
    {
        _stack.Add(card);
    }


    public void UpdateCoins(int newAmount)
    {
        Coins = newAmount;
    }

    public void UpdateElo(int newAmount)
    {
        Elo = newAmount;
    }
}
