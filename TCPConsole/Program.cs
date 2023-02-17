using System.Net.Sockets;
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

try
{
    // Set the TcpListener on port 13000.
    int port = 13000;
    string server = "localhost";

    // Create a TcpClient.
    TcpClient client = new TcpClient(server, port);

    Console.WriteLine("Connected to server on port 13000...");

    // Get a client stream for reading and writing.
    NetworkStream stream = client.GetStream();

    // Start a new thread to listen for messages from the server.
    Thread listenThread = new Thread(new ParameterizedThreadStart(ListenForMessages));
    listenThread.Start(stream);

    while (true)
    {
        Console.Write("Enter a message to send: ");

        // Read a message from the console.
        string message = Console.ReadLine();

        // Translate the passed message into ASCII and store it as a byte array.
        byte[] data = Encoding.ASCII.GetBytes(message);
        Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} {message} - {CalculateChecksum(message)}");
        // Send the message to the connected TcpServer.
        stream.Write(data, 0, data.Length);
        Console.WriteLine("Sent: {0}", message);
    }

    // Close everything.
    stream.Close();
    client.Close();
}
catch (Exception e)
{
    Console.WriteLine(e.ToString());
}


static void ListenForMessages(object obj)
{
    NetworkStream stream = (NetworkStream)obj;

    while (true)
    {
        // Check if there is any data available.
        if (stream.DataAvailable)
        {
            byte[] data = new byte[256];
            string responseData = string.Empty;

            int bytes = stream.Read(data, 0, data.Length);
            responseData = Encoding.ASCII.GetString(data, 0, bytes);
            Console.WriteLine("Received: {0}", responseData);
        }

        // Sleep for a short time to avoid busy waiting.
        Thread.Sleep(100);
    }
}