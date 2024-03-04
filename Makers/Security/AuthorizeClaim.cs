using Makers.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Security.Claims;

namespace Makers.Security;

public class AuthorizeClaim : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        #region Prepration and Logging
        string USER_NAME = string.Empty;

        if (context.HttpContext.User.Claims.Any())
            USER_NAME = context.HttpContext.User.Claims.Where(c => c.Type == "11711510111411097109101").Select(c => c.Value).FirstOrDefault();

        var env = context.HttpContext.RequestServices.GetService(typeof(IWebHostEnvironment)) as IWebHostEnvironment;

        var path = context.HttpContext.Request.Path.ToString().ToLower();

        var logger = context.HttpContext.RequestServices.GetService(typeof(ILogger<AuthorizeClaim>)) as ILogger<AuthorizeClaim>;

        var cache = context.HttpContext.RequestServices.GetService(typeof(IMemoryCache)) as IMemoryCache;

        context.ActionArguments.TryGetValue("reqBody", out object reqBody);

        logger.LogMsg($"[{context.HttpContext.Request.Host.Value}] [{USER_NAME}] [{path}] [{JsonConvert.SerializeObject(SecurityHelper.RemoveSensetiveReqData(reqBody))}]");
        #endregion

        #region Authorization
        if (env.EnvironmentName != Environments.Development ||
            !context.ActionDescriptor.EndpointMetadata.OfType<UnderDevelopment>().Any())
        {

            var routeClaims = cache.Get<List<Tuple<string, string, string>>>(Constants.CacheKeyRouteClaims);

            if (routeClaims is null
                || routeClaims.Count == 0
                || routeClaims.All(x => x.Item1 != path)
                || routeClaims.Any(x => x.Item1 == path && x.Item3 == "Y" && x.Item2 is null))
            {
                context.Result = new StatusCodeResult(401);
                return;
            }

            var toAuthClaims = (from rc in routeClaims
                                where rc.Item1 == path && rc.Item2 is not null
                                select rc.Item2).ToArray();

            if (!context.HttpContext.User.IsAuthorized(toAuthClaims))
            {
                context.Result = new StatusCodeResult(401);
                return;
            }
        }
        #endregion

        #region Current and Refreshed JWT
        if (!string.IsNullOrEmpty(USER_NAME))
        {
            var jwt = context.HttpContext.RequestServices.GetService(typeof(IJwt)) as IJwt;

            jwt.UserId = Convert.ToInt32(context.HttpContext.User.Claims.Where(c => c.Type == "117115101114105100").Select(c => c.Value).First());
            jwt.USER_NAME = USER_NAME;
            jwt.RoleId = Convert.ToInt32(context.HttpContext.User.Claims.Where(c => c.Type == "114111108101105100").Select(c => c.Value).First());
            jwt.LastLoginDate = context.HttpContext.User.Claims.Where(c => c.Type == "10897115116108111103105110").Select(c => c.Value).First();

            var isUsingDefaultPassword = (string)context.HttpContext.Request.Headers["DFP"]; // Either null or "true". GetParameter is not required.
            var isPasswordExpired = (string)context.HttpContext.Request.Headers["PWEXP"]; // Either null or "true". GetParameter is not required.

            List<Claim> refreshedTokenClaims = new()
            {
                new Claim("117115101114105100", jwt.UserId.ToString()),
                new Claim("11711510111411097109101", jwt.USER_NAME),
                new Claim("114111108101105100", jwt.RoleId.ToString()),
                new Claim("10897115116108111103105110", jwt.LastLoginDate),
                new Claim("9910897105109115", context.HttpContext.User.Claims.Where(c => c.Type == "9910897105109115").Select(c => c.Value).First())
            };

            if (!string.IsNullOrWhiteSpace(isUsingDefaultPassword))
            {
                refreshedTokenClaims.Add(new Claim("100102112", isUsingDefaultPassword));
            }

            if (!string.IsNullOrWhiteSpace(isPasswordExpired))
            {
                refreshedTokenClaims.Add(new Claim("8087698880738269", isPasswordExpired));
            }

            var refreshedToken = JwtHelper.GetJwt(cache.Get<String>(Constants.CacheKeyUsersTsk),
                Constants.JwtAudienceWebApplication,
                DateTime.Now.AddMinutes(cache.Get<int>(Constants.CacheKeyUsersJwtExpiration)),
                refreshedTokenClaims);

            context.HttpContext.Response.Headers.Add("refreshedtoken", refreshedToken);
        }
        #endregion
    }
}

public class UnderDevelopment : ActionFilterAttribute
{

}