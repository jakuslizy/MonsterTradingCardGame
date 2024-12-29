using System.Text.Json;
using MonsterTradingCardGame.API.Server.DTOs;
using MonsterTradingCardGame.Business.Services;
using MonsterTradingCardGame.Data.Repositories;
using MonsterTradingCardGame.Domain.Models;

namespace MonsterTradingCardGame.API.Server
{
    public class Router
    {
        private readonly UserHandler _userHandler;
        private readonly PackageHandler _packageHandler;
        private readonly TradingHandler _tradingHandler;
        private readonly IUserService _userService;
        private readonly CardHandler _cardHandler;
        private readonly StatsHandler _statsHandler;
        private readonly ICardService _cardService;         
        private readonly IBattleService _battleService;
        private readonly IPackageRepository _packageRepository;  
        private readonly IUserRepository _userRepository; 
        private readonly IStatsRepository _statsRepository;
        private readonly BattleQueue _battleQueue;
        private readonly ITradingService _tradingService;

        public Router(
            IUserService userService, 
            ICardService cardService, 
            IBattleService battleService, 
            IPackageService packageService,
            IPackageRepository packageRepository,
            IUserRepository userRepository,
            IStatsRepository statsRepository,
            BattleQueue battleQueue,
            ITradingService tradingService)         
        {
            _userHandler = new UserHandler(userService);
            _packageHandler = new PackageHandler(packageService, userRepository, packageRepository);
            _tradingHandler = new TradingHandler(tradingService);
            _cardHandler = new CardHandler(cardService);
            _statsHandler = new StatsHandler(userService, statsRepository, userRepository);
            _userService = userService;
            _cardService = cardService;           
            _battleService = battleService;
            _packageRepository = packageRepository;  
            _userRepository = userRepository; 
            _statsRepository = statsRepository;
            _battleQueue = battleQueue;
            _tradingService = tradingService;
        }

        public Response RouteRequest(string? requestLine, Dictionary<string, string> headers, string body)
        {
            if (string.IsNullOrEmpty(requestLine))
                return new Response(400, "Bad Request", "text/plain");

            var parts = requestLine.Split(' ');
            if (parts.Length < 2)
                return new Response(400, "Bad Request", "text/plain");

            var method = parts[0];
            var pathAndQuery = parts[1].Split('?');
            var path = pathAndQuery[0];
            
            // Query-Parameter extrahieren
            var queryParams = new Dictionary<string, string>();
            if (pathAndQuery.Length > 1)
            {
                var queryParts = pathAndQuery[1].Split('&');
                foreach (var param in queryParts)
                {
                    var keyValue = param.Split('=');
                    if (keyValue.Length == 2)
                    {
                        queryParams[keyValue[0]] = keyValue[1];
                    }
                }
            }

            // Nicht-geschützte Routen
            if (method == "POST" && path == "/users")
                return _userHandler.RegisterUser(new Request { 
                    Body = body, 
                    Path = "/users", 
                    Method = "POST",
                    QueryParameters = queryParams 
                });
    
            if (method == "POST" && path == "/sessions")
                return _userHandler.LoginUser(new Request { Body = body, Path = "/sessions", Method = "POST" });
    
            if (method == "GET" && path == "/")
                return new Response(200, "<html><body><h1>Willkommen beim Monster Trading Card Game</h1></body></html>", "text/html");

            // Alle anderen Routen sind geschützt
            return HandleProtectedRoute(method, path, headers, body, queryParams);
        }

        private Response HandleProtectedRoute(string method, string path, Dictionary<string, string> headers, string body, Dictionary<string, string> queryParams)
        {
            // Token-Validierung
            if (!headers.TryGetValue("Authorization", out var authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return new Response(401, "Unauthorized: No valid token provided", "application/json");
            }

            var token = authHeader.Substring("Bearer ".Length);
            if (!_userService.ValidateToken(token))
            {
                return new Response(401, "Unauthorized: Invalid or expired token", "application/json");
            }

            var user = _userService.GetUserFromToken(token);

            // Router für geschützte Endpunkte
            return (method, path) switch
            {
                ("POST", "/transactions/packages") => _packageHandler.HandleBuyPackage(user),
                ("POST", "/packages") => _packageHandler.HandleCreatePackage(user.Username, body),
                ("GET", "/cards") => _cardHandler.HandleGetUserCards(user),
                ("GET", "/deck") => _cardHandler.HandleGetDeck(user, queryParams.GetValueOrDefault("format")),
                ("PUT", "/deck") => _cardHandler.HandleConfigureDeck(user, body),
                ("GET", var p) when p.StartsWith("/users/") => _userHandler.HandleGetUserData(user, p[7..]),
                ("PUT", var p) when p.StartsWith("/users/") => _userHandler.HandleUpdateUserData(user, p[7..], body),
                ("GET", "/stats") => _statsHandler.HandleGetStats(user),
                ("GET", "/scoreboard") => _statsHandler.HandleScoreboard(),
                ("POST", "/battles") => HandleBattle(user),
                ("GET", "/tradings") => _tradingHandler.HandleGetTradings(),
                ("POST", "/tradings") => _tradingHandler.HandleCreateTrading(user, body),
                ("POST", var p) when p.StartsWith("/tradings/") => _tradingHandler.HandleExecuteTrading(user, p[10..], body),
                ("DELETE", var p) when p.StartsWith("/tradings/") => _tradingHandler.HandleDeleteTrading(user, p[10..]),
                _ => new Response(404, "Not Found", "text/plain")
            };
        }
        

        private Response HandleBattle(User user)
        {
            try
            {
                // Prüfen ob ein anderer Spieler wartet
                var waitingPlayer = _battleQueue.GetWaitingPlayer();
                
                if (waitingPlayer == null)
                {
                    // Wenn kein Spieler wartet, füge aktuellen Spieler zur Queue hinzu
                    _battleQueue.AddPlayer(user);
                    return new Response(202, JsonSerializer.Serialize(new { Message = "Waiting for opponent" }), "application/json");
                }
                
                if (waitingPlayer.Id == user.Id)
                {
                    return new Response(400, "Cannot battle against yourself", "application/json");
                }

                // Battle durchführen
                var battleLog = _battleService.ExecuteBattle(waitingPlayer, user);
                _battleQueue.RemovePlayer(waitingPlayer);
                
                return new Response(200, battleLog, "text/plain");
            }
            catch (InvalidOperationException ex)
            {
                return new Response(400, ex.Message, "application/json");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in battle: {ex}");
                return new Response(500, "Internal server error", "application/json");
            }
        }
    }
}