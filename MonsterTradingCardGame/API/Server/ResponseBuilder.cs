using System.Text;
using MonsterTradingCardGame.API.Server.DTOs;

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
                202 => "Accepted",
                400 => "Bad Request",
                401 => "Unauthorized",
                402 => "Payment Required",
                403 => "Forbidden",
                404 => "Not Found",
                409 => "Conflict",
                500 => "Internal Server Error",
                _ => "Internal Server Error"
            };
        }
    }
}