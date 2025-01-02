namespace MonsterTradingCardGame.API.Server.DTOs;

public class TradingRequest
{
    public string Id { get; init; } = "";
    public string CardToTrade { get; init; } = "";
    public string Type { get; init; } = "";
    public int? MinimumDamage { get; init; }
}