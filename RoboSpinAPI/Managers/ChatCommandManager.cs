using RoboSpinAPI.Interfaces;

namespace RoboSpinAPI.Managers;

public class ChatCommandManager
{
    private static ChatCommandManager _instance;

    private static ChatCommandManager Instance => _instance ??= new ChatCommandManager();

    private IEnumerable<IChatCommand> Commands { get; set; }

    private ChatCommandManager()
    {
        //find all classes that implement IConsoleCommand and instantiate them
        Commands = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => typeof(IChatCommand).IsAssignableFrom(p) && !p.IsInterface && !p.IsAbstract)
            .Select(t => (IChatCommand)Activator.CreateInstance(t))
            .ToArray();
    }
        
    public static IEnumerable<IChatCommand> GetCommands()
    {
        return Instance.Commands;
    }

    private static IChatCommand GetCommand(string commandName)
    {
        return Instance.Commands.FirstOrDefault(c => c.CommandName == commandName);
    }

    public static bool HasCommand(string commandName)
    {
        return Instance.Commands.Any(c => c.CommandName == commandName);
    }

    public static void ExecuteCommand(string commandName, string channel, string user, IEnumerable<string> args = null)
    {
        IChatCommand command = GetCommand(commandName);
        if (command == null)
        {
            Logger.ApplicationConsoleMessage($"Command {commandName} not found");
            return;
        }

        command.Execute(args, channel, user);
    }
}