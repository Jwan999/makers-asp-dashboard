using Makers.Database.Entities;
using Makers.Security;
using Makers.Utilities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Makers.Controllers;

public partial class DashboardController
{
    [HttpPost]
    public IActionResult GetFounders([FromBody] JObject reqBody)
    {
        var Id = reqBody.Value<int>("StartupId");

        //var PageNumber = reqBody.GetParameter<int>("PageNumber");
        //var PageSize = reqBody.GetParameter<int>("PageSize");
        //var Filter = reqBody.GetParameter<string>("Filter");

        var data = from e in db.T_FOUNDERS
                   where e.STARTUP_ID == Id
                   orderby e.ID descending
                   select new
                   {
                       e.ID,
                       e.INSDATE,
                       e.LUPDATE,
                       e.NAMEX,
                       e.PHONEX,
                       e.EMAILX,
                       e.GENDER
                   };

        //if (!string.IsNullOrEmpty(Filter))
        //{
        //    data = data.Where(e => e.NAMEX.Contains(Filter)).OrderByDescending(e => e.ID);
        //}

        //var dataCount = data.Count();

        //var resultData = SecurityHelper.Paging(PageSize, dataCount, data.Skip((PageNumber - 1) * PageSize).Take(PageSize));

        return this.Response(null, data);
    }

    [HttpPost]
    public IActionResult GetFounder([FromBody] JObject reqBody)
    {
        var Id = reqBody.GetParameter<int>("Id");

        var data = from e in db.T_FOUNDERS
                   where e.ID == Id
                   select new
                   {
                       e.ID,
                       e.INSDATE,
                       e.LUPDATE,
                       e.EMAILX,
                       e.PHONEX,
                       e.STARTUP_ID,
                       e.GENDER,
                       e.NAMEX
                   };

        return this.Response(null, data);
    }

    [HttpPost]
    public async Task<IActionResult> AddFounder([FromBody] JObject reqBody)
    {
        var StartupId = reqBody.GetParameter<int>("StartupId");
        var Namex = reqBody.GetParameter<string>("NAMEX");
        var Emailx = reqBody.GetParameter<string>("EMAILX");
        var Phonex = reqBody.GetParameter<string>("PHONEX");
        var Gender = reqBody.GetParameter<string>("GENDER");


        T_FOUNDERS newFounder = new()
        {
            ID = null,
            INSDATE = DateTime.Now,
            LUPDATE = DateTime.Now,
            NAMEX = Namex,
            PHONEX = Phonex,
            EMAILX = Emailx,
            GENDER = Gender,
            STARTUP_ID = StartupId
        };

        await db.T_FOUNDERS.AddAsync(newFounder);
        await db.SaveChangesAsync();


        await db.AuditAsync(jwt, Constants.AuditActionInsert, newFounder, $"Founder NAMEX: {newFounder.NAMEX}", true);

        return this.Response("Founder added successfully", null);
    }

    [HttpPost]
    public async Task<IActionResult> EditFounder([FromBody] JObject reqBody)
    {
        var EditEntityId = reqBody.GetParameter<int>("EditEntityId");
        var StartupId = reqBody.GetParameter<int>("StartupId");
        var Namex = reqBody.GetParameter<string>("NAMEX");
        var Emailx = reqBody.GetParameter<string>("EMAILX");
        var Phonex = reqBody.GetParameter<string>("PHONEX");
        var Gender = reqBody.GetParameter<string>("GENDER");

        var founder = db.T_FOUNDERS.First(e => e.ID == EditEntityId);

        founder.NAMEX = Namex;
        founder.EMAILX = Emailx;
        founder.PHONEX = Phonex;
        founder.GENDER = Gender;
        founder.LUPDATE = DateTime.Now;
        founder.STARTUP_ID = StartupId;

        db.T_FOUNDERS.Update(founder);

        await db.AuditAsync(jwt, Constants.AuditActionUpdate, founder, $"Founder NAMEX: {founder.NAMEX}");

        await db.SaveChangesAsync();

        return this.Response("Founder updated successfully", null);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteFounder([FromBody] JObject reqBody)
    {
        var Id = reqBody.GetParameter<int>("Id");

        var founder = db.T_FOUNDERS.First(e => e.ID == Id);

        db.T_FOUNDERS.Remove(founder);

        await db.AuditAsync(jwt, Constants.AuditActionDelete, founder, $"Founder NAMEX: {founder.NAMEX}");

        await db.SaveChangesAsync();

        return this.Response("Founder deleted successfully", null);
    }
}