namespace MonsterTradingCardGame;

public class Program
{
    public static void Main(string[] args)
    {
        var user = new User("TestUser", "password123");
        Console.WriteLine($"Created user: {user.Username}");
    }
}