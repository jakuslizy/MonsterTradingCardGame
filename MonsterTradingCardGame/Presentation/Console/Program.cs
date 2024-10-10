using MonsterTradingCardGame.API.Server;
using MonsterTradingCardGame.Business.Services;
using MonsterTradingCardGame.Domain.Models;
using MonsterTradingCardGame.Domain.Models.MonsterCards;
using MonsterTradingCardGame.Data;
using System.Net;
using System.Net.Sockets;

namespace MonsterTradingCardGame.Presentation.Console;

public class Program
{
    public static void Main(string[] args)                                                                                                             
    {
        // Dienste initialisieren
        var userService = new UserService();
        var cardService = new CardService();
        var battleService = new BattleService();

        // Testbenutzer erstellen und zur Datenbank hinzufügen
        var player1 = new User("TestUser", "password123");
        var player2 = new User("TestUser2", "password123");
        InMemoryDatabase.AddUser(player1);
        InMemoryDatabase.AddUser(player2);

        // Karten zu den Decks hinzufügen
        AddCardsToUser(player1, cardService);
        AddCardsToUser(player2, cardService);

        const int port = 10001;
        var router = new Router(userService, cardService, battleService);
        var requestProcessor = new RequestProcessor(router);
        var tcpListener = new TcpListener(IPAddress.Any, port);
        var server = new HttpServer(port, requestProcessor, tcpListener);

        try
        {
            server.Start();
            System.Console.WriteLine($"Server is running at: http://localhost:{port}");
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    private static void AddCardsToUser(User user, CardService cardService)
    {
        var cards = new List<Card>
        {
            new Goblin(Guid.NewGuid().ToString(), 30, ElementType.Fire),
            new SpellCard(Guid.NewGuid().ToString(), "Feuerball", 25, ElementType.Fire),
            new Ork(Guid.NewGuid().ToString(), 40, ElementType.Normal),
            new SpellCard(Guid.NewGuid().ToString(), "Wasserwelle", 20, ElementType.Water),
            new Dragon(Guid.NewGuid().ToString(), "Drache", 50, ElementType.Fire),
            new SpellCard(Guid.NewGuid().ToString(), "Eisstrahl", 35, ElementType.Water)
        };

        foreach (var card in cards)
        {
            user.AddCardToStack(card);
        }

        // 10 zusätzliche Karten hinzu
        for (int i = 0; i < 10; i++)
        {
            user.AddCardToStack(new Dragon(Guid.NewGuid().ToString(), $"Dragon{i}", 30 + i, ElementType.Fire));
            user.AddCardToStack(new SpellCard(Guid.NewGuid().ToString(), $"Spell{i}", 25 + i, ElementType.Water));
        }

        // Deck des Benutzers (ersten 4 Karten)
        var deckCards = user.GetStack().Take(4).Select(c => c.Id).ToList();
        cardService.ConfigureDeck(user, deckCards);
    }
}