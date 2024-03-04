using Makers.Database.Entities;
using Makers.Security;
using Makers.Utilities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Makers.Controllers;

public partial class DashboardController
{
    [HttpPost]
    public IActionResult GetContests([FromBody] JObject reqBody)
    {
        //var PageNumber = reqBody.GetParameter<int>("PageNumber");
        //var PageSize = reqBody.GetParameter<int>("PageSize");
        //var Filter = reqBody.GetParameter<string>("Filter");

        var data = from e in db.T_CONTESTS
                   orderby e.ID descending
                   select new
                   {
                       e.ID,
                       e.IS_ACTIVE,
                       e.INST,
                       e.NAMEX,
                       e.OVERVIEW,
                       e.START_DATE,
                       e.DURATION,
                       e.TYPEX
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
    public IActionResult GetContest([FromBody] JObject reqBody)
    {
        var Id = reqBody.GetParameter<int>("Id");

        var data = from e in db.T_CONTESTS
                   where e.ID == Id
                   select e;

        return this.Response(null, data);
    }

    [HttpPost]
    public async Task<IActionResult> AddContest([FromBody] JObject reqBody)
    {
        var Inst = reqBody.GetParameter<int>("INST");
        var Namex = reqBody.GetParameter<string>("NAMEX");
        var Overview = reqBody.GetParameter<string>("OVERVIEW");
        var Typex = reqBody.GetParameter<string>("TYPEX");
        var StartDate = reqBody.GetParameter<DateTime>("START_DATE");
        var Duration = reqBody.GetParameter<int>("DURATION");

        T_CONTESTS newContest = new()
        {
            ID = null,
            INSDATE = DateTime.Now,
            LUPDATE = DateTime.Now,
            INST = Inst,
            NAMEX = Namex,
            OVERVIEW = Overview,
            START_DATE = StartDate,
            DURATION = Duration,
            TYPEX = Typex
        };

        await db.T_CONTESTS.AddAsync(newContest);

        await db.SaveChangesAsync();

        await db.AuditAsync(jwt, Constants.AuditActionInsert, newContest, $"Contest NAMEX: {newContest.NAMEX}", true);

        return this.Response("Contest added successfully", null);
    }

    [HttpPost]
    public async Task<IActionResult> EditContest([FromBody] JObject reqBody)
    {
        var EditEntityId = reqBody.GetParameter<int>("EditEntityId");
        var Inst = reqBody.GetParameter<int>("INST");
        var Namex = reqBody.GetParameter<string>("NAMEX");
        var Typex = reqBody.GetParameter<string>("TYPEX");
        var Overview = reqBody.GetParameter<string>("OVERVIEW");
        var StartDate = reqBody.GetParameter<DateTime>("START_DATE");
        var Duration = reqBody.GetParameter<int>("DURATION");

        var Contest = db.T_CONTESTS.First(e => e.ID == EditEntityId);

        Contest.INST = Inst;
        Contest.NAMEX = Namex;
        Contest.OVERVIEW = Overview;
        Contest.START_DATE = StartDate;
        Contest.DURATION = Duration;
        Contest.TYPEX = Typex;
        Contest.LUPDATE = DateTime.Now;

        db.T_CONTESTS.Update(Contest);

        await db.AuditAsync(jwt, Constants.AuditActionUpdate, Contest, $"Contest NAMEX: {Contest.NAMEX}");

        await db.SaveChangesAsync();

        return this.Response("Contest updated successfully", null);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteContest([FromBody] JObject reqBody)
    {
        var Id = reqBody.GetParameter<int>("Id");

        var Contest = db.T_CONTESTS.First(e => e.ID == Id);

        db.T_CONTESTS.Remove(Contest);

        await db.AuditAsync(jwt, Constants.AuditActionDelete, Contest, $"Contest NAMEX: {Contest.NAMEX}");

        await db.SaveChangesAsync();

        return this.Response("Contest deleted successfully", null);
    }

    [HttpPost]
    public async Task<IActionResult> ChangeContestStatus([FromBody] JObject reqBody)
    {
        var Id = reqBody.GetParameter<int>("Id");

        var Contest = db.T_CONTESTS.First(e => e.ID == Id);

        if (Contest.IS_ACTIVE == Constants.Yes)
        {
            Contest.IS_ACTIVE = Constants.No;
        }
        else
        {
            Contest.IS_ACTIVE = Constants.Yes;
        }

        db.T_CONTESTS.Update(Contest);

        await db.AuditAsync(jwt, Constants.AuditActionUpdate, Contest, $"Contest NAMEX: {Contest.NAMEX}");

        await db.SaveChangesAsync();

        return this.Response("Contest changes status successfully", null);
    }
}