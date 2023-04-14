using TcpClientConsole;

internal class Program
{
    private static async Task Main(string[] args)
    {
        Console.WriteLine("Enter the server IP address:");
        var serverIP = Console.ReadLine();

        Console.WriteLine("Enter the server port:");
        var serverPort = int.Parse(Console.ReadLine());
        
        while (true)
        {
            using var chatClient = new TcpChatClient(serverIP, serverPort);
            try
            {
                await chatClient.ConnectAsync();
                Console.WriteLine("Connected to the server.");

                var readTask = Task.Run(async () =>
                {
                    while (chatClient.IsConnected)
                    {
                        var receivedMessage = await chatClient.ReadMessageAsync();
                        if (receivedMessage != null) Console.WriteLine($"Received: {receivedMessage}");
                    }
                });

                while (chatClient.IsConnected)
                {
                    Console.Write("Enter a message: ");
                    var message = Console.ReadLine();
                    if (string.IsNullOrEmpty(message)) break;

                    await chatClient.SendMessageAsync(message);
                }

                await readTask;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            Console.WriteLine("Disconnected from the server.");
        }
    }
}