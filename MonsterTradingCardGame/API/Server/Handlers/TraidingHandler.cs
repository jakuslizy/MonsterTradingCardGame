using System.Text.Json;
using MonsterTradingCardGame.API.Server.DTOs;
using MonsterTradingCardGame.Business.Services.Interfaces;
using MonsterTradingCardGame.Domain.Models;
using System.Text;

namespace MonsterTradingCardGame.API.Server.Handlers;

public class TradingHandler(ITradingService tradingService)
{
    public Response HandleGetTradings(string? format = null)
    {
        try
        {
            var trades = tradingService.GetTrades();
            if (!trades.Any())
            {
                return new Response(200, "[]", "application/json");
            }

            // Plain Text Format
            if (format?.ToLower() == "plain")
            {
                var plainText = new StringBuilder();
                plainText.AppendLine("=== Verfügbare Trading Deals ===\n");
                
                var sortedTrades = trades.OrderBy(t => t.Id);
                int counter = 1;
                
                foreach (var trade in sortedTrades)
                {
                    plainText.AppendLine(
                        $"Deal {counter}:" +
                        $"\n   ID: {trade.Id}" +
                        $"\n   Angebotene Karte: {trade.CardToTrade}" +
                        $"\n   Gewünschter Typ: {trade.Type}" +
                        $"\n   Mindestschaden: {trade.MinimumDamage ?? 0}" +
                        $"\n   Anbieter: {trade.UserId}\n");
                    counter++;
                }
                
                return new Response(200, plainText.ToString(), "text/plain");
            }

            // JSON Format (mit Einrückung)
            var tradingList = trades.Select((trade, index) => new
            {
                Number = index + 1,
                trade.Id,
                CardToTrade = trade.CardToTrade,
                Type = trade.Type,
                MinimumDamage = trade.MinimumDamage ?? 0,
                UserId = trade.UserId
            })
            .OrderBy(t => t.Number)
            .ToList();

            var options = new JsonSerializerOptions { WriteIndented = true };
            return new Response(200,
                JsonSerializer.Serialize(tradingList, options),
                "application/json");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in HandleGetTradings: {ex}");
            return new Response(500, "Internal server error", "application/json");
        }
    }

    public Response HandleCreateTrading(User user, string body)
    {
        try
        {
            var tradingRequest = JsonSerializer.Deserialize<TradingRequest>(body);
            if (tradingRequest == null)
            {
                return new Response(400, "Invalid request body", "application/json");
            }

            tradingService.CreateTrade(
                tradingRequest.Id,
                tradingRequest.CardToTrade,
                tradingRequest.Type,
                tradingRequest.MinimumDamage,
                user
            );
            return new Response(201, "Trading deal created", "application/json");
        }
        catch (InvalidOperationException ex)
        {
            return new Response(403, ex.Message, "application/json");
        }
        catch (Exception ex)
        {
            return new Response(500, ex.Message, "application/json");
        }
    }

    public Response HandleExecuteTrading(User user, string tradeId, string body)
    {
        try
        {
            var offeredCardId = JsonSerializer.Deserialize<string>(body);
            if (offeredCardId == null)
            {
                return new Response(400, "Invalid request body", "application/json");
            }

            tradingService.ExecuteTrade(tradeId, offeredCardId, user);
            return new Response(201, "Trading deal executed successfully", "application/json");
        }
        catch (InvalidOperationException ex)
        {
            return new Response(403, ex.Message, "application/json");
        }
        catch (Exception ex)
        {
            return new Response(500, ex.Message, "application/json");
        }
    }

    public Response HandleDeleteTrading(User user, string tradeId)
    {
        try
        {
            tradingService.DeleteTrade(tradeId, user);
            return new Response(200, "Trading deal deleted successfully", "application/json");
        }
        catch (InvalidOperationException ex)
        {
            return new Response(403, ex.Message, "application/json");
        }
        catch (Exception ex)
        {
            return new Response(500, ex.Message, "application/json");
        }
    }
}