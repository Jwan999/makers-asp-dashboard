using Makers.Database.Entities;
using Makers.Security;
using Makers.Utilities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Makers.Controllers;

public partial class DashboardController
{
    [HttpPost]
    public IActionResult GetStoreSettings([FromBody] JObject reqBody)
    {
        var PageNumber = reqBody.GetParameter<int>("PageNumber");
        var PageSize = reqBody.GetParameter<int>("PageSize");
        var Filter = reqBody.Value<string>("Filter");

        var data = from e in db.T_STORE_SETTINGS
                   orderby e.ID descending
                   select e;

        if (!string.IsNullOrEmpty(Filter))
        {
            data = data.Where(e => e.PARAMETER_NAME.Contains(Filter)).OrderByDescending(e => e.ID);
        }

        var dataCount = data.Count();

        var resultData = SecurityHelper.Paging(PageSize, dataCount, data.Skip((PageNumber - 1) * PageSize).Take(PageSize));

        return this.Response(null, resultData);
    }

    [HttpPost]
    public IActionResult GetStoreSetting([FromBody] JObject reqBody)
    {
        var Id = reqBody.GetParameter<int>("Id");

        var data = from e in db.T_STORE_SETTINGS
                   where e.ID == Id
                   select e;

        return this.Response(null, data);
    }

    [HttpPost]
    public async Task<IActionResult> AddStoreSetting([FromBody] JObject reqBody)
    {
        var ParameterName = reqBody.GetParameter<string>("ParameterName");
        var ParameterValue = reqBody.GetParameter<string>("ParameterValue");
        var ParameterDescription = reqBody.GetParameter<string>("ParameterDescription");

        T_STORE_SETTINGS newStoreSetting = new()
        {
            ID = null,
            INSDATE = DateTime.Now,
            LUPDATE = DateTime.Now,
            PARAMETER_NAME = ParameterName,
            PARAMETER_VALUE = ParameterValue,
            PARAMETER_DESCRIPTION = ParameterDescription,
        };

        await db.T_STORE_SETTINGS.AddAsync(newStoreSetting);
        await db.SaveChangesAsync();

        await db.AuditAsync(jwt, Constants.AuditActionInsert, newStoreSetting, $"Store Setting ParameterName: {ParameterName}", true);

        return this.Response("Store Setting added successfully", null);
    }

    [HttpPost]
    public async Task<IActionResult> EditStoreSetting([FromBody] JObject reqBody)
    {
        var EditEntityId = reqBody.GetParameter<int>("EditEntityId");
        var ParameterName = reqBody.GetParameter<string>("ParameterName");
        var ParameterValue = reqBody.GetParameter<string>("ParameterValue");
        var ParameterDescription = reqBody.GetParameter<string>("ParameterDescription");

        var StoreSetting = db.T_STORE_SETTINGS.First(e => e.ID == EditEntityId);

        StoreSetting.PARAMETER_NAME = ParameterName;
        StoreSetting.PARAMETER_VALUE = ParameterValue;
        StoreSetting.PARAMETER_DESCRIPTION = ParameterDescription;
        StoreSetting.LUPDATE = DateTime.Now;

        db.T_STORE_SETTINGS.Update(StoreSetting);

        await db.AuditAsync(jwt, Constants.AuditActionUpdate, StoreSetting, $"Store Setting ParameterName: {ParameterName}");

        await db.SaveChangesAsync();

        return this.Response("Store Setting updated successfully", null);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteStoreSetting([FromBody] JObject reqBody)
    {
        var Id = reqBody.GetParameter<int>("Id");

        var StoreSetting = db.T_STORE_SETTINGS.First(e => e.ID == Id);

        db.T_STORE_SETTINGS.Remove(StoreSetting);

        await db.AuditAsync(jwt, Constants.AuditActionDelete, StoreSetting, $"Store Setting ParameterName: {StoreSetting.PARAMETER_NAME}");

        await db.SaveChangesAsync();

        return this.Response("Store Setting deactivated successfully", null);
    }
}

