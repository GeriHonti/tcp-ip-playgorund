using System.Net;
using System.Net.Sockets;
using System.Text;

public class Server
{
    private readonly TcpListener _listener;
    private int _messageCount;

    public Server(int port)
    {
        _listener = new TcpListener(IPAddress.Any, port);
        _messageCount = 0;
    }

    public void Start()
    {
        _listener.Start();
        Console.WriteLine("Server started...");
        ThreadPool.QueueUserWorkItem(ListenerThread);
    }

    private void ListenerThread(object state)
    {
        while (true)
        {
            var client = _listener.AcceptTcpClient();
            Console.WriteLine("Client connected.");
            ThreadPool.QueueUserWorkItem(HandleClient, client);
        }
    }

    private void HandleClient(object state)
    {
        using (var client = (TcpClient)state)
        {
            var stream = client.GetStream();
            var buffer = new byte[1024];
            int bytesRead;

            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                _messageCount++;
                var message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"Received: {message}");

                if (_messageCount % 2 == 0)
                {
                    Console.WriteLine("Server disconnecting after 2nd message...");
                    break;
                }
            }
        }

        Console.WriteLine("Client disconnected.");
    }
}