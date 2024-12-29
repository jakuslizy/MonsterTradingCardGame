using System.Text.Json;
using MonsterTradingCardGame.API.Server.DTOs;
using MonsterTradingCardGame.Business.Services;
using MonsterTradingCardGame.Domain.Models;

namespace MonsterTradingCardGame.API.Server;

public class CardHandler
{
    private readonly ICardService _cardService;

    public CardHandler(ICardService cardService)
    {
        _cardService = cardService;
    }
    
    public Response HandleGetUserCards(User user)
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
        public Response HandleGetDeck(User user, string? format = null)
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
        public Response HandleConfigureDeck(User user, string body)
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
}