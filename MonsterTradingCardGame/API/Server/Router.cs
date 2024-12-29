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
                ("GET", "/cards") => HandleGetUserCards(user),
                ("GET", "/deck") => HandleGetDeck(user, queryParams.GetValueOrDefault("format")),
                ("PUT", "/deck") => HandleConfigureDeck(user, body),
                ("GET", var p) when p.StartsWith("/users/") => _userHandler.HandleGetUserData(user, p[7..]),
                ("PUT", var p) when p.StartsWith("/users/") => _userHandler.HandleUpdateUserData(user, p[7..], body),
                ("GET", "/stats") => HandleGetStats(user),
                ("GET", "/scoreboard") => HandleScoreboard(),
                ("POST", "/battles") => HandleBattle(user),
                ("GET", "/tradings") => _tradingHandler.HandleGetTradings(),
                ("POST", "/tradings") => _tradingHandler.HandleCreateTrading(user, body),
                ("POST", var p) when p.StartsWith("/tradings/") => _tradingHandler.HandleExecuteTrading(user, p[10..], body),
                ("DELETE", var p) when p.StartsWith("/tradings/") => _tradingHandler.HandleDeleteTrading(user, p[10..]),
                _ => new Response(404, "Not Found", "text/plain")
            };
        }

        private Response HandleGetUserCards(User user)
        {
            try 
            {
                var cards = _cardService.GetUserCards(user);
                if (!cards.Any())
                {
                    return new Response(200, "[]", "application/json");
                }

                var cardsList = cards.Select(card => new
                {
                    card.Id,
                    card.Name,
                    card.Damage
                });

                return new Response(200, 
                    JsonSerializer.Serialize(cardsList), 
                    "application/json");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in HandleGetUserCards: {ex}");
                return new Response(500, $"Error retrieving cards: {ex.Message}", "application/json");
            }
        }
        private Response HandleGetDeck(User user, string? format = null)
        {
            try
            {
                var deck = _cardService.GetUserDeck(user);
                if (!deck.Any())
                {
                    return new Response(200, "[]", "application/json");
                }

                if (format?.ToLower() == "plain")
                {
                    var plainText = string.Join("\n", deck.Select(card => 
                        $"Card: {card.Name} ({card.Id}), Damage: {card.Damage}"));
                    return new Response(200, plainText, "text/plain");
                }

                var deckResponse = deck.Select(card => new
                {
                    card.Id,
                    card.Name,
                    card.Damage
                }).ToList();

                return new Response(200, 
                    JsonSerializer.Serialize(deckResponse), 
                    "application/json");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in HandleGetDeck: {ex}");
                return new Response(500, "Internal server error", "application/json");
            }
        }
        private Response HandleConfigureDeck(User user, string body)
        {
            try
            {
                if (string.IsNullOrEmpty(body))
                {
                    return new Response(400, "Request body is empty", "application/json");
                }

                var cardIds = JsonSerializer.Deserialize<List<string>>(body);
                if (cardIds == null)
                {
                    return new Response(400, "Invalid request body", "application/json");
                }

                _cardService.ConfigureDeck(user, cardIds);
                
                // Nach erfolgreicher Konfiguration die aktualisierten Karten aus der DB holen
                var updatedDeck = _cardService.GetUserDeck(user);
                var deckResponse = updatedDeck.Select(card => new
                {
                    card.Id,
                    card.Name,
                    card.Damage
                }).ToList();

                return new Response(200, 
                    JsonSerializer.Serialize(deckResponse), 
                    "application/json");
            }
            catch (JsonException)
            {
                return new Response(400, "Invalid JSON format", "application/json");
            }
            catch (InvalidOperationException ex)
            {
                return new Response(400, ex.Message, "application/json");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in HandleConfigureDeck: {ex}");
                return new Response(500, "Internal server error", "application/json");
            }
        }
        private Response HandleGetStats(User user)
        {
            try
            {
                var stats = _userService.GetUserStats(user.Id);
                var statsResponse = new
                {
                    Name = user.Username,
                    ELO = stats.Elo,
                    stats.GamesPlayed,
                    stats.GamesWon,
                    stats.GamesLost,
                    WinRate = stats.GamesPlayed > 0 
                        ? (stats.GamesWon * 100.0 / stats.GamesPlayed).ToString("F1") + "%" 
                        : "0%"
                };

                return new Response(200, 
                    JsonSerializer.Serialize(statsResponse), 
                    "application/json");
            }
            catch (Exception ex)
            {
                return new Response(500, $"Error retrieving stats: {ex.Message}", "application/json");
            }
        }

        private Response HandleScoreboard()
        {
            try
            {
                var scoreboard = _statsRepository.GetAllStats()
                    .OrderByDescending(s => s.Elo)
                    .Select((stats, index) => 
                    {
                        var user = _userRepository.GetUserById(stats.UserId);
                        string rank = (index + 1) switch
                        {
                            1 => " 1st Place",
                            2 => " 2nd Place", 
                            3 => " 3rd Place",
                            _ => $"#{index + 1}"
                        };
                
                        return new { 
                            Rank = rank,
                            Name = user?.Username,
                            stats.Elo,
                            stats.GamesPlayed,
                            WinRate = stats.GamesPlayed > 0 
                                ? (stats.GamesWon * 100.0 / stats.GamesPlayed).ToString("F1") + "%" 
                                : "0%"
                        };
                    })
                    .ToList();

                // JSON mit ordentlicher Formatierung
                var options = new JsonSerializerOptions { WriteIndented = true };
                return new Response(200, 
                    JsonSerializer.Serialize(scoreboard, options), 
                    "application/json");
            }
            catch (Exception ex)
            {
                return new Response(500, ex.Message, "application/json");
            }
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