namespace MonsterTradingCardGame.API.Server.DTOs;

public class Request
{
    //required, weil bei der erstellung gesetzt werden müssen
    public required string Method { get; init; }
    public required string Path { get; init; }
    public string? Body { get; init; }
    public Dictionary<string, string> QueryParameters { get; init; } = new();
}