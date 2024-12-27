using MonsterTradingCardGame.Domain.Models;

namespace MonsterTradingCardGame.API.Server;

public class BattleQueue
{
    private static readonly object _lock = new object();
    private User? _waitingPlayer;

    public User? GetWaitingPlayer()
    {
        lock (_lock)
        {
            return _waitingPlayer;
        }
    }

    public void AddPlayer(User player)
    {
        lock (_lock)
        {
            _waitingPlayer = player;
        }
    }

    public void RemovePlayer(User player)
    {
        lock (_lock)
        {
            if (_waitingPlayer?.Id == player.Id)
            {
                _waitingPlayer = null;
            }
        }
    }
} 