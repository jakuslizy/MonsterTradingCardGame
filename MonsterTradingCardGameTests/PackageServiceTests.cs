using MonsterTradingCardGame.Business.Services;
using MonsterTradingCardGame.Business.Services.Interfaces;
using MonsterTradingCardGame.Data.Repositories.Interfaces;
using NSubstitute;
using System.Text.Json;
using MonsterTradingCardGame.Domain.Models;

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
        // Arrange
        var invalidJson = "invalid json format";

        // Act & Assert
        Assert.Throws<JsonException>(
            () => _packageService.CreatePackage(invalidJson, "admin")
        );
    }

    [Test]
    public void CreatePackage_NonAdminUser_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var validJson = "[{\"Id\":\"1\", \"Name\":\"Dragon\", \"Damage\":10}]";

        // Act & Assert
        var exception = Assert.Throws<UnauthorizedAccessException>(
            () => _packageService.CreatePackage(validJson, "normaluser")
        );
        Assert.That(exception.Message, Is.EqualTo("Only admin can create packages"));
    }

    [Test]
    public void CreatePackage_WithFiveValidCards_CreatesSuccessfully()
    {
        // Arrange
        var validJson = @"[
            {""Id"":""1"",""Name"":""WaterGoblin"",""Damage"":10},
            {""Id"":""2"",""Name"":""FireSpell"",""Damage"":20},
            {""Id"":""3"",""Name"":""RegularSpell"",""Damage"":30},
            {""Id"":""4"",""Name"":""Knight"",""Damage"":40},
            {""Id"":""5"",""Name"":""WaterSpell"",""Damage"":50}
        ]";

        // Mock the card creation
        _cardService.CreateCard(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<int>(), Arg.Any<ElementType>())
            .Returns(args => new SpellCard(
                (string)args[0], 
                (string)args[1], 
                (int)args[2], 
                (ElementType)args[3]
            ));

        // Act & Assert
        Assert.DoesNotThrow(() => _packageService.CreatePackage(validJson, "admin"));
        
        // Verify that package was created
        _packageRepository.Received(1).CreatePackage(Arg.Any<Package>(), Arg.Any<List<Card>>());
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

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(
            () => _packageService.CreatePackage(invalidJson, "admin")
        );
        Assert.That(ex.Message, Is.EqualTo("Package must contain exactly 5 cards"));
    }
}