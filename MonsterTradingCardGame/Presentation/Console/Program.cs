using MonsterTradingCardGame.Business.Services;
using MonsterTradingCardGame.Domain.Models;

namespace MonsterTradingCardGame.Presentation.Console;

public class Program
{
    public static void Main(string[] args)
    {
        User player1 = new User("TestUser", "password123");
        User player2 = new User("TestUser2", "password123");
        player1.AddCardToDeck(new MonsterCard("Goblin", "Lizy", 30, ElementType.Fire));
        player2.AddCardToDeck(new SpellCard("Fasz", 80, ElementType.Fire));

        BattleService battle = new BattleService(player1, player2);
        battle.Playtestlizy();
    }
}