using System.Data;
using MonsterTradingCardGame.Domain.Models;

namespace MonsterTradingCardGame.Data.Repositories;

public class SessionRepository
{
    private readonly DataLayer _dal = DataLayer.Instance;

    public void CreateSession(Session session)
    {
        using var command = _dal.CreateCommand(@"
            INSERT INTO sessions (token, user_id, created_at, expires_at)
            VALUES (@token, @userId, @createdAt, @expiresAt)");
        DataLayer.AddParameterWithValue(command, "@token", DbType.String, session.Token);
        DataLayer.AddParameterWithValue(command, "@userId", DbType.Int32, session.UserId);
        DataLayer.AddParameterWithValue(command, "@createdAt", DbType.DateTime, session.CreatedAt);
        DataLayer.AddParameterWithValue(command, "@expiresAt", DbType.DateTime, session.ExpiresAt);
        command.ExecuteNonQuery();
    }

    public Session? GetSessionByToken(string token)
    {
        using var command = _dal.CreateCommand("SELECT * FROM sessions WHERE token = @token");
        DataLayer.AddParameterWithValue(command, "@token", DbType.String, token);
        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new Session(
                token: reader.GetString(reader.GetOrdinal("token")),
                userId: reader.GetInt32(reader.GetOrdinal("user_id")),
                createdAt: reader.GetDateTime(reader.GetOrdinal("created_at")),
                expiresAt: reader.GetDateTime(reader.GetOrdinal("expires_at"))
            );
        }

        return null;
    }

    public void DeleteExpiredSessions()
    {
        using var command = _dal.CreateCommand("DELETE FROM sessions WHERE expires_at < @now");
        DataLayer.AddParameterWithValue(command, "@now", DbType.DateTime, DateTime.UtcNow);
        command.ExecuteNonQuery();
    }
}