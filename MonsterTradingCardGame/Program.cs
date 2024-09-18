namespace MonsterTradingCardGame;

public class Program
{
    public static void Main(string[] args)
    {
        User user = new User("TestUser", "password123");
        Console.WriteLine($"Created user: {user.username}");
    }
}