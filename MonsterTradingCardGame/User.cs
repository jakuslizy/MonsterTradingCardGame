namespace MonsterTradingCardGame;

public class User
{
    public string Username { get; set; }
    public string Password { get; set; }
    public List<Card> Stack { get; set; }
    public List<Card> Deck { get; set; }
    public int Coins { get; set; }
    public int Elo { get; set; }
    
    public User(string username, string password)
    {
        Username = username;
        Password = password;
        Stack = new List<Card>();
        Deck = new List<Card>();
        Coins = 20;
        Elo = 100;
    }
}

