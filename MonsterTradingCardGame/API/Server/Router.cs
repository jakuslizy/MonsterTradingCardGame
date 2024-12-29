using System.Text.Json;
using MonsterTradingCardGame.API.Server.DTOs;
using MonsterTradingCardGame.API.Server.Handlers;
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
        private readonly BattleHandler _battleHandler;

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
            _battleHandler = new BattleHandler(battleService, battleQueue);
            _userService = userService;
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
                // Package routes
                ("POST", "/transactions/packages") => _packageHandler.HandleBuyPackage(user),
                ("POST", "/packages") => _packageHandler.HandleCreatePackage(user.Username, body),
            
                // Card routes
                ("GET", "/cards") => _cardHandler.HandleGetUserCards(user),
                ("GET", "/deck") => _cardHandler.HandleGetDeck(user, queryParams.GetValueOrDefault("format")),
                ("PUT", "/deck") => _cardHandler.HandleConfigureDeck(user, body),
            
                // User routes
                ("GET", var p) when p.StartsWith("/users/") => _userHandler.HandleGetUserData(user, p[7..]),
                ("PUT", var p) when p.StartsWith("/users/") => _userHandler.HandleUpdateUserData(user, p[7..], body),
            
                // Stats routes
                ("GET", "/stats") => _statsHandler.HandleGetStats(user),
                ("GET", "/scoreboard") => _statsHandler.HandleScoreboard(),
            
                // Battle route
                ("POST", "/battles") => _battleHandler.HandleBattle(user),
            
                // Trading routes
                ("GET", "/tradings") => _tradingHandler.HandleGetTradings(),
                ("POST", "/tradings") => _tradingHandler.HandleCreateTrading(user, body),
                ("POST", var p) when p.StartsWith("/tradings/") => _tradingHandler.HandleExecuteTrading(user, p[10..], body),
                ("DELETE", var p) when p.StartsWith("/tradings/") => _tradingHandler.HandleDeleteTrading(user, p[10..]),
            
                _ => new Response(404, "Not Found", "text/plain")
            };
        }
        

        
    }
}