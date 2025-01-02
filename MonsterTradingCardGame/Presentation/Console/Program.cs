using MonsterTradingCardGame.Business.Services;
using MonsterTradingCardGame.Data;
using System.Net;
using System.Net.Sockets;
using MonsterTradingCardGame.API.Server;
using MonsterTradingCardGame.Data.Repositories;
using System.Reflection;

namespace MonsterTradingCardGame.Presentation.Console;

public class Program
{
    private static HttpServer? _server;
    private static Thread? _serverThread;

    public static void Main(string[] args)
    {
        try
        {
            // Repositories initialisieren


            // Services und Repositories mit korrekter Reihenfolge
            var cardRepository = new CardRepository();
            var userRepository = new UserRepository(cardRepository);
            var cardService = new CardService(userRepository);
            var statsRepository = new StatsRepository();
            var sessionRepository = new SessionRepository();

            // CardRepository mit dem cardService aktualisieren
            typeof(CardRepository)
                .GetField("_cardService", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.SetValue(cardRepository, cardService);

            var packageRepository = new PackageRepository(cardService);
            var battleService = new BattleService(statsRepository, userRepository);
            var userService = new UserService(userRepository, sessionRepository, statsRepository);
            var packageService = new PackageService(packageRepository, cardService);

            // BattleQueue initialisieren
            var battleQueue = new BattleQueue();
            var tradingRepository = new TradingRepository();
            var tradingService = new TradingService(tradingRepository, cardRepository);
            // Server-Komponenten initialisieren
            const int port = 10001;
            var router = new Router(
                userService,
                cardService,
                battleService,
                packageService,
                packageRepository,
                userRepository,
                statsRepository,
                battleQueue,
                tradingService
            );

            var requestProcessor = new RequestProcessor(router);
            var tcpListener = new TcpListener(IPAddress.Any, port);
            _server = new HttpServer(port, requestProcessor, tcpListener);

            // Server in einem separaten Thread starten
            _serverThread = new Thread(() =>
            {
                try
                {
                    _server.Start();
                    System.Console.WriteLine($"Server läuft unter: http://localhost:{port}");
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine($"Serverfehler: {ex.Message}");
                }
            });
            _serverThread.Start();

            // Datenbankverbindung testen
            TestDatabaseConnection();

            System.Console.WriteLine("Drücken Sie eine beliebige Taste zum Beenden...");
            System.Console.ReadKey();

            // Server ordnungsgemäß beenden
            _server.Stop();
            _serverThread.Join(); // Warten bis der Server-Thread beendet ist
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Kritischer Fehler: {ex.Message}");
            System.Console.WriteLine(ex.StackTrace);
        }
        finally
        {
            // Sicherstellen, dass der Server in jedem Fall beendet wird
            if (_server != null)
            {
                _server.Stop();
            }

            if (_serverThread != null && _serverThread.IsAlive)
            {
                _serverThread.Join();
            }
        }
    }

    private static void TestDatabaseConnection()
    {
        try
        {
            var dataLayer = DataLayer.Instance;
            using var command = dataLayer.CreateCommand("SELECT 1");
            command.ExecuteScalar();
            System.Console.WriteLine("Datenbankverbindung erfolgreich hergestellt!");
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Datenbankverbindungsfehler: {ex.Message}");
            throw;
        }
    }
}