namespace MonsterTradingCardGame.Domain.Models;

public class User
{
    public string Username { get; private set; }
    public string Password { get; private set; }
    private List<Card> _stack ;
    private Card[] _deck ;
    public int Coins { get; private set; }
    public int Elo { get; private set; }
    
    public User(string username, string password)
    {
        Username = username;
        Password = password;
        _stack = new List<Card>();
        _deck = new Card[4];
        Coins = 20;
        Elo = 100;
    } 
    
    public void AddCardToStack(Card card)
    {
        _stack.Add(card);
    }
}

