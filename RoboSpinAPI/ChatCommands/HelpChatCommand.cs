using System.Text;
using RoboSpinAPI.Interfaces;
using RoboSpinAPI.Managers;

namespace RoboSpinAPI.ChatCommands;

public class HelpChatCommand : IChatCommand
{
    public void Execute(IEnumerable<string> args, string channel, string user)
    {
        var commands = ChatCommandManager.GetCommands().ToList();
        commands.Sort((x, y) => string.Compare(x.CommandName, y.CommandName, StringComparison.Ordinal));
        StringBuilder sb = new StringBuilder();
        foreach (IChatCommand command in commands)
        {

            sb.AppendLine($"{command.CommandName} - {command.Description}");
            //Logger.ApplicationConsoleMessage($"{command.CommandName} - {command.Description}");
        }
        //TwitchBot.SendWhisper(user, sb.ToString());
    }

    public string CommandName => "help";
    public string Description => "Shows help for all chat commands";
}