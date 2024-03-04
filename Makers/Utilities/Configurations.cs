using Makers.Database.Contexts;
using Microsoft.Extensions.Caching.Memory;

namespace Makers.Utilities;

public class Configurations
{
    public string LServicesApiTsk { get; set; }

    public static void Reinit(Db db, IMemoryCache cache, string usersTsk)
    {
        var data = (from r in db.T_ROUTE
            join rc in db.T_MAP_ROUTE_CLAIM on r.ID equals rc.ROUTE_ID into joinResult
            from jr in joinResult.DefaultIfEmpty()
            join c in db.T_CLAIMS on jr.CLAIM_ID equals c.ID into joinResult2
            from jr2 in joinResult2.DefaultIfEmpty()
            select new Tuple<string, string, string>(r.ROUTE_NAME.ToLower(), jr2.CLAIM_VALUE, r.HAS_CLAIMS)).ToList();

        var jwtExpiration = int.Parse(db.T_SYS_PARAMS.First(e => e.PARAMETER_NAME == Constants.CacheKeyUsersJwtExpiration).PARAMETER_VALUE);

        cache.Set(Constants.CacheKeyRouteClaims, data);
        cache.Set(Constants.CacheKeyUsersJwtExpiration, jwtExpiration);

        if (!string.IsNullOrEmpty(usersTsk))
        {
            cache.Set(Constants.CacheKeyUsersTsk, usersTsk);
        }
    }
}