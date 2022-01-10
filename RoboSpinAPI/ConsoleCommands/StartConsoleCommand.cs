using RoboSpinAPI.Interfaces;
using RoboSpinAPI.Managers;
using RoboSpinAPI.Twitter;

namespace RoboSpinAPI.ConsoleCommands;

public class StartConsoleCommand : IConsoleCommand
{
    public string CommandName => "/start";

    public string Description => "Sends out stream notifications to both Twitter and Discord";
    
    public void Execute(IEnumerable<string> args)
    {
        Logger.ApplicationConsoleMessage("Starting...");
        const string url =
            "https://discord.com/api/webhooks/924853226375884850/q0wPwD34fhmEMHH2Y0XZclB1FSqtEIDZbvN1F8PNOrtrz6wzEpwteMVJCpmhxTQ1Wh8c?wait=true";
        TwitterBotOptions twitterBotOptions = new()
        {
            ConsumerKey = RoboSpinConfigurationManager.GetConfigurationValue("ApiKey"),
            ConsumerSecret = RoboSpinConfigurationManager.GetConfigurationValue("ApiSecret"),
            AccessToken = RoboSpinConfigurationManager.GetConfigurationValue("AccessToken"),
            AccessSecret = RoboSpinConfigurationManager.GetConfigurationValue("AccessSecret")
        };
            
        TwitterBot twitterBot = new(twitterBotOptions);
        
        Logger.ApplicationConsoleMessage("Sending Twitter notification...");
        twitterBot.SendTweet("SpinnyHat is now online! https://www.twitch.tv/SpinnyHat");
        
        //send post request to discord webhook
        Logger.ApplicationConsoleMessage("Sending Discord notification...");
        using (HttpClient client = new())
        {
            HttpRequestMessage request = new(HttpMethod.Post, url);
            
            //create key value pair with content and spinnyhat is online
            KeyValuePair<string,string> keyValuePair = new("content", "SpinnyHat is now online! https://www.twitch.tv/SpinnyHat");
            List <KeyValuePair<string,string>> list = new() {keyValuePair};

            request.Content = new FormUrlEncodedContent(list);
            HttpResponseMessage response = client.Send(request);

            if (response.IsSuccessStatusCode)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Discord Message Sent");
                Console.ResetColor();   
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Discord Message Failed");
                Console.ResetColor();
            }
        }
        
        
        Logger.ApplicationConsoleMessage("Done!");


    }

}

   
