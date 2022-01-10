using System.Collections.Concurrent;

namespace RoboSpinAPI.Models;

public class GlobalBots
{
    private static readonly Lazy<ConcurrentDictionary<string, TwitchBot>> InfoBuilder
            = new( () => new ConcurrentDictionary<string, TwitchBot>() );
        public static ConcurrentDictionary<string, TwitchBot> Info => InfoBuilder.Value;
}