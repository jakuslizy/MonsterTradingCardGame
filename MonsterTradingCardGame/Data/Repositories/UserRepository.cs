using System.Data;
using MonsterTradingCardGame.Domain.Models;

namespace MonsterTradingCardGame.Data.Repositories;

public class UserRepository : IUserRepository
{
    private readonly DataLayer _dal = DataLayer.Instance;

    public void AddUser(User user)
    {
        using var command = _dal.CreateCommand(@"
            INSERT INTO users (username, password_hash, coins, elo, created_at)
            VALUES (@username, @password_hash, @coins, @elo, @created_at)
            RETURNING id");
        DataLayer.AddParameterWithValue(command, "@username", DbType.String, user.Username);
        DataLayer.AddParameterWithValue(command, "@password_hash", DbType.String, user.PasswordHash);
        DataLayer.AddParameterWithValue(command, "@coins", DbType.Int32, user.Coins);
        DataLayer.AddParameterWithValue(command, "@elo", DbType.Int32, user.Elo);
        DataLayer.AddParameterWithValue(command, "@created_at", DbType.DateTime, user.CreatedAt);
        var id = (int)(command.ExecuteScalar() ?? 0);
    }

    public User? GetUserByUsername(string username)
    {
        using var command = _dal.CreateCommand("SELECT * FROM users WHERE username = @username");
        DataLayer.AddParameterWithValue(command, "@username", DbType.String, username);
        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new User(
                id: reader.GetInt32(reader.GetOrdinal("id")),
                username: reader.GetString(reader.GetOrdinal("username")),
                passwordHash: reader.GetString(reader.GetOrdinal("password_hash")),
                createdAt: reader.GetDateTime(reader.GetOrdinal("created_at")),
                coins: reader.GetInt32(reader.GetOrdinal("coins")),
                elo: reader.GetInt32(reader.GetOrdinal("elo"))
            );
        }

        return null;
    }

    public User? GetUserById(int id)
    {
        using var command = _dal.CreateCommand("SELECT * FROM users WHERE id = @id");
        DataLayer.AddParameterWithValue(command, "@id", DbType.Int32, id);
        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new User(
                id: reader.GetInt32(reader.GetOrdinal("id")),
                username: reader.GetString(reader.GetOrdinal("username")),
                passwordHash: reader.GetString(reader.GetOrdinal("password_hash")),
                createdAt: reader.GetDateTime(reader.GetOrdinal("created_at")),
                coins: reader.GetInt32(reader.GetOrdinal("coins")),
                elo: reader.GetInt32(reader.GetOrdinal("elo"))
            );
        }

        return null;
    }

    // Todo: weitere Methoden wie UpdateUser, DeleteUser usw.
}