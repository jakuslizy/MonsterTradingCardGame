namespace MonsterTradingCardGame.Domain.Models;

public class User
{
    public string Username { get; private set; }
    public string Password { get; private set; }
    private List<Card> _stack;
    public List<Card> Deck { get; set; } = new List<Card>(4);
    public int Coins { get; private set; }
    public int Elo { get; private set; }

    public User(string username, string password)
    {
        Username = username;
        Password = password;
        _stack = new List<Card>();
        Deck = new List<Card>(4);
        Coins = 20;
        Elo = 100;
    }

    public void AddCardToStack(Card card)
    {
        _stack.Add(card);
    }

    public void AddCardToDeck(Card card)
    {
        if (Deck.Count >= 4)
        {
            throw new InvalidOperationException("Deck is already full");
        }

        if (!_stack.Contains(card))
        {
            throw new InvalidOperationException("Card is not in the user's stack");
        }

        Deck.Add(card);
        _stack.Remove(card);
    }

    public void RemoveCardFromDeck(Card card)
    {
        if (!Deck.Remove(card))
        {
            throw new InvalidOperationException("Card is not in the deck");
        }
        _stack.Add(card);
    }

    public void UpdateElo(int change)
    {
        Elo += change;
    }

    public IReadOnlyList<Card> GetStack()
    {
        return _stack.AsReadOnly();
    }

    public void ClearDeck()
    {
        _stack.AddRange(Deck);
        Deck.Clear();
    }

    public void UpdateCoins(int newAmount)
    {
        Coins = newAmount;
    }
}