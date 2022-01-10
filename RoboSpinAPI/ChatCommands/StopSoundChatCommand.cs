using RoboSpinAPI.Interfaces;

namespace RoboSpinAPI.ChatCommands;

public class StopSoundChatCommand : IChatCommand
{
    public void Execute(IEnumerable<string> args, string channel, string user)
    {
        Logger.ApplicationConsoleMessage("stopping all sounds...");
        //TODO: get all sounds and stop them somehow
        // foreach (var soundClip in TwitchBot.SoundClips)
        // {
        //     soundClip.Value.Kill();
        // }
    }

    public string CommandName => "stop";
    public string Description => "Stops all sounds (for when things get out of control)";
}