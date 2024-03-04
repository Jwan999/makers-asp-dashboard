using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.IdentityModel.Tokens.Jwt;

namespace Makers.Utilities;

[Authorize(AuthenticationSchemes = Constants.AuthSchemeUsers)]
public class WebSocketHub : Hub
{
    private readonly ILogger logger;

    public const string Route = "/websockethub";

    public WebSocketHub(ILogger<WebSocketHub> logger)
    {
        this.logger = logger;
    }

    public override Task OnConnectedAsync()
    {
        var token = Context.GetHttpContext().Request.Query.Where(p => p.Key == "access_token").FirstOrDefault().Value;

        var USER_NAME = new JwtSecurityToken(token).Claims.First(c => c.Type == "11711510111411097109101").Value;

        logger.LogMsg($"Websocket connection established with user [{USER_NAME}]");

        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception ex)
    {
        var token = Context.GetHttpContext().Request.Query.Where(p => p.Key == "access_token").FirstOrDefault().Value;

        var USER_NAME = new JwtSecurityToken(token).Claims.First(c => c.Type == "11711510111411097109101").Value;

        logger.LogMsg($"Websocket connection terminated with user [{USER_NAME}]");

        if (ex is not null)
        {
            logger.LogMsg($"Disconnected abnormally, exception: {ex}");
        }

        return base.OnDisconnectedAsync(ex);
    }
}