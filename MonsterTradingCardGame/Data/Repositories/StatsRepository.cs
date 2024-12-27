using System.Data;
using MonsterTradingCardGame.Domain.Models;

namespace MonsterTradingCardGame.Data.Repositories;

public class StatsRepository
{
    private readonly DataLayer _dal = DataLayer.Instance;

    public void CreateStats(Stats stats)
    {
        using var connection = _dal.CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO stats (user_id, games_played, games_won, games_lost, elo)
            VALUES (@userId, @gamesPlayed, @gamesWon, @gamesLost, @elo)";
        
        DataLayer.AddParameterWithValue(command, "@userId", DbType.Int32, stats.UserId);
        DataLayer.AddParameterWithValue(command, "@gamesPlayed", DbType.Int32, stats.GamesPlayed);
        DataLayer.AddParameterWithValue(command, "@gamesWon", DbType.Int32, stats.GamesWon);
        DataLayer.AddParameterWithValue(command, "@gamesLost", DbType.Int32, stats.GamesLost);
        DataLayer.AddParameterWithValue(command, "@elo", DbType.Int32, stats.Elo);
        
        command.ExecuteNonQuery();
    }

    public Stats? GetStatsByUserId(int userId)
    {
        using var connection = _dal.CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM stats WHERE user_id = @userId";
        DataLayer.AddParameterWithValue(command, "@userId", DbType.Int32, userId);
        
        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new Stats(
                userId: reader.GetInt32(reader.GetOrdinal("user_id")))
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                GamesPlayed = reader.GetInt32(reader.GetOrdinal("games_played")),
                GamesWon = reader.GetInt32(reader.GetOrdinal("games_won")),
                GamesLost = reader.GetInt32(reader.GetOrdinal("games_lost")),
                Elo = reader.GetInt32(reader.GetOrdinal("elo")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at"))
            };
        }
        return null;
    }

    public void UpdateStats(Stats stats)
    {
        using var connection = _dal.CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE stats 
            SET games_played = @gamesPlayed, 
                games_won = @gamesWon, 
                games_lost = @gamesLost, 
                elo = @elo,
                updated_at = CURRENT_TIMESTAMP
            WHERE user_id = @userId";
        
        DataLayer.AddParameterWithValue(command, "@userId", DbType.Int32, stats.UserId);
        DataLayer.AddParameterWithValue(command, "@gamesPlayed", DbType.Int32, stats.GamesPlayed);
        DataLayer.AddParameterWithValue(command, "@gamesWon", DbType.Int32, stats.GamesWon);
        DataLayer.AddParameterWithValue(command, "@gamesLost", DbType.Int32, stats.GamesLost);
        DataLayer.AddParameterWithValue(command, "@elo", DbType.Int32, stats.Elo);
        
        command.ExecuteNonQuery();
    }

    public List<Stats> GetAllStats()
    {
        using var connection = _dal.CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM stats";
        
        var stats = new List<Stats>();
        using var reader = command.ExecuteReader();
    
        while (reader.Read())
        {
            stats.Add(new Stats(
                userId: reader.GetInt32(reader.GetOrdinal("user_id")))
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                GamesPlayed = reader.GetInt32(reader.GetOrdinal("games_played")),
                GamesWon = reader.GetInt32(reader.GetOrdinal("games_won")),
                GamesLost = reader.GetInt32(reader.GetOrdinal("games_lost")),
                Elo = reader.GetInt32(reader.GetOrdinal("elo")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at"))
            });
        }
        return stats;
    }
}