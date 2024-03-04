using Makers.Security;
using Makers.Utilities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Makers.Controllers;

public partial class DashboardController
{
    [HttpPost]
    public IActionResult GetClaims([FromBody] JObject reqBody)
    {
        var PageNumber = reqBody.GetParameter<int>("PageNumber");
        var PageSize = reqBody.GetParameter<int>("PageSize");
        var Filter = reqBody.Value<string>("Filter");

        var data = from e in db.T_CLAIMS
                   orderby e.ID descending
                   select e;

        if (!string.IsNullOrEmpty(Filter))
        {
            data = data.Where(e => e.CLAIM_NAME.Contains(Filter)).OrderByDescending(e => e.ID);
        }

        var dataCount = data.Count();

        var resultData = SecurityHelper.Paging(PageSize, dataCount, data.Skip((PageNumber - 1) * PageSize).Take(PageSize));

        return this.Response(null, resultData);
    }
}