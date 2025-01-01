namespace MonsterTradingCardGame.API.Server.DTOs;

public class TradingRequest
{
    public string Id { get; set; } = "";
    public string CardToTrade { get; set; } = "";
    public string Type { get; set; } = "";
    public int? MinimumDamage { get; set; }
}