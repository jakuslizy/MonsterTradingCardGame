using System.Text.Json;
using MonsterTradingCardGame.API.Server.DTOs;
using MonsterTradingCardGame.Business.Services.Interfaces;
using MonsterTradingCardGame.Domain.Models;

namespace MonsterTradingCardGame.API.Server.Handlers;

public class CardHandler(ICardService cardService)
{
    public Response HandleGetUserCards(User user)
    {
        try
        {
            var cards = cardService.GetUserCards(user);
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
            var deck = cardService.GetUserDeck(user);
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

            cardService.ConfigureDeck(user, cardIds);

            // Nach erfolgreicher Konfiguration die aktualisierten Karten aus der DB holen
            var updatedDeck = cardService.GetUserDeck(user);
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