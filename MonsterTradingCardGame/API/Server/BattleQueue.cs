using MonsterTradingCardGame.Domain.Models;


namespace MonsterTradingCardGame.API.Server;

public class BattleQueue
{
    private static readonly object Lock = new object();
    private User? _waitingPlayer;

    public User? GetWaitingPlayer()
    {
        lock (Lock)
        {
            return _waitingPlayer;
        }
    }

    public void AddPlayer(User player)
    {
        lock (Lock)
        {
            _waitingPlayer = player;
        }
    }

    public void RemovePlayer(User player)
    {
        lock (Lock)
        {
            if (_waitingPlayer?.Id == player.Id)
            {
                _waitingPlayer = null;
            }
        }
    }
}