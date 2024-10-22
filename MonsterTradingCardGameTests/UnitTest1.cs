using MonsterTradingCardGame.API.Server;
using MonsterTradingCardGame.Business.Services;
using MonsterTradingCardGame.Data.Repositories;

namespace MonsterTradingCardGameTests;

public class Tests
{
    private Router _router;
    private UserService _userService;
    private CardService _cardService;
    private BattleService _battleService;
    private UserRepository _userRepository;
    private SessionRepository _sessionRepository;

    [SetUp]
    public void Setup()
    {
        // Repositories und Services initialisieren
        _userRepository = new UserRepository();
        _sessionRepository = new SessionRepository();
        _userService = new UserService(_userRepository, _sessionRepository);
        _cardService = new CardService();
        _battleService = new BattleService();
        
        // Router mit den Services
        _router = new Router(_userService, _cardService, _battleService);
    }

    [Test]
    public void TestHomeRoute()
    {
        // Test der Home-Route
        var response = _router.RouteRequest("GET / HTTP/1.1", new Dictionary<string, string>(), "");
        if (response != null)
            Assert.Pass();
        else
            Assert.Fail();
    }

    [Test]
    public void TestUsersRoute()
    {
        // Test der Users-Route
        var response = _router.RouteRequest("POST /users HTTP/1.1", new Dictionary<string, string>(), "");
        if (response != null)
            Assert.Pass();
        else
            Assert.Fail();
    }

    [Test]
    public void TestInvalidRoute()
    {
        // Test einer ungültigen Route
        var response = _router.RouteRequest("GET /invalidpath HTTP/1.1", new Dictionary<string, string>(), "");
        Assert.That(response.StatusCode, Is.EqualTo(401));
    }

    [Test]
    public void TestNullRequest()
    {
        // Test mit null als Request
        var response = _router.RouteRequest(null, new Dictionary<string, string>(), "");
        Assert.That(response.StatusCode, Is.EqualTo(400));
    }
}