using MonsterTradingCardGame.Business.Services;
using MonsterTradingCardGame.Business.Services.Interfaces;
using MonsterTradingCardGame.Data.Repositories.Interfaces;
using MonsterTradingCardGame.Domain.Models;
using NSubstitute;

namespace MonsterTradingCardGameTests;

public class TradingServiceTests
{
    private ITradingService _tradingService;
    private ITradingRepository _tradingRepository;
    private ICardRepository _cardRepository;
    private User _testUser;

    [SetUp]
    public void Setup()
    {
        _cardRepository = Substitute.For<ICardRepository>();
        _tradingRepository = Substitute.For<ITradingRepository>();
        _testUser = new User("testUser", "hashedPassword", 1);
        _tradingService = new TradingService(_tradingRepository, _cardRepository);
    }

    [Test]
    public void CreateTrade_CardInDeck_ThrowsInvalidOperationException()
    {
        // Arrange
        var card = new SpellCard("1", "TestSpell", 10, ElementType.Fire) 
        { 
            UserId = 1, 
            InDeck = true 
        };
        _cardRepository.GetCardById("1").Returns(card);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => 
            _tradingService.CreateTrade("trade1", "1", "spell", 10, _testUser));
    }

    [Test]
    public void ExecuteTrade_TradingWithSelf_ThrowsInvalidOperationException()
    {
        // Arrange
        var trading = new Trading("trade1", "card1", "spell", 10, 1);
        _tradingRepository.GetTrade("trade1").Returns(trading);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => 
            _tradingService.ExecuteTrade("trade1", "card2", _testUser));
    }
}
