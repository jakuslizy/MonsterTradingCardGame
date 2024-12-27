public class Trading
{
    public string Id { get; }
    public string CardToTrade { get; }
    public string Type { get; }
    public int? MinimumDamage { get; }
    public int UserId { get; }

    public Trading(string id, string cardToTrade, string type, int? minimumDamage, int userId)
    {
        Id = id;
        CardToTrade = cardToTrade;
        Type = type;
        MinimumDamage = minimumDamage;
        UserId = userId;
    }
} 