using MonsterTradingCardGame.Business.Services;
using MonsterTradingCardGame.Business.Services.Interfaces;
using MonsterTradingCardGame.Data.Repositories.Interfaces;
using NSubstitute;

namespace MonsterTradingCardGameTests;

public class PackageServiceTests
{
    private IPackageRepository _packageRepository;
    private ICardService _cardService;
    private PackageService _packageService;

    [SetUp]
    public void Setup()
    {
        _packageRepository = Substitute.For<IPackageRepository>();
        _cardService = Substitute.For<ICardService>();
        _packageService = new PackageService(_packageRepository, _cardService);
    }

    [Test]
    public void CreatePackage_NonAdminUser_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var cardJson = "[{\"Id\":\"1\", \"Name\":\"Dragon\", \"Damage\":10}]";
        
        // Act & Assert
        Assert.Throws<UnauthorizedAccessException>(() => 
            _packageService.CreatePackage(cardJson, "normaluser"));
    }

    [Test]
    public void CreatePackage_InvalidJson_ThrowsJsonException()
    {
        // Arrange
        var invalidJson = "invalid json format";
    
        // Act & Assert
        Assert.Throws<System.Text.Json.JsonException>(() => 
            _packageService.CreatePackage(invalidJson, "admin"));
    }
}