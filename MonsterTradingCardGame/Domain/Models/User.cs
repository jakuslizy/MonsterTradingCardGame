namespace MonsterTradingCardGame.Domain.Models;

public class User
{
    public string Username { get; private set; }
    public string Password { get; private set; }
    private List<Card> _stack;
    public List<Card> Deck { get; private set; }
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
        Deck.Add(card);
    }
    
    public void UpdateElo(int change)
    {
        Elo += change;
    }
}

