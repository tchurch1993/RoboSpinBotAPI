using System.Net.Http.Headers;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoboSpinAPI.Models;
using RoboSpinAPI.Services;
using TwitchLib.Api;
using TwitchLib.Api.Auth;

namespace RoboSpinAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/user")]
public class RoboSpinTwitchController : ControllerBase
{

    private readonly ILogger<RoboSpinTwitchController> _logger;
    private readonly RoboSpinUsersService _roboSpinUsersService;
    
    public RoboSpinTwitchController(ILogger<RoboSpinTwitchController> logger, RoboSpinUsersService roboSpinUsersService)
    {
        _logger = logger;
        _roboSpinUsersService = roboSpinUsersService;
    }


    [HttpGet(Name = "GetOrCreateRoboSpinUser")]
    public async Task<ActionResult<RoboSpinUser>> Get()
    {
        Console.WriteLine(Request.Headers["Authorization"]);;
        string? userId = User.FindFirstValue(ClaimTypes.Name);
        string? userName = User.FindFirstValue(ClaimTypes.NameIdentifier);


        RoboSpinUser? roboSpinUser = await _roboSpinUsersService.GetAsync(userId);

        if (roboSpinUser?.Id != null) return Ok(roboSpinUser);

        RoboSpinUser newRoboSpinUser = new RoboSpinUser
        {
            Id = userId,
            Name = userName
        };

        await _roboSpinUsersService.CreateAsync(newRoboSpinUser);
        return Created("",newRoboSpinUser);

    }
    
    [HttpPut("BotSettings")]
    public async Task<ActionResult<RoboSpinUser>> Put(BotSettings settings)
    {
        //set bot settings
        string? userId = User.FindFirstValue(ClaimTypes.Name);
        RoboSpinUser? roboSpinUser = await _roboSpinUsersService.GetAsync(userId);
        if (roboSpinUser == null) return NotFound();
        roboSpinUser.BotSettings = settings;
        await _roboSpinUsersService.UpdateAsync(roboSpinUser.Id, roboSpinUser);
        return Ok(roboSpinUser);
        
        
    }
    
}