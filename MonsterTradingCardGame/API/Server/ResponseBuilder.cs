using System.Text;
using MonsterTradingCardGame.Domain.Models;

namespace MonsterTradingCardGame.API.Server
{
    public static class ResponseBuilder
    {
        public static void SendResponse(StreamWriter writer, Response response)
        {
            writer.WriteLine($"HTTP/1.1 {response.StatusCode} {GetStatusDescription(response.StatusCode)}");
            writer.WriteLine($"Content-Type: {response.ContentType}");
            writer.WriteLine($"Content-Length: {Encoding.UTF8.GetByteCount(response.Content)}");
            writer.WriteLine();
            writer.WriteLine(response.Content);
        }

        private static string GetStatusDescription(int statusCode)
        {
            return statusCode switch
            {
                200 => "OK",
                201 => "Created",
                400 => "Bad Request",
                401 => "Unauthorized",
                404 => "Not Found",
                _ => "Internal Server Error"
            };
        }
    }
}