using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MonsterTradingCardGame.API.Server;

public class HttpServer
{
    private TcpListener _server;
    private readonly int _port;

    // Konstruktor: Server mit einem bestimmten Port
    public HttpServer(int port, TcpListener server)
    {
        _port = port;
        _server = server;
    }

    // Start mit dem Abhören von Verbindungen
    public void Start()
    {
        _server = new TcpListener(IPAddress.Any, _port);
        _server.Start();
        Console.WriteLine($"Server: use http://localhost:{_port}/");

        while (true)
        {
            HandleClient();
        }
    }

    // Verarbeitet eine einzelne Client-Verbindung
    private void HandleClient()
    {
        using var client = _server.AcceptTcpClient();
        using var writer = new StreamWriter(client.GetStream()) { AutoFlush = true };
        using var reader = new StreamReader(client.GetStream());

        // Parst die erste Zeile der HTTP-Anfrage
        var (method, path, version) = ParseRequestLine(reader.ReadLine());
        Console.WriteLine($"Method: {method}, Path: {path}, Version: {version}");

        // Liest und verarbeitet die HTTP-Header
        var headers = ParseHeaders(reader);
        // Liest den Body der Anfrage, falls vorhanden
        var body = ReadBody(reader, headers);

        // Sendet eine Standard-Antwort zurück an den Client
        SendResponse(writer);
    }

    // Zerlegt die erste Zeile der HTTP-Anfrage in Methode, Pfad und HTTP-Version
    private (string method, string path, string version) ParseRequestLine(string? line)
    {
        var parts = line?.Split(' ');
        return (parts?[0] ?? "", parts?[1] ?? "", parts?[2] ?? "");
    }

    // Liest und verarbeitet die HTTP-Header
    private Dictionary<string, string> ParseHeaders(StreamReader reader)
    {
        var headers = new Dictionary<string, string>();
        string? line;
        // Liest Zeilen, bis eine leere Zeile auftritt (Ende der Header)
        while ((line = reader.ReadLine()) != null && line.Length > 0)
        {
            var parts = line.Split(':', 2);
            if (parts.Length == 2)
            {
                headers[parts[0]] = parts[1].Trim();
            }
        }

        return headers;
    }

    // Liest den Body der Anfrage, basierend auf dem Content-Length Header
    private string ReadBody(StreamReader reader, Dictionary<string, string> headers)
    {
        if (headers.TryGetValue("Content-Length", out var contentLengthStr) &&
            int.TryParse(contentLengthStr, out var contentLength) &&
            contentLength > 0)
        {
            var buffer = new char[contentLength];
            reader.Read(buffer, 0, contentLength);
            return new string(buffer);
        }

        return string.Empty;
    }

    // Sendet eine einfache HTTP-Antwort zurück an den Client
    private void SendResponse(StreamWriter writer)
    {
        string responseBody = "<html><body><h1>Hello from the MTCG Server!</h1></body></html>";
        writer.WriteLine("HTTP/1.1 200 OK");
        writer.WriteLine("Content-Type: text/html; charset=utf-8");
        writer.WriteLine($"Content-Length: {Encoding.UTF8.GetByteCount(responseBody)}");
        writer.WriteLine(); // Leere Zeile markiert das Ende der Header
        writer.WriteLine(responseBody);
    }
}