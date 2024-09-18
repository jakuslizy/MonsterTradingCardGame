namespace MonsterTradingCardGame;

public class User
{
    public string username
    {
        get;
        private set;
    }
    public string password { get; private set; }
    private List<Card> Stack ;
    private Card[] deckCards ;
    public int coins { get; private set; }
    public int elo { get; private set; }
    
    public User(string username, string password)
    {
        this.username = username;
        this.password = password;
        Stack = new List<Card>();
        deckCards = new Card[4];
        coins = 20;
        elo = 100;
    }
}

