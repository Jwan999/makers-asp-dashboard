using Makers.Database.Entities;
using Makers.Security;
using Makers.Utilities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Data;

namespace Makers.Controllers;

public partial class DashboardController
{
    [HttpPost]
    public IActionResult GetSystemParameters([FromBody] JObject reqBody)
    {
        //var PageNumber = reqBody.GetParameter<int>("PageNumber");
        //var PageSize = reqBody.GetParameter<int>("PageSize");
        //var Filter = reqBody.Value<string>("Filter");

        var data = from e in db.T_SYS_PARAMS
                   orderby e.ID descending
                   select e;

        //if (!string.IsNullOrEmpty(Filter))
        //{
        //    data = data.Where(e => e.PARAMETER_NAME.Contains(Filter)).OrderByDescending(e => e.ID);
        //}

        //var dataCount = data.Count();

        //var resultData = SecurityHelper.Paging(PageSize, dataCount, data.Skip((PageNumber - 1) * PageSize).Take(PageSize));

        return this.Response(null, data);
    }

    [HttpPost]
    public IActionResult GetSystemParameter([FromBody] JObject reqBody)
    {
        var Id = reqBody.GetParameter<int>("Id");

        var data = from e in db.T_SYS_PARAMS
                   where e.ID == Id
                   select e;

        return this.Response(null, data);
    }

    [HttpPost]
    public async Task<IActionResult> AddSystemParameter([FromBody] JObject reqBody)
    {
        var ParameterName = reqBody.GetParameter<string>("PARAMETER_NAME");
        var ParameterValue = reqBody.GetParameter<string>("PARAMETER_VALUE");
        var ParameterDescription = reqBody.GetParameter<string>("PARAMETER_DESCRIPTION");

        T_SYS_PARAMS newSystemParameter = new()
        {
            ID = null,
            INSDATE = DateTime.Now,
            LUPDATE = DateTime.Now,
            PARAMETER_NAME = ParameterName,
            PARAMETER_VALUE = ParameterValue,
            PARAMETER_DESCRIPTION = ParameterDescription,
        };

        await db.T_SYS_PARAMS.AddAsync(newSystemParameter);
        await db.SaveChangesAsync();

        await db.AuditAsync(jwt, Constants.AuditActionInsert, newSystemParameter, $"System Parameter ParameterName: {ParameterName}", true);

        return this.Response("System Parameter added successfully", null);
    }

    [HttpPost]
    public async Task<IActionResult> EditSystemParameter([FromBody] JObject reqBody)
    {
        var EditEntityId = reqBody.GetParameter<int>("EditEntityId");
        var ParameterName = reqBody.GetParameter<string>("PARAMETER_NAME");
        var ParameterValue = reqBody.GetParameter<string>("PARAMETER_VALUE");
        var ParameterDescription = reqBody.GetParameter<string>("PARAMETER_DESCRIPTION");

        var SystemParameter = db.T_SYS_PARAMS.First(e => e.ID == EditEntityId);

        SystemParameter.PARAMETER_NAME = ParameterName;
        SystemParameter.PARAMETER_VALUE = ParameterValue;
        SystemParameter.PARAMETER_DESCRIPTION = ParameterDescription;
        SystemParameter.LUPDATE = DateTime.Now;

        db.T_SYS_PARAMS.Update(SystemParameter);

        await db.AuditAsync(jwt, Constants.AuditActionUpdate, SystemParameter, $"System Parameter ParameterName: {ParameterName}");

        await db.SaveChangesAsync();

        return this.Response("System Parameter updated successfully", null);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteSystemParameter([FromBody] JObject reqBody)
    {
        var Id = reqBody.GetParameter<int>("Id");

        var SystemParameter = db.T_SYS_PARAMS.First(e => e.ID == Id);

        db.T_SYS_PARAMS.Remove(SystemParameter);

        await db.AuditAsync(jwt, Constants.AuditActionDelete, SystemParameter, $"System Parameter ParameterName: {SystemParameter.PARAMETER_NAME}");

        await db.SaveChangesAsync();

        return this.Response("System Parameter deleted successfully", null);
    }

    [HttpPost]
    public async Task<IActionResult> ReInit()
    {
        Configurations.Reinit(db, cache, "");

        await db.AuditAsync(jwt, Constants.AuditActionExecute, $"ReInit system", true);

        return this.Response("System reinitialized successfully", null);
    }
}

