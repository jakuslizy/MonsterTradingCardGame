using MonsterTradingCardGame.API.Server;
using MonsterTradingCardGame.Business.Services;
using MonsterTradingCardGame.Domain.Models;
using System.Net;
using System.Net.Sockets;

namespace MonsterTradingCardGame.Presentation.Console;

public class Program
{
    public static void Main(string[] args)
    {
        // Erstellen Sie zwei Testbenutzer
        User player1 = new User("TestUser", "password123");
        User player2 = new User("TestUser2", "password123");

        var server = new HttpServer(8080, new TcpListener(IPAddress.Any, 8080));
        server.Start();

        // Fügen Sie mehr Karten zu den Decks hinzu, um einen längeren Kampf zu simulieren
        player1.AddCardToDeck(new MonsterCard("Goblin", "Lizy", 30, ElementType.Fire));
        player1.AddCardToDeck(new SpellCard("Feuerball", 25, ElementType.Fire));
        player1.AddCardToDeck(new MonsterCard("Ork", "Grunty", 40, ElementType.Normal));

        player2.AddCardToDeck(new SpellCard("Wasserwelle", 20, ElementType.Water));
        player2.AddCardToDeck(new MonsterCard("Drache", "Flamey", 50, ElementType.Fire));
        player2.AddCardToDeck(new SpellCard("Eisstrahl", 35, ElementType.Water));
        for (int i = 0; i < 10; i++) // Fügen Sie 10 Karten für jeden Spieler hinzu
        {
            player1.AddCardToDeck(new MonsterCard($"Monster{i}", $"Name{i}", 30 + i, ElementType.Fire));
            player2.AddCardToDeck(new SpellCard($"Spell{i}", 25 + i, ElementType.Fire));
        }

        // Erstellen Sie einen BattleService mit den Testbenutzern
        BattleService battle = new BattleService(player1, player2);

        // Führen Sie den Kampf aus und speichern Sie das Ergebnis
        string battleResult = battle.ExecuteBattle();

        // Geben Sie das Ergebnis aus
        System.Console.WriteLine("Kampfergebnis:");
        System.Console.WriteLine(battleResult);
    }
}