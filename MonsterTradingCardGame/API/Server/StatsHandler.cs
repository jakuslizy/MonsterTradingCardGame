using System.Text.Json;
using MonsterTradingCardGame.API.Server.DTOs;
using MonsterTradingCardGame.Business.Services;
using MonsterTradingCardGame.Data.Repositories;
using MonsterTradingCardGame.Domain.Models;

namespace MonsterTradingCardGame.API.Server;

public class StatsHandler
{
    private readonly IUserService _userService;
    private readonly IStatsRepository _statsRepository;
    private readonly IUserRepository _userRepository;

    public StatsHandler(
        IUserService userService, 
        IStatsRepository statsRepository,
        IUserRepository userRepository)
    {
        _userService = userService;
        _statsRepository = statsRepository;
        _userRepository = userRepository;
    }
    
    public Response HandleGetStats(User user)
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

        public Response HandleScoreboard()
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
}