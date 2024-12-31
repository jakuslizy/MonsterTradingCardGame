using MonsterTradingCardGame.API.Server.DTOs;

namespace MonsterTradingCardGame.Data.Repositories.Interfaces;

public interface ISessionRepository
{
    void CreateSession(Session session);
    Session? GetSessionByToken(string token);
    void DeleteExpiredSessions();
}