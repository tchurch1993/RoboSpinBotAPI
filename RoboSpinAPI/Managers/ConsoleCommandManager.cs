using RoboSpinAPI.Interfaces;

namespace RoboSpinAPI.Managers;

public class ConsoleCommandManager
{
        private static ConsoleCommandManager _instance;

        private static ConsoleCommandManager Instance => _instance ??= new ConsoleCommandManager();

        private IConsoleCommand[] Commands { get; set; }

        private ConsoleCommandManager()
        {
            //find all classes that implement IConsoleCommand and instantiate them
            Commands = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => typeof(IConsoleCommand).IsAssignableFrom(p) && !p.IsInterface && !p.IsAbstract)
                .Select(t => (IConsoleCommand)Activator.CreateInstance(t))
                .ToArray();
        }
        
        public static IEnumerable<IConsoleCommand> GetCommands()
        {
            return Instance.Commands;
        }

        private static IConsoleCommand GetCommand(string commandName)
        {
            return Instance.Commands.FirstOrDefault(c => c.CommandName == commandName);
        }

        public static bool HasCommand(string commandName)
        {
            return Instance.Commands.Any(c => c.CommandName == commandName);
        }

        public static void ExecuteCommand(string commandName, IEnumerable<string> args = null)
        {
            IConsoleCommand command = GetCommand(commandName);
            if (command == null)
            {
                Logger.ApplicationConsoleMessage($"Command {commandName} not found");
                return;
            }

            command.Execute(args);
        }

}