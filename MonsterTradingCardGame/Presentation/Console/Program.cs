using MonsterTradingCardGame.Domain.Models;

namespace MonsterTradingCardGame.Presentation.Console;

public class Program
{
    public static void Main(string[] args)
    {
        User user = new User("TestUser", "password123");
        System.Console.WriteLine($"Created user: {user.Username}");
    }
}