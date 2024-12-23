using MonsterTradingCardGame.Business.Services;
using MonsterTradingCardGame.Domain.Models;
using MonsterTradingCardGame.Data.Repositories; 

namespace MonsterTradingCardGame.API.Server
{
    public class Router
    {
        private readonly UserHandler _userHandler;
        private readonly IUserService _userService;
        private readonly IPackageService _packageService; 
        private readonly ICardService _cardService;         
        private readonly IBattleService _battleService;
        private readonly PackageRepository _packageRepository;  
        private readonly IUserRepository _userRepository;      

        public Router(
            IUserService userService, 
            ICardService cardService, 
            IBattleService battleService, 
            IPackageService packageService,
            PackageRepository packageRepository,    
            IUserRepository userRepository)         
        {
            _userHandler = new UserHandler(userService);
            _userService = userService;
            _packageService = packageService; 
            _cardService = cardService;           
            _battleService = battleService;
            _packageRepository = packageRepository;  
            _userRepository = userRepository;        
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
                ("POST", "/transactions/packages") => HandleBuyPackage(user),
                ("POST", "/packages") => HandleCreatePackage(user.Username, body),
                ("GET", "/cards") => HandleGetUserCards(user),
                ("GET", "/deck") => HandleGetDeck(user, queryParams.GetValueOrDefault("format")),
                ("PUT", "/deck") => HandleConfigureDeck(user, body),
                ("GET", var p) when p.StartsWith("/users/") => HandleGetUserData(user, p[7..]),
                ("PUT", var p) when p.StartsWith("/users/") => HandleUpdateUserData(user, p[7..], body),
                _ => new Response(404, "Not Found", "text/plain")
            };
        }

        // Neue Methode für Package-Handling
        private Response HandleCreatePackage(string username, string body)
        {
            try
            {
                _packageService.CreatePackage(body, username);
                return new Response(201, "Package created successfully", "application/json");
            }
            catch (UnauthorizedAccessException)
            {
                return new Response(403, "Forbidden - only admin can create packages", "application/json");
            }
            catch (ArgumentException ex)
            {
                return new Response(400, ex.Message, "application/json");
            }
            catch (Exception)
            {
                return new Response(500, "Internal server error", "application/json");
            }
        }
        private Response HandleBuyPackage(User user)
        {
            try 
            {
                if (user.Coins < Package.PackagePrice)
                {
                    return new Response(402, "Not enough money", "application/json");
                }

                var package = _packageRepository.GetPackage(user.Id);
                if (package == null)
                {
                    return new Response(404, "No packages available", "application/json");
                }

                // Ziehe Coins ab und füge Karten zum Stack hinzu
                user.UpdateCoins(user.Coins - Package.PackagePrice);
                foreach (var card in package.GetCards())
                {
                    user.AddCardToStack(card);
                }

                _userRepository.UpdateUserCoins(user.Id, user.Coins);
                _userRepository.SaveUserCards(user.Id, package.GetCards().ToList());

                return new Response(201, "Package successfully acquired", "application/json");
            }
            catch (Exception ex)
            {
                return new Response(500, $"Error while acquiring package: {ex.Message}", "application/json");
            }
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
                    System.Text.Json.JsonSerializer.Serialize(cardsList), 
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
                if (deck == null || !deck.Any())
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
                    Id = card.Id,
                    Name = card.Name,
                    Damage = card.Damage
                }).ToList();

                return new Response(200, 
                    System.Text.Json.JsonSerializer.Serialize(deckResponse), 
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

                var cardIds = System.Text.Json.JsonSerializer.Deserialize<List<string>>(body);
                if (cardIds == null)
                {
                    return new Response(400, "Invalid request body", "application/json");
                }

                _cardService.ConfigureDeck(user, cardIds);
                
                // Nach erfolgreicher Konfiguration die aktualisierten Karten zurückgeben
                var updatedDeck = user.Deck;
                var deckResponse = updatedDeck.Select(card => new
                {
                    Id = card.Id,
                    Name = card.Name,
                    Damage = card.Damage
                }).ToList();

                return new Response(200, 
                    System.Text.Json.JsonSerializer.Serialize(deckResponse), 
                    "application/json");
            }
            catch (System.Text.Json.JsonException)
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

        private Response HandleGetUserData(User requestingUser, string username)
        {
            try
            {
                if (requestingUser.Username != username)
                {
                    return new Response(403, "Access denied", "application/json");
                }

                var user = _userService.GetUserData(username);
                var userData = new
                {
                    Name = user.Name,
                    Bio = user.Bio,
                    Image = user.Image
                };
                
                return new Response(200, 
                    System.Text.Json.JsonSerializer.Serialize(userData), 
                    "application/json");
            }
            catch (Exception ex)
            {
                return new Response(500, $"Error retrieving user data: {ex.Message}", "application/json");
            }
        }

        private Response HandleUpdateUserData(User requestingUser, string username, string body)
        {
            try
            {
                if (requestingUser.Username != username)
                {
                    return new Response(403, "Access denied", "application/json");
                }

                var updateData = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(body);
                if (updateData == null)
                {
                    return new Response(400, "Invalid request body", "application/json");
                }

                _userService.UpdateUserData(
                    username,
                    updateData.GetValueOrDefault("Name"),
                    updateData.GetValueOrDefault("Bio"),
                    updateData.GetValueOrDefault("Image")
                );

                return new Response(200, "User data updated successfully", "application/json");
            }
            catch (System.Text.Json.JsonException)
            {
                return new Response(400, "Invalid JSON format", "application/json");
            }
            catch (Exception ex)
            {
                return new Response(500, $"Error updating user data: {ex.Message}", "application/json");
            }
        }
    }
}