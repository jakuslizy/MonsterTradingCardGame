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
        var card = new SpellCard("1", "TestSpell", 10, ElementType.Fire)
        {
            UserId = 1,
            InDeck = true
        };
        TestContext.WriteLine($"Karte vorbereitet: {card.Name} (Im Deck: {card.InDeck})");

        _cardRepository.GetCardById("1").Returns(card);
        TestContext.WriteLine("CardRepository Mock konfiguriert");

        var ex = Assert.Throws<InvalidOperationException>(() =>
            _tradingService.CreateTrade("trade1", "1", "spell", 10, _testUser));
        TestContext.WriteLine($"{ex.Message}");
    }

    [Test]
    public void ExecuteTrade_TradingWithSelf_ThrowsInvalidOperationException()
    {
        var trading = new Trading("trade1", "card1", "spell", 10, 1);
        TestContext.WriteLine($"Trading vorbereitet: ID {trading.Id}, UserId {trading.UserId}");

        _tradingRepository.GetTrade("trade1").Returns(trading);
        TestContext.WriteLine("TradingRepository Mock konfiguriert");

        var ex = Assert.Throws<InvalidOperationException>(() =>
            _tradingService.ExecuteTrade("trade1", "card2", _testUser));
        TestContext.WriteLine($" {ex.Message}");
    }

    [Test]
    public void ExecuteTrade_WithValidCards_SuccessfullyCompletesTrade()
    {
        var tradingCard = new SpellCard("card1", "FireSpell", 50, ElementType.Fire)
        {
            UserId = 2, // Anderer User
            InDeck = false
        };

        var offeredCard = new SpellCard("card2", "WaterSpell", 80, ElementType.Water)
        {
            UserId = 1, // Test User
            InDeck = false
        };

        TestContext.WriteLine($"Trading Card vorbereitet: {tradingCard.Name} (Besitzer: User {tradingCard.UserId})");
        TestContext.WriteLine($"Offered Card vorbereitet: {offeredCard.Name} (Besitzer: User {offeredCard.UserId})");

        var trading = new Trading("trade1", "card1", "spell", 70, 2);

        _tradingRepository.GetTrade("trade1").Returns(trading);
        _cardRepository.GetCardById("card1").Returns(tradingCard);
        _cardRepository.GetCardById("card2").Returns(offeredCard);
        TestContext.WriteLine("Repository Mocks konfiguriert");

        TestContext.WriteLine("Führe Trade aus...");
        Assert.DoesNotThrow(() => _tradingService.ExecuteTrade("trade1", "card2", _testUser));
        TestContext.WriteLine("Trade erfolgreich durchgeführt");

        _cardRepository.Received(1).TransferCard("card1", 2, 1); // Trading Card zum Test User
        _cardRepository.Received(1).TransferCard("card2", 1, 2); // Offered Card zum anderen User
        _tradingRepository.Received(1).DeleteTrade("trade1");

        TestContext.WriteLine("Überprüfung: Karten wurden korrekt übertragen und Trading-Deal wurde gelöscht");
    }
}