using System.Text.Json;
using MonsterTradingCardGame.API.Server.DTOs;
using MonsterTradingCardGame.Business.Services;
using MonsterTradingCardGame.Domain.Models;

namespace MonsterTradingCardGame.API.Server;

public class TradingHandler
{
    private readonly ITradingService _tradingService;

    public TradingHandler(ITradingService tradingService)
    {
        _tradingService = tradingService;
    }

    public Response HandleGetTradings()
        {
            try
            {
                var trades = _tradingService.GetTrades();
                return new Response(200, JsonSerializer.Serialize(trades), "application/json");
            }
            catch (Exception ex)
            {
                return new Response(500, ex.Message, "application/json");
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
                _tradingService.CreateTrade(
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
                _tradingService.ExecuteTrade(tradeId, offeredCardId, user);
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
                _tradingService.DeleteTrade(tradeId, user);
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