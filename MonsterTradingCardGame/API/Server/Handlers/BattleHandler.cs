using System.Text.Json;
using MonsterTradingCardGame.API.Server.DTOs;
using MonsterTradingCardGame.Business.Services.Interfaces;
using MonsterTradingCardGame.Domain.Models;

namespace MonsterTradingCardGame.API.Server.Handlers;

public class BattleHandler(IBattleService battleService, BattleQueue battleQueue)
{
    public Response HandleBattle(User user)
    {
        try
        {
            // Prüfen ob ein anderer Spieler wartet
            var waitingPlayer = battleQueue.GetWaitingPlayer();

            if (waitingPlayer == null)
            {
                // Wenn kein Spieler wartet, füge aktuellen Spieler zur Queue hinzu
                battleQueue.AddPlayer(user);
                return new Response(202, JsonSerializer.Serialize(new { Message = "Waiting for opponent" }),
                    "application/json");
            }

            if (waitingPlayer.Id == user.Id)
            {
                return new Response(400, "Cannot battle against yourself", "application/json");
            }

            // Battle durchführen
            var battleLog = battleService.ExecuteBattle(waitingPlayer, user);
            battleQueue.RemovePlayer(waitingPlayer);

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