using System.Diagnostics;
using RoboSpinAPI.Interfaces;

namespace RoboSpinAPI.ChatCommands;

public class SoundChatCommand : IChatCommand
{
    public void Execute(IEnumerable<string> args, string channel, string user)
    {
        Process soundClip = Process.Start(@"powershell", $@"-c (New-Object Media.SoundPlayer 'http://soundbible.com/grab.php?id=1949&type=wav').PlaySync();");
        //TODO: Add sound clip to the list of sounds
        // TwitchBot.SoundClips.Add(soundClip!.Id, soundClip);
        // soundClip.WaitForExit();
        // TwitchBot.SoundClips.Remove(soundClip.Id);
    }

    public string CommandName => "sound";
    public string Description => "Play a sound";
}