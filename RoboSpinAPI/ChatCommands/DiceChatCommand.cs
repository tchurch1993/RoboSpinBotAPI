using RoboSpinAPI.Interfaces;
using RoboSpinAPI.Models;

namespace RoboSpinAPI.ChatCommands;

public class DiceChatCommand : IChatCommand
{
    public void Execute(IEnumerable<string> args, string channel, string user)
    {
        //rolls a dice between 1 and 6
        //SendMessage(channel,$"rolled a {new Random().Next(1, 7)}");
    }

    public string CommandName => "dice";
    public string Description => "Rolls a dice between 1 and 6";
}