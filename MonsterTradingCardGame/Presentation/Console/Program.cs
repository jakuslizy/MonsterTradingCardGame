using MonsterTradingCardGame.Business.Services;
using MonsterTradingCardGame.Domain.Models;

namespace MonsterTradingCardGame.Presentation.Console;

public class Program
{
    public static void Main(string[] args)
    {
        User player1 = new User("TestUser", "password123");
        player1.AddCardToDeck(new MonsterCard("LizyMonster1", 100, ElementType.Water));
        player1.AddCardToDeck(new MonsterCard("LizyMonster2", 80, ElementType.Normal));
        player1.AddCardToDeck(new MonsterCard("LizyMonster3", 10, ElementType.Normal));
        player1.AddCardToDeck(new MonsterCard("LizyMonster4", 60, ElementType.Fire));
        player1.AddCardToDeck(new MonsterCard("LizyMonster5", 80, ElementType.Fire));
        
        User player2 = new User("TestUser", "password123");
        player2.AddCardToDeck(new MonsterCard("MuyMuyMonster1", 200, ElementType.Fire));
        player2.AddCardToDeck(new MonsterCard("MuyMuyMonster2", 20, ElementType.Normal));
        player2.AddCardToDeck(new MonsterCard("MuyMuyMonster3", 30, ElementType.Normal));
        player2.AddCardToDeck(new MonsterCard("MuyMuyMonster4", 70, ElementType.Normal));
        player2.AddCardToDeck(new MonsterCard("MuyMuyMonster5", 60, ElementType.Fire));

        BattleService battle = new BattleService(player1, player2);
        battle.Playtestlizy();
    }
}