using RoboSpinAPI.Interfaces;

namespace RoboSpinAPI.ConsoleCommands;

public class SendMessageConsoleCommand : IConsoleCommand
{
    public void Execute(IEnumerable<string> args)
    {
        //first value is channel, and all other values are message
        var arguments = args.ToList();
        if(arguments.Count < 2)
        {
            Logger.ApplicationConsoleMessage("Invalid number of arguments. ex: /sendmessage spinnyhat Hello World!");
            return;
        }
        string channel = arguments[0];
        string message = string.Join(" ", arguments.Skip(1));
        //TODO: send message
        //TwitchBot.SendMessage(channel, message);
    }

    public string CommandName => "/sendmessage";
    public string Description => "Sends a message to the chat. ex: /sendmessage spinnyhat Hello World!";
}