using System.Net;
using System.Net.Sockets;


namespace MonsterTradingCardGame.API.Server
{
    public class HttpServer(int port, RequestProcessor requestProcessor, TcpListener server)
    {
        private TcpListener _server = server;
        private bool _isRunning;

        public void Stop()
        {
            _isRunning = false;
            _server.Stop();
        }

        public void Start()
        {
            _isRunning = true;
            _server = new TcpListener(IPAddress.Any, port);
            Console.WriteLine("Starting server...");
            _server.Start();
            Console.WriteLine($"Server: use http://localhost:{port}/");

            while (true)
            {
                TcpClient client = _server.AcceptTcpClient();
                ThreadPool.QueueUserWorkItem(requestProcessor.ProcessRequest, client);
            }
        }
    }
}