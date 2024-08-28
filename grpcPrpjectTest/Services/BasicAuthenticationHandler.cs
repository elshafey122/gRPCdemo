using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace grpcPrpjectTest.Services
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public BasicAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (Request.Headers.ContainsKey("Authorization"))
            {
                var headervalue = Request.Headers["Authorization"].ToString();
                if (headervalue.StartsWith("Basic"))
                {
                    var token = headervalue.Split(" ")[1];
                    var bytes = Convert.FromBase64String(token);
                    var plaintext = Encoding.UTF8.GetString(bytes);

                    int seperator = plaintext.IndexOf(':');

                    var username = plaintext.Substring(0, seperator);
                    var password  = plaintext.Substring(seperator + 1);

                    if(username=="device" && password == "p@ssw@rd")
                    {
                        var claimprinciple = new ClaimsPrincipal
                            (
                            new ClaimsIdentity(new List<Claim> {
                                new Claim(ClaimTypes.Name, username),
                                new Claim(ClaimTypes.Role, "Device") })
                            );
                        return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(claimprinciple, Scheme.Name)));
                    }
                }
            }
            return Task.FromResult(AuthenticateResult.Fail("UnAuthorized"));
        }
    }
}
