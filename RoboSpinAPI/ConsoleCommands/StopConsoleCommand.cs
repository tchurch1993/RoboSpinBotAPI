using RoboSpinAPI.Interfaces;

namespace RoboSpinAPI.ConsoleCommands;

public class StopConsoleCommand : IConsoleCommand
{
    public string CommandName => "/stop";

    public string Description => "Stops the bot";

    public void Execute(IEnumerable<string> args)
    {
        Logger.ApplicationConsoleMessage("Stopping bot...");
        Environment.Exit(0);
    }
}