using System.Net.Sockets;
using System.Net;
using System.Text;

static int CalculateChecksum(string str)
{
    int sum = 0;
    foreach (char c in str)
    {
        sum += (int)c;
    }
    return sum % 256;
}
static void HandleClient(object obj)
{
    TcpClient client = (TcpClient)obj;

    // Buffer for reading data.
    byte[] bytes = new byte[256];
    string data = null;

    // Get a stream object for reading and writing.
    NetworkStream stream = client.GetStream();

    int i;

    // Loop to receive all the data sent by the client.
    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
    {
        // Translate data bytes to a ASCII string.
        data = Encoding.ASCII.GetString(bytes, 0, i);
        Console.WriteLine("Received: {0}", data);

        var checksum = CalculateChecksum(data);
        // Process the data sent by the client.
        data = data.ToUpper();

        byte[] msg = Encoding.ASCII.GetBytes(checksum.ToString());

        // Send back a response.
        stream.Write(msg, 0, msg.Length);
        Console.WriteLine("Sent: {0}", data);
    }

    // Shutdown and end connection.
    client.Close();
}

TcpListener server = null;
try
{
    // Set the TcpListener on port 13000.
    int port = 13000;
    IPAddress localAddr = IPAddress.Parse("127.0.0.1");

    // TcpListener server = new TcpListener(port);
    server = new TcpListener(localAddr, port);

    // Start listening for client requests.
    server.Start();

    Console.WriteLine("Server is running on port 13000...");

    // Enter the listening loop.
    while (true)
    {
        Console.Write("Waiting for a connection... ");

        // Perform a blocking call to accept requests.
        // You could also use server.AcceptSocket() here.
        TcpClient client = server.AcceptTcpClient();
        Console.WriteLine("Connected!");

        // Start a new thread to handle the client connection.
        Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClient));
        clientThread.Start(client);
    }
}
catch (SocketException e)
{
    Console.WriteLine("SocketException: {0}", e);
}
finally
{
    // Stop listening for new clients.
    server.Stop();
}

Console.WriteLine("\nHit enter to continue...");
Console.Read();


