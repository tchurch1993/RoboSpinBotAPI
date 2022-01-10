using RoboSpinAPI.Interfaces;
using RoboSpinAPI.Managers;

namespace RoboSpinAPI.ConsoleCommands;

public class HelpConsoleCommand : IConsoleCommand
{
    public string CommandName => "/help";

    public string Description => "Shows help for all commands";

    public void Execute(IEnumerable<string> args)
    {
        var commands = ConsoleCommandManager.GetCommands().ToList();
        commands.Sort((x, y) => string.Compare(x.CommandName, y.CommandName, StringComparison.Ordinal));

        foreach (IConsoleCommand command in commands)
        {
            Logger.ApplicationConsoleMessage($"{command.CommandName} - {command.Description}");
        }
    }
}