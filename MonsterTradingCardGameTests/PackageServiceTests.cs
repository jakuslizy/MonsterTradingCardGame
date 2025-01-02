using MonsterTradingCardGame.Business.Services;
using MonsterTradingCardGame.Business.Services.Interfaces;
using MonsterTradingCardGame.Data.Repositories.Interfaces;
using NSubstitute;
using System.Text.Json;
using MonsterTradingCardGame.Domain.Models;
using MonsterTradingCardGame.Domain.Models.MonsterCards;

namespace MonsterTradingCardGameTests;

[TestFixture]
public class PackageServiceTests
{
    private IPackageService _packageService;
    private IPackageRepository _packageRepository;
    private ICardService _cardService;

    [SetUp]
    public void Setup()
    {
        _packageRepository = Substitute.For<IPackageRepository>();
        _cardService = Substitute.For<ICardService>();
        _packageService = new PackageService(_packageRepository, _cardService);
    }

    [Test]
    public void CreatePackage_InvalidJson_ThrowsJsonException()
    {
        var invalidJson = "invalid json format";
        TestContext.WriteLine($"Ungültige JSON-Eingabe: '{invalidJson}'");

        // Act & Assert
        TestContext.WriteLine("Versuche Paket zu erstellen...");
        var ex = Assert.Throws<JsonException>(
            () => _packageService.CreatePackage(invalidJson, "admin")
        );
        TestContext.WriteLine($" {ex.Message}");
    }

    [Test]
    public void CreatePackage_NonAdminUser_ThrowsUnauthorizedAccessException()
    {
        var validJson = "[{\"Id\":\"1\", \"Name\":\"Dragon\", \"Damage\":10}]";
        TestContext.WriteLine($"Gültige JSON-Eingabe vorbereitet, aber nicht-Admin User");

        // Act & Assert
        TestContext.WriteLine("Versuche Paket als 'normaluser' zu erstellen...");
        var ex = Assert.Throws<UnauthorizedAccessException>(
            () => _packageService.CreatePackage(validJson, "normaluser")
        );
        TestContext.WriteLine($" {ex.Message}");
        Assert.That(ex.Message, Is.EqualTo("Only admin can create packages"));
    }

    [Test]
    public void CreatePackage_WithFiveValidCards_CreatesSuccessfully()
    {
        TestContext.WriteLine("Test: Erstelle Paket mit 5 verschiedenen Karten");

        var validJson = @"[
            {""Id"":""1"",""Name"":""WaterGoblin"",""Damage"":10},
            {""Id"":""2"",""Name"":""FireSpell"",""Damage"":20},
            {""Id"":""3"",""Name"":""Dragon"",""Damage"":30},
            {""Id"":""4"",""Name"":""Knight"",""Damage"":40},
            {""Id"":""5"",""Name"":""WaterSpell"",""Damage"":50}
        ]";
        TestContext.WriteLine("JSON-Eingabe vorbereitet");

        // Mock the card creation mit korrekten Kartentypen
        _cardService.CreateCard(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<int>(), Arg.Any<ElementType>())
            .Returns(args =>
            {
                var name = (string)args[1];
                var id = (string)args[0];
                var damage = (int)args[2];
                var elementType = (ElementType)args[3];

                Card card = name switch
                {
                    var n when n.EndsWith("Spell") => new SpellCard(id, name, damage, elementType),
                    var n when n.Contains("Goblin") => new Goblin(id, name, damage, elementType),
                    var n when n.Contains("Dragon") => new Dragon(id, name, damage, elementType),
                    var n when n.Contains("Knight") => new Knight(id, name, damage, elementType),
                    _ => new SpellCard(id, name, damage, elementType) // Fallback
                };

                TestContext.WriteLine($"Karte erstellt: {card.GetType().Name} - {card.Name} mit Schaden {card.Damage}");
                return card;
            });

        // Act & Assert
        TestContext.WriteLine("Versuche Paket zu erstellen...");
        Assert.DoesNotThrow(() => _packageService.CreatePackage(validJson, "admin"));
        TestContext.WriteLine("Paket wurde erfolgreich erstellt");

        // Verify that package was created with correct card types
        _packageRepository.Received(1).CreatePackage(
            Arg.Any<Package>(),
            Arg.Is<List<Card>>(cards =>
                cards.Count == 5 &&
                cards.Any(c => c is Goblin) &&
                cards.Any(c => c is SpellCard) &&
                cards.Any(c => c is Dragon) &&
                cards.Any(c => c is Knight)
            )
        );
        TestContext.WriteLine("Repository wurde mit verschiedenen Kartentypen aufgerufen");
    }

    [Test]
    public void CreatePackage_WithInvalidCardCount_ThrowsException()
    {
        // Arrange
        var invalidJson = @"[
            {""Id"":""1"",""Name"":""FireSpell"",""Damage"":10},
            {""Id"":""2"",""Name"":""WaterSpell"",""Damage"":20},
            {""Id"":""3"",""Name"":""RegularSpell"",""Damage"":30}
        ]";
        TestContext.WriteLine($"JSON-Eingabe mit {invalidJson.Count(c => c == '{')}/5 Karten vorbereitet");

        // Act & Assert
        TestContext.WriteLine("Versuche Paket zu erstellen...");
        var ex = Assert.Throws<ArgumentException>(
            () => _packageService.CreatePackage(invalidJson, "admin")
        );
        TestContext.WriteLine($" {ex.Message}");
        Assert.That(ex.Message, Is.EqualTo("Package must contain exactly 5 cards"));
    }
}