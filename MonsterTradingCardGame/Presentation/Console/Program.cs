using MonsterTradingCardGame.Business.Services;
using MonsterTradingCardGame.Data;
using MonsterTradingCardGame.Domain.Models;
using Npgsql;
using System;
using System.Net;
using System.Net.Sockets;
using MonsterTradingCardGame.API.Server;
using MonsterTradingCardGame.Data.Repositories;


namespace MonsterTradingCardGame.Presentation.Console;

public class Program
{
    public static void Main(string[] args)
    {
        // Dienste initialisieren
        var userRepository = new UserRepository();
        var sessionRepository = new SessionRepository();
        var userService = new UserService(userRepository, sessionRepository);
        var cardService = new CardService();
        var battleService = new BattleService();

        const int port = 10001;
        var router = new Router(userService, cardService, battleService);
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

                // Benutzer hinzufügen und überprüfen
               // AddAndVerifyUser();

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
            // Server beenden
            
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

    // private static void AddAndVerifyUser()
    // {
    //     var userRepository = new UserRepository();
    //     var sessionRepository = new SessionRepository();
    //     var userService = new UserService(userRepository, sessionRepository);
    //
    //     var testUsername = "Bubi";
    //     var testPassword = "testpassword";
    //
    //     System.Console.WriteLine($"Versuche, Benutzer {testUsername} zu registrieren...");
    //     var newUser = userService.RegisterUser(testUsername, testPassword);
    //
    //     System.Console.WriteLine($"Benutzer {newUser.Username} wurde registriert. Überprüfe Speicherung...");
    //
    //     var retrievedUser = userRepository.GetUser(testUsername);
    //     if (retrievedUser != null)
    //     {
    //         System.Console.WriteLine($"Benutzer {retrievedUser.Username} wurde erfolgreich in der Datenbank gespeichert.");
    //         System.Console.WriteLine($"Benutzer-ID: {retrievedUser.Id}");
    //         System.Console.WriteLine($"Erstellungsdatum: {retrievedUser.CreatedAt}");
    //         System.Console.WriteLine($"Gespeicherter Passwort-Hash: {retrievedUser.PasswordHash}");
    //
    //         // Test Login und Token-Validierung
    //         TestLoginAndTokenValidation(userService, testUsername, testPassword);
    //     }
    //     else
    //     {
    //         System.Console.WriteLine("Fehler: Benutzer konnte nicht in der Datenbank gefunden werden.");
    //     }
    // }

    // private static void TestLoginAndTokenValidation(UserService userService, string username, string password)
    // {
    //     try
    //     {
    //         System.Console.WriteLine($"Versuche, Benutzer {username} anzumelden...");
    //         var token = userService.LoginUser(username, password);
    //         System.Console.WriteLine($"Anmeldung erfolgreich. Token: {token}");
    //
    //         System.Console.WriteLine("Überprüfe Token-Validierung...");
    //         var isValid = userService.ValidateToken(token);
    //         System.Console.WriteLine($"Token ist gültig: {isValid}");
    //
    //         if (isValid)
    //         {
    //             var user = userService.GetUserFromToken(token);
    //             System.Console.WriteLine($"Benutzer aus Token: {user.Username}");
    //         }
    //     }
    //     catch (Exception ex)
    //     {
    //         System.Console.WriteLine($"Fehler beim Login oder der Token-Validierung: {ex.Message}");
    //     }
   // }
}
