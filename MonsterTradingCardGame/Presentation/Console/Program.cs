﻿using MonsterTradingCardGame.Business.Services;
using MonsterTradingCardGame.Data;
using MonsterTradingCardGame.Domain.Models;
using System.Net;
using System.Net.Sockets;
using MonsterTradingCardGame.API.Server;
using MonsterTradingCardGame.Data.Repositories;
using System.Reflection;

namespace MonsterTradingCardGame.Presentation.Console;

public class Program
{
    public static void Main(string[] args)
    {
        // Repositories initialisieren
        var userRepository = new UserRepository(null);
        var sessionRepository = new SessionRepository();
        var statsRepository = new StatsRepository();

        // Services initialisieren
        var cardService = new CardService(userRepository);
        var battleService = new BattleService(statsRepository);
        var userService = new UserService(userRepository, sessionRepository, statsRepository);

        // PackageRepository mit cardService initialisieren
        var packageRepository = new PackageRepository(cardService);
        var packageService = new PackageService(packageRepository, userRepository, cardService);

        // CardService im UserRepository nachträglich setzen
        typeof(UserRepository).GetField("_cardService", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.SetValue(userRepository, cardService);

        const int port = 10001;
        var router = new Router(
            userService, 
            cardService, 
            battleService, 
            packageService,
            packageRepository,    
            userRepository,
            statsRepository
        );
        var requestProcessor = new RequestProcessor(router);
        var tcpListener = new TcpListener(IPAddress.Any, port);
        var server = new HttpServer(port, requestProcessor, tcpListener);

        try
        {
            // Server in einem separaten Thread starten
            Thread serverThread = new Thread(() =>
            {
                try
                {
                    server.Start();
                    System.Console.WriteLine($"Server is running at: http://localhost:{port}");
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine($"Server konnte nicht gestartet werden: {ex.Message}");
                }
            });
            serverThread.Start();

            try
            {
                // Datenbankverbindung testen
                TestDatabaseConnection();

                System.Console.WriteLine("Drücken Sie eine beliebige Taste zum Beenden...");
                System.Console.ReadKey();
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"Ein Fehler ist aufgetreten: {ex.Message}");
            }
        }
        finally
        {
            // Optional: Cleanup-Code hier
        }
    }

    private static void TestDatabaseConnection()
    {
        try
        {
            var dataLayer = DataLayer.Instance;
            
            using (var command = dataLayer.CreateCommand("SELECT 1"))
            {
                command.ExecuteScalar();
            }
            
            System.Console.WriteLine("Datenbankverbindung erfolgreich!");
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Datenbankverbindungsfehler: {ex.Message}");
            throw;
        }
    }
}