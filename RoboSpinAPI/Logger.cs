namespace RoboSpinAPI;

public static class Logger
{
    //colors console output from application dark cyan
    public static void ApplicationConsoleMessage(string serverMessage)
    {
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine(serverMessage);
        Console.ResetColor();
    }
    
    //colors console output from server yellow
    public static void ServerConsoleMessage(string serverMessage)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(serverMessage);
        Console.ResetColor();
    }
    
    //Default Logging Color
    public static void DefaultConsoleMessage(string serverMessage)
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine(serverMessage);
        Console.ResetColor();
    }
}