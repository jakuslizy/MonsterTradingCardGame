namespace MonsterTradingCardGame.Domain.Models;

public class Response(int statusCode, string content, string contentType)
{
    public int StatusCode { get; } = statusCode;
    public string Content { get; } = content;
    public string ContentType { get; } = contentType;
}