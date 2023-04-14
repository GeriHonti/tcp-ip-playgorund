internal class Program
{
    private static void Main(string[] args)
    {
        var port = 8888;
        if (args.Length > 0) int.TryParse(args[0], out port);

        var server = new Server(port);
        server.Start();

        Console.WriteLine("Press Enter to stop the server...");
        Console.ReadLine();
    }
}