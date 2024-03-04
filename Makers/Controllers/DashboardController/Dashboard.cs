using Makers.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Data;

namespace Makers.Controllers;

public partial class DashboardController
{
    [HttpPost]
    [AllowAnonymous]

    public async Task<IActionResult> GetDashboardData([FromBody] JObject reqBody)
    {
        try
        {

            var Project = reqBody.Value<string>("Project") == null ? "All" : reqBody.Value<string>("Project");


            using var transaction = await db.Database.BeginTransactionAsync();

            var proj = new SqlParameter("@proj", Project);
            var outputParameter = new SqlParameter("@OutputData", SqlDbType.VarChar)
            {
                Direction = ParameterDirection.Output,
                Size = -1
            };

            await db.Database.ExecuteSqlRawAsync($"EXECUTE GET_DASHBOARD_DATA @proj, @OutputData OUTPUT", proj, outputParameter);

            var data1 = outputParameter.Value?.ToString();

            var data = JArray.Parse(data1).ToArray();

            logger.LogMsg($"Data returned successfully, returning OK response...");

            return this.Response("", data);
        }

        catch (Exception ex)
        {
            logger.LogMsg($"Failed to get dashboard data , error : {ex}");
            return this.Response($"Failed to get dashboard data, the error was {ex.Message}", null);
        }
    }
}