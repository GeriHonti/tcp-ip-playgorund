using System.IO;
using System.Net.Sockets;
using System.Text;

static int CalculateChecksum(string str)
{
    var sum = str.Aggregate(0, (current, c) => current + c);
    return sum % 256;
}

try
{
    // Set the TcpListener on port 13000.
    var port = 13000;
    var server = "localhost";

    // Create a TcpClient.
    var client = new TcpClient(server, port);

    Console.WriteLine("Connected to server on port 13000...");

    // Get a client stream for reading and writing.
    var stream = client.GetStream();

    // Start a new thread to listen for messages from the server.
    var listenThread = new Thread(ListenForMessages);
    listenThread.Start(stream);

    while (true)
    {
        Console.Write("Enter a message to send: ");

        // Read a message from the console.
        var message = Console.ReadLine();
        if (message == "/quit")
        {
            break;
        }
       
        // Translate the passed message into ASCII and store it as a byte array.
        var data = Encoding.ASCII.GetBytes(message);
        Console.WriteLine(
            $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} {message} - {CalculateChecksum(message)}");
        // Send the message to the connected TcpServer.
        stream.Write(data, 0, data.Length);
        Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} Sent: {message}");
    }
    stream.Close();
    client.Close();

}
catch (Exception e)
{
    Console.WriteLine(e.ToString());
}


static void ListenForMessages(object obj)
{
    var stream = (NetworkStream)obj;

    while (true)
    {
        // Check if there is any data available.
        if (stream.DataAvailable)
        {
            var data = new byte[256];

            var bytes = stream.Read(data, 0, data.Length);
            var responseData = Encoding.ASCII.GetString(data, 0, bytes);
            Console.WriteLine("Received: {0}", responseData);
        }

        // Sleep for a short time to avoid busy waiting.
        Thread.Sleep(100);
    }
}