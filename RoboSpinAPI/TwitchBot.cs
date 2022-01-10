using System.Collections;
using System.Diagnostics;
using System.Text;
using RoboSpinAPI.Managers;
using RoboSpinAPI.Models;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;

namespace RoboSpinAPI
{
    public class TwitchBot
    {
        private readonly string? _nick;
        private readonly string? _password;
        private readonly char _prefix;
        private readonly string _channel;
        private readonly Dictionary<int, Process> _soundClips;
        private TwitchClient _twitchClient;

        public TwitchBot(BotSettings settings, string channelName)
        {
            this._nick = settings.TwitchBotUsername;
            this._password = settings.TwitchBotPassword;
            this._channel = channelName;
            this._prefix = string.IsNullOrEmpty(settings.TwitchBotPrefix) ? '!' : settings.TwitchBotPrefix[0];
            this._soundClips = new Dictionary<int, Process>();
            
            _twitchClient = new TwitchClient();
        }

        public bool Start()
        {
            ConnectionCredentials credentials = new ConnectionCredentials(_nick, _password);
            _twitchClient.Initialize(credentials, _channel, _prefix, _prefix, true);
            
            _twitchClient.OnChatCommandReceived += TwitchClient_OnChatCommandReceived;
            _twitchClient.OnLog += TwitchClient_OnLog;
            _twitchClient.OnJoinedChannel += TwitchClient_OnJoinedChannel;
            _twitchClient.OnMessageReceived += TwitchClient_OnMessageReceived;
            _twitchClient.OnWhisperReceived += TwitchClient_OnWhisperReceived;
            _twitchClient.OnNewSubscriber += TwitchClient_OnNewSubscriber;
            _twitchClient.OnConnected += TwitchClient_OnConnected;
            return _twitchClient.Connect();

        }
        
        public async Task Stop()
        {
            _twitchClient.Disconnect();
        }
        
        //change Logger color to actually represent what is logging/communicating
        private static void TwitchClient_OnLog(object? sender, OnLogArgs e)
        {
            Logger.DefaultConsoleMessage($"TwitchClient_OnLog - {e.DateTime.ToLocalTime()}: {e.BotUsername} - {e.Data}");
        }
        private static void TwitchClient_OnJoinedChannel(object? sender, OnJoinedChannelArgs e)
        {
            Logger.ApplicationConsoleMessage($"TwitchClient_OnJoinedChannel - Joined channel {e.Channel}");
        }
        private static void TwitchClient_OnMessageReceived(object? sender, OnMessageReceivedArgs e)
        {
            Logger.ServerConsoleMessage($"TwitchClient_OnMessageReceived - {e.ChatMessage.Username}: {e.ChatMessage.Message}");
        }
        private static void TwitchClient_OnWhisperReceived(object? sender, OnWhisperReceivedArgs e)
        {
            Logger.ServerConsoleMessage($"(Whisper){e.WhisperMessage.Username}: {e.WhisperMessage.Message}");
        }
        private static void TwitchClient_OnNewSubscriber(object? sender, OnNewSubscriberArgs e)
        {
            Logger.ServerConsoleMessage($"{e.Subscriber.DisplayName} just subscribed!");
        }
        private static void TwitchClient_OnConnected(object? sender, OnConnectedArgs e)
        {
            //send message every 2 minutes
            Logger.ApplicationConsoleMessage($"Connected to Twitch IRC as {e.BotUsername}");
        }
        

        //I should probably make an interface for chat commands like I did console commands. TODO: Make an interface for chat commands
        private static void TwitchClient_OnChatCommandReceived(object? sender, OnChatCommandReceivedArgs e)
        {
            if (ChatCommandManager.HasCommand(e.Command.CommandText))
            {
                ChatCommandManager.ExecuteCommand(e.Command.CommandText, e.Command.ChatMessage.Channel, e.Command.ChatMessage.Username, e.Command.ArgumentsAsList);
            }
        }
        
        
        
        public void SendMessage(string channel, string? message)
        {
            _twitchClient.SendMessage(channel, message);
        }
        
        public void SendWhisper(string user, string message)
        {
            _twitchClient.SendWhisper(user, message);
        }
        
        //TODO: find better way to do this. may move this to a different class idk.
        public Dictionary<int, Process> SoundClips => _soundClips;
        
        public bool IsRunning => _twitchClient.IsConnected;


        private static async Task<Task> ConsoleInput(ITwitchClient twClient)
        {
            try
            {
                while (true)
                {
                    string message = await GetConsoleCommand();
                    if (string.IsNullOrEmpty(message)) continue;
                    string command = message.Split(" ")[0];
                    string[] args = message.Split(" ").Skip(1).ToArray();


                    if (ConsoleCommandManager.HasCommand(command))
                    {
                        // The empty array is temporary, it will be replaced with the command arguments after the command is split
                        ConsoleCommandManager.ExecuteCommand(command, args);
                        continue;
                    }

                    twClient.SendRaw(command);
                }
            }
            catch (Exception e)
            {
                Logger.ApplicationConsoleMessage("Error in ConsoleInput: " + e.Message);
                Console.ReadKey(true);
                //ConsoleInput(twClient);
            }

            return Task.CompletedTask;
        }

