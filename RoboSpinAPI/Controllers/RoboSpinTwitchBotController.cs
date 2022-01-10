using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoboSpinAPI.Models;
using RoboSpinAPI.Services;

namespace RoboSpinAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/bot")]
public class RoboSpinTwitchBotController : ControllerBase
{
    private readonly ILogger<RoboSpinTwitchBotController> _logger;
    private readonly RoboSpinUsersService _roboSpinUsersService;
    
    public RoboSpinTwitchBotController(ILogger<RoboSpinTwitchBotController> logger, RoboSpinUsersService roboSpinUsersService)
    {
        _logger = logger;
        _roboSpinUsersService = roboSpinUsersService;
    }

    //[Route("api/bot/start")]
    [HttpPost("start")]
    // start the bot
    public async Task<IActionResult> StartBot()
    {
        string? userId = User.FindFirstValue(ClaimTypes.Name);
        string? userName = User.FindFirstValue(ClaimTypes.NameIdentifier);

        RoboSpinUser? user = await _roboSpinUsersService.GetAsync(userId);
        if (user == null)
        {
            return Problem("bruh wtf even happend, you are not in the database");
        }
        
        if (GlobalBots.Info.ContainsKey(userId) && GlobalBots.Info[userId].IsRunning)
        {
            return Conflict("bot is already running");
        }

        if (user.BotSettings == new BotSettings() || !user.BotSettings.IsValid())
        {
            return NotFound("Missing Settings to start bot");
        }
        
        TwitchBot bot = new TwitchBot(user.BotSettings, userName);
        
        if(!bot.Start()) return Problem("bot failed to start");

        GlobalBots.Info.TryAdd(userId, bot);
        
        return Ok($"Bot {user.BotSettings.TwitchBotUsername} Started");
    }
    //[Route("api/bot/stop")]
    [HttpPost("stop")]
    // stop the bot
    public async Task<IActionResult> StopBot()
    {
        string? userId = User.FindFirstValue(ClaimTypes.Name);
        RoboSpinUser? user = await _roboSpinUsersService.GetAsync(userId);
        if (user == null)
        {
            return Problem("bruh wtf even happend, you are not in the database");
        }
        
        if (!GlobalBots.Info.ContainsKey(userId))
        {
            //bot is not running
            return Conflict("Bot is not running");
        }

        GlobalBots.Info.TryRemove(userId, out TwitchBot _);
        return Ok($"Bot {user.BotSettings.TwitchBotUsername} Stopped");
    }
    
    //[Route("api/bot/sendMessage")]
    [HttpPost("SendMessage")]
    // send a message from the bot
    public async Task<IActionResult> SendMessage([FromBody] string message)
    {
        string? userId = User.FindFirstValue(ClaimTypes.Name);
        RoboSpinUser? user = await _roboSpinUsersService.GetAsync(userId);
        if (user == null)
        {
            return Problem("bruh wtf even happend, you are not in the database");
        }
        
        if (!GlobalBots.Info.ContainsKey(userId))
        {
            //bot is not running
            return Conflict("Bot is not running");
        }

        GlobalBots.Info[userId].SendMessage(user.Name, message);
        return Ok();
    }
    
    //[Route("api/bot/isRunning")]
    [HttpGet("isBotRunning")]
    // get the status of the bot
    public async Task<IActionResult> IsBotRunning()
    {
        string? userId = User.FindFirstValue(ClaimTypes.Name);
        RoboSpinUser? user = await _roboSpinUsersService.GetAsync(userId);
        if (user == null)
        {
            return Problem("bruh wtf even happend, you are not in the database");
        }
        
        if (!GlobalBots.Info.ContainsKey(userId))
        {
            //bot is not running
            return Ok(false);
        }
        
        TwitchBot bot = GlobalBots.Info[userId];
        return Ok(bot.IsRunning);
    }
    
    

}