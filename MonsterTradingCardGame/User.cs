namespace MonsterTradingCardGame;

public class User
{
    public string Username { get; private set; }
    public string Password { get; private set; }
    private List<Card> _stack ;
    private Card[] _deckCards ;
    public int Coins { get; private set; }
    public int Elo { get; private set; }
    
    public User(string username, string password)
    {
        this.Username = username;
        this.Password = password;
        _stack = new List<Card>();
        _deckCards = new Card[4];
        Coins = 20;
        Elo = 100;
    }
}