        //this is for autocompleting console commands.  It works but its kind of jank. might just remove it.
        private static Task<string> GetConsoleCommand()
        {
            StringBuilder sb = new StringBuilder();
            var commandList = ConsoleCommandManager.GetCommands().Select(x => x.CommandName).ToList();
            
            while (true)
            {
                ConsoleKeyInfo result = Console.ReadKey(true);
                switch (result.Key)
                {
                    case ConsoleKey.Enter:
                        string message = sb.ToString();
                        sb.Clear();
                        Console.WriteLine();
                        return Task.FromResult(message);
                    case ConsoleKey.Tab:
                        //auto complete command from list of Console Commands and current input
                        string commandName = commandList.FirstOrDefault(x => x.StartsWith(sb.ToString()));
                        
                        if(string.IsNullOrEmpty(commandName)) continue;

                        if (commandList.Any(x => x.Equals(sb.ToString())))
                        {
                            int indexOf = commandList.IndexOf(sb.ToString());
                            commandName = indexOf > commandList.Count - 2 ? commandList[0] : commandList[indexOf + 1];
                        }
                        
                        ClearCurrentLine();
                        Console.Write(commandName);;
                        sb.Clear();
                        sb.Append(commandName);

                        break;

                    case ConsoleKey.Backspace:
                        if (sb.Length > 0)
                        {
                            ClearCurrentLine();
                            sb.Remove(sb.Length - 1, 1);
                            Console.Write(sb.ToString());
                        }
                        break;
                    case ConsoleKey.Clear:
                    case ConsoleKey.Pause:
                    case ConsoleKey.Escape:
                    case ConsoleKey.PageUp:
                    case ConsoleKey.PageDown:
                    case ConsoleKey.End:
                    case ConsoleKey.Home:
                    case ConsoleKey.LeftArrow:
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.RightArrow:
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.Select:
                    case ConsoleKey.Print:
                    case ConsoleKey.Execute:
                    case ConsoleKey.PrintScreen:
                    case ConsoleKey.Insert:
                    case ConsoleKey.Delete:
                    case ConsoleKey.Help:
                    case ConsoleKey.LeftWindows:
                    case ConsoleKey.RightWindows:
                    case ConsoleKey.Applications:
                    case ConsoleKey.Sleep:
                    case ConsoleKey.Multiply:
                    case ConsoleKey.Add:
                    case ConsoleKey.Separator:
                    case ConsoleKey.Subtract:
                    case ConsoleKey.Decimal:
                    case ConsoleKey.Divide:
                    case ConsoleKey.F1:
                    case ConsoleKey.F2:
                    case ConsoleKey.F3:
                    case ConsoleKey.F4:
                    case ConsoleKey.F5:
                    case ConsoleKey.F6:
                    case ConsoleKey.F7:
                    case ConsoleKey.F8:
                    case ConsoleKey.F9:
                    case ConsoleKey.F10:
                    case ConsoleKey.F11:
                    case ConsoleKey.F12:
                    case ConsoleKey.F13:
                    case ConsoleKey.F14:
                    case ConsoleKey.F15:
                    case ConsoleKey.F16:
                    case ConsoleKey.F17:
                    case ConsoleKey.F18:
                    case ConsoleKey.F19:
                    case ConsoleKey.F20:
                    case ConsoleKey.F21:
                    case ConsoleKey.F22:
                    case ConsoleKey.F23:
                    case ConsoleKey.F24:
                    case ConsoleKey.BrowserBack:
                    case ConsoleKey.BrowserForward:
                    case ConsoleKey.BrowserRefresh:
                    case ConsoleKey.BrowserStop:
                    case ConsoleKey.BrowserSearch:
                    case ConsoleKey.BrowserFavorites:
                    case ConsoleKey.BrowserHome:
                    case ConsoleKey.VolumeMute:
                    case ConsoleKey.VolumeDown:
                    case ConsoleKey.VolumeUp:
                    case ConsoleKey.MediaNext:
                    case ConsoleKey.MediaPrevious:
                    case ConsoleKey.MediaStop:
                    case ConsoleKey.MediaPlay:
                    case ConsoleKey.LaunchMail:
                    case ConsoleKey.LaunchMediaSelect:
                    case ConsoleKey.LaunchApp1:
                    case ConsoleKey.LaunchApp2:
                    case ConsoleKey.Oem1:
                    case ConsoleKey.OemPlus:
                    case ConsoleKey.OemComma:
                    case ConsoleKey.OemMinus:
                    case ConsoleKey.OemPeriod:
                    case ConsoleKey.Oem2:
                    case ConsoleKey.Oem3:
                    case ConsoleKey.Oem4:
                    case ConsoleKey.Oem5:
                    case ConsoleKey.Oem6:
                    case ConsoleKey.Oem7:
                    case ConsoleKey.Oem8:
                    case ConsoleKey.Oem102:
                    case ConsoleKey.Process:
                    case ConsoleKey.Packet:
                    case ConsoleKey.Attention:
                    case ConsoleKey.CrSel:
                    case ConsoleKey.ExSel:
                    case ConsoleKey.EraseEndOfFile:
                    case ConsoleKey.Play:
                    case ConsoleKey.Zoom:
                    case ConsoleKey.NoName:
                    case ConsoleKey.Pa1:
                    case ConsoleKey.OemClear:
                    default:
                        sb.Append(result.KeyChar);
                        Console.Write(result.KeyChar);
                        break;
                }
            }
        }

        private static void ClearCurrentLine()
        {
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, Console.CursorTop);
        }
    }
}
