using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using TwitchLib.Api.Auth;

namespace RoboSpinAPI.Handlers;

public class RoboSpinAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public RoboSpinAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
    {
        
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if(!Request.Headers.ContainsKey("Authorization"))
        {
            return AuthenticateResult.Fail("Missing Authorization Header");
        }
        AuthenticationHeaderValue authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
        if(authHeader.Scheme.ToLower() != "bearer")
        {
            return AuthenticateResult.Fail("Invalid Authorization Header");
        }
        string? token = authHeader.Parameter;
        TwitchLib.Api.TwitchAPI twitchApi = new TwitchLib.Api.TwitchAPI();
        ValidateAccessTokenResponse? result = await twitchApi.Auth.ValidateAccessTokenAsync(token);
        
        if (result == null) return AuthenticateResult.Fail("Invalid Token");
        
        Claim[] claims = new[] {new Claim(ClaimTypes.Name, result.UserId), new Claim(ClaimTypes.NameIdentifier, result.Login)};
        ClaimsIdentity identity = new ClaimsIdentity(claims, Scheme.Name);
        ClaimsPrincipal principal = new ClaimsPrincipal(identity);
        AuthenticationTicket ticket = new AuthenticationTicket(principal, Scheme.Name);
        return AuthenticateResult.Success(ticket);
    }
}