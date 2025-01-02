namespace MonsterTradingCardGame.Domain.Models;

public class Trading(string id, string cardToTrade, string type, int? minimumDamage, int userId)
{
    public string Id { get; } = id;
    public string CardToTrade { get; } = cardToTrade;
    public string Type { get; } = type;
    public int? MinimumDamage { get; } = minimumDamage;
    public int UserId { get; } = userId;
}