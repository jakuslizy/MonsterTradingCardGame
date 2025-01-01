using System.Text.Json;
using MonsterTradingCardGame.API.Server.DTOs;
using MonsterTradingCardGame.Business.Services.Interfaces;
using MonsterTradingCardGame.Data.Repositories.Interfaces;
using MonsterTradingCardGame.Domain.Models;

namespace MonsterTradingCardGame.API.Server.Handlers;

public class StatsHandler(
    IUserService userService,
    IStatsRepository statsRepository,
    IUserRepository userRepository)
{
    public Response HandleGetStats(User user)
    {
        try
        {
            var stats = userService.GetUserStats(user.Id);
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

    public Response HandleScoreboard()
    {
        try
        {
            var scoreboard = statsRepository.GetAllStats()
                .Where(stats => userRepository.GetUserById(stats.UserId)?.Username != "admin")
                .OrderByDescending(stats => stats.Elo)
                .Select((stats, index) =>
                {
                    var user = userRepository.GetUserById(stats.UserId);
                    var rank = index switch
                    {
                        0 => " 1st Place",
                        1 => " 2nd Place",
                        2 => " 3rd Place",
                        _ => $"{index + 1}th Place"
                    };

                    return new
                    {
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
}