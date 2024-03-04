using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using Makers.Database.Contexts;
using Makers.Utilities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Makers.Security;

public class Jwt : IJwt
{
    public int UserId { get; set; }
    public string USER_NAME { get; set; }
    public int RoleId { get; set; }
    public string LastLoginDate { get; set; }
}

public static class JwtHelper
{
    private static readonly string[] AuditClaims = new string[] { "117115101114105100", "11711510111411097109101", "114111108101105100" };

    public static string GetJwt(string tsk, string audience, DateTime expiry, List<Claim> userClaims)
    {
        return new JwtSecurityTokenHandler().WriteToken(
            new JwtSecurityToken(
                "Makers",
                audience,
                userClaims,
                expires: expiry,
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(tsk)), SecurityAlgorithms.HmacSha256)));
    }

    public static List<Claim> SetClaims(Db db, T_USERS user, IMemoryCache cache)
    {
        var claims = string.Empty;

        var role = db.T_ROLES.First(e => e.ID == user.ROLE_ID_FK);

        if (role.ID == Constants.GlobalAdminId && role.ROLE_NAME == Constants.GlobalAdmin)
        {
            claims = string.Join(',', db.T_CLAIMS.Select(e => e.CLAIM_VALUE));
        }

        else if (role.ID == Constants.WebsiteUsersId)
        {
            claims = string.Join(',', db.T_MAP_ROLES_CLAIMS.Where(e => e.ROLE_ID == user.ROLE_ID_FK).Select(e => e.CLAIM_VALUE));
        }

        List<Claim> tokenClaims = new()
        {
            new Claim("117115101114105100", user.ID.ToString()),
            new Claim("11711510111411097109101", user.USERNAME),
            new Claim("114111108101105100", user.ROLE_ID_FK.ToString()),
            new Claim("10897115116108111103105110", user.LAST_LOGIN_DATE is null ? "" : user.LAST_LOGIN_DATE.Value.ToString("ddd, dd MMM yyyy hh:mm tt")),
            new Claim("9910897105109115", claims)
        };

        //if (user.IS_DEFAULT_PASS == "Y")
        //{
        //    tokenClaims.Add(new Claim("100102112", "true"));
        //}

        //if (DateTime.Now > user.PW_EXP_ON)
        //{
        //    tokenClaims.Add(new Claim("8087698880738269", "true"));
        //}

        return tokenClaims;
    }

    public static bool IsAuthorized(this ClaimsPrincipal cp, string[] toAuthClaims)
    {
        if (cp.Claims.Count() > 0)
        {
            var userClaims = cp.Claims.Where(c => c.Type == "9910897105109115").Select(c => c.Value).First().Split(',');

            foreach (var claim in toAuthClaims)
            {
                if (!userClaims.Contains(claim))
                {
                    return false;
                }
            }
        }

        return true;
    }

    public static string GetAuditMsg(this ClaimsPrincipal cp)
    {
        return $"[{string.Join(" - ", cp.Claims.Where(c => AuditClaims.Contains(c.Type)).Select(c => c.Value))}]";
    }
}