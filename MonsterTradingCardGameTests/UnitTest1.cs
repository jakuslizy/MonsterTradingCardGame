using MonsterTradingCardGame.API.Server;
using MonsterTradingCardGame.Business.Services;
using MonsterTradingCardGame.Data.Repositories;
using NSubstitute;

namespace MonsterTradingCardGameTests;

public class Tests
{
    private Router _router;
    private IUserService _userService;
    private ICardService _cardService;
    private IBattleService _battleService;
    private IUserRepository _userRepository;
    private SessionRepository _sessionRepository;

    [SetUp]
    public void Setup()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _sessionRepository = Substitute.For<SessionRepository>();
        
        _userService = Substitute.For<IUserService>(); // Substitute verwenden
        _cardService = Substitute.For<ICardService>();
        _battleService = Substitute.For<IBattleService>();
        
        _router = new Router(_userService, _cardService, _battleService);
    }

    [Test]
    public void TestHomeRoute()
    {
        var response = _router.RouteRequest("GET / HTTP/1.1", new Dictionary<string, string>(), "");
        Assert.IsNotNull(response, "The response should not be null.");
        Assert.That(response.StatusCode, Is.EqualTo(200), "The Home route should return status code 200.");
    }

    [Test]
    public void TestUsersRoute()
    {
        var response = _router.RouteRequest("POST /users HTTP/1.1", new Dictionary<string, string>(), "");
        Assert.IsNotNull(response, "The response should not be null");
        Assert.That(response.StatusCode, Is.EqualTo(400), "The Users route should return status code 400 (empty body)");
    }

    [Test]
    public void TestInvalidRoute()
    {
        var response = _router.RouteRequest("GET /invalidpath HTTP/1.1", new Dictionary<string, string>(), "");
        Assert.That(response.StatusCode, Is.EqualTo(401));
    }

    [Test]
    public void TestNullRequest()
    {
        var response = _router.RouteRequest(null, new Dictionary<string, string>(), "");
        Assert.That(response.StatusCode, Is.EqualTo(400));
    }
}
