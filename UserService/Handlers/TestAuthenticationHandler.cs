using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace UserService.Handlers;

public class TestAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public TestAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, 
        ILoggerFactory logger, 
        UrlEncoder encoder, 
        ISystemClock clock)
        : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, "a6427685-84e3-4fbf-8716-c94d1053b020"),
            new Claim(JwtRegisteredClaimNames.Email, "test@example.com"),
        };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "Test");

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}