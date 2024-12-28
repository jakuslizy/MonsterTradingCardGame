using System.Net.Sockets;


namespace MonsterTradingCardGame.API.Server
{
    public class RequestProcessor(Router router)
    {
        public void ProcessRequest(object? clientObj)
        {
            if (clientObj is TcpClient client)
            {
                using var stream = client.GetStream();
                using var reader = new StreamReader(stream);
                using var writer = new StreamWriter(stream);
                writer.AutoFlush = true;

                var requestLine = reader.ReadLine();
                var headers = ParseHeaders(reader);
                var body = ReadBody(reader, headers);

                var response = router.RouteRequest(requestLine, headers, body);
                ResponseBuilder.SendResponse(writer, response);
            }
        }

        private Dictionary<string, string> ParseHeaders(StreamReader reader)
        {
            var headers = new Dictionary<string, string>();
            string? line;
            while (!string.IsNullOrEmpty(line = reader.ReadLine()))
            {
                var parts = line.Split(':', 2);
                if (parts.Length == 2)
                {
                    headers[parts[0].Trim()] = parts[1].Trim();
                }
            }

            return headers;
        }

        private string ReadBody(StreamReader reader, Dictionary<string, string> headers)
        {
            if (headers.TryGetValue("Content-Length", out var contentLengthStr) &&
                int.TryParse(contentLengthStr, out var contentLength))
            {
                var buffer = new char[contentLength];
                reader.Read(buffer, 0, contentLength);
                return new string(buffer);
            }

            return string.Empty;
        }
    }
}