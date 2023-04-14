using System.Net.Sockets;
using System.Text;

namespace TcpClientConsole;

public class TcpChatClient : IDisposable
{
    private readonly TcpClient _client;
    private StreamReader _reader;
    private StreamWriter _writer;

    private readonly string serverIp;
    private readonly int serverPort;

    public TcpChatClient(string serverIP, int serverPort)
    {
        _client = new TcpClient();
        serverIp = serverIP;
        this.serverPort = serverPort;
    }

    public bool IsConnected => _client.Connected;

    public void Dispose()
    {
        _client.Dispose();
        _reader.Dispose();
        _writer.Dispose();
    }

    public async Task ConnectAsync()
    {
        await _client.ConnectAsync(serverIp, serverPort);
        var stream = _client.GetStream();
        _reader = new StreamReader(stream, Encoding.ASCII);
        _writer = new StreamWriter(stream, Encoding.ASCII) { AutoFlush = true };
    }

    public async Task<string> ReadMessageAsync()
    {
        return await _reader.ReadLineAsync();
    }

    public async Task SendMessageAsync(string message)
    {
        await _writer.WriteLineAsync(message.Trim() + "\r");
    }
}