using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoboSpinAPI.Managers;
using RoboSpinAPI.Models;
using RoboSpinAPI.Services;
using RoboSpinAPI.Twitter;

namespace RoboSpinAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/commands")]
public class RoboSpinWebCommands : ControllerBase
{
    private readonly ILogger<RoboSpinWebCommands> _logger;
    private readonly RoboSpinUsersService _roboSpinUsersService;
    
    public RoboSpinWebCommands(ILogger<RoboSpinWebCommands> logger, RoboSpinUsersService roboSpinUsersService)
    {
        _logger = logger;
        _roboSpinUsersService = roboSpinUsersService;
    }

    [HttpPost("SendNotifications")]
    public async Task<IActionResult> SendNotifications([FromBody] string message)
    {
        Logger.ApplicationConsoleMessage("Starting...");
        RoboSpinUser? user = await _roboSpinUsersService.GetAsync(User.FindFirstValue(ClaimTypes.Name));
        
        //TODO: add setting for discord server callback url
        const string url =
            "https://discord.com/api/webhooks/924853226375884850/q0wPwD34fhmEMHH2Y0XZclB1FSqtEIDZbvN1F8PNOrtrz6wzEpwteMVJCpmhxTQ1Wh8c?wait=true";
        TwitterBotOptions twitterBotOptions = new()
        {
            ConsumerKey = user?.BotSettings.ApiKey ?? string.Empty,
            ConsumerSecret = user?.BotSettings.ApiSecret ?? string.Empty,
            AccessToken = user?.BotSettings.AccessToken ?? string.Empty,
            AccessSecret = user?.BotSettings.AccessSecret ?? string.Empty
        };

        TwitterBot twitterBot = new(twitterBotOptions);

        Logger.ApplicationConsoleMessage("Sending Twitter notification...");
        twitterBot.SendTweet($"{message} https://www.twitch.tv/{user?.Name}");

        //send post request to discord webhook
        Logger.ApplicationConsoleMessage("Sending Discord notification...");
        using HttpClient client = new();
        HttpRequestMessage request = new(HttpMethod.Post, url);

        //create key value pair with content and spinnyhat is online
        KeyValuePair<string, string> keyValuePair = new("content", $"{message} https://www.twitch.tv/{user?.Name}");;
        List<KeyValuePair<string, string>> list = new() {keyValuePair};

        request.Content = new FormUrlEncodedContent(list);
        HttpResponseMessage response = await client.SendAsync(request);

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

        return AcceptedAtAction("Notifications Sent");
    }
}