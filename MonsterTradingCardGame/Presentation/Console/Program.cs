using MonsterTradingCardGame.Business.Services;
using MonsterTradingCardGame.Data;
using MonsterTradingCardGame.Domain.Models;
using Npgsql;
using System;

namespace MonsterTradingCardGame.Presentation.Console;

public class Program
{
    public static void Main(string[] args)
    {
        try
        {
            // Datenbankverbindung testen
            TestDatabaseConnection();

            // Benutzer hinzufügen und überprüfen
            AddAndVerifyUser();

            System.Console.WriteLine("Drücken Sie eine beliebige Taste zum Beenden...");
            System.Console.ReadKey();
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Ein Fehler ist aufgetreten: {ex.Message}");
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

    private static void AddAndVerifyUser()
    {
        var userRepository = new UserRepository();
        var userService = new UserService(userRepository);

        var testUsername = "Pluto";
        var testPassword = "testpassword";

        System.Console.WriteLine($"Versuche, Benutzer {testUsername} zu registrieren...");
        var newUser = userService.RegisterUser(testUsername, testPassword);

        System.Console.WriteLine($"Benutzer {newUser.Username} wurde registriert. Überprüfe Speicherung...");

        var retrievedUser = userRepository.GetUser(testUsername);
        if (retrievedUser != null)
        {
            System.Console.WriteLine($"Benutzer {retrievedUser.Username} wurde erfolgreich in der Datenbank gespeichert.");
            System.Console.WriteLine($"Benutzer-ID: {retrievedUser.Id}");
            System.Console.WriteLine($"Erstellungsdatum: {retrievedUser.CreatedAt}");
            System.Console.WriteLine($"Gespeicherter Passwort-Hash: {retrievedUser.PasswordHash}");
        }
        else
        {
            System.Console.WriteLine("Fehler: Benutzer konnte nicht in der Datenbank gefunden werden.");
        }
    }
}
