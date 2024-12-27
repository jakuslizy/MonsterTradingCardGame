namespace MonsterTradingCardGame.Domain.Models;

public class TradingRequest
{
    public string Id { get; set; } = "";
    public string CardToTrade { get; set; } = "";
    public string Type { get; set; } = "";
    public int? MinimumDamage { get; set; }
} 