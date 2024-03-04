using Makers.Database.Entities;
using Makers.Security;
using Makers.Utilities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Makers.Controllers;

public partial class DashboardController
{
    [HttpPost]
    public IActionResult GetStartups([FromBody] JObject reqBody)
    {
        //var PageNumber = reqBody.GetParameter<int>("PageNumber");
        //var PageSize = reqBody.GetParameter<int>("PageSize");
        //var Filter = reqBody.GetParameter<string>("Filter");

        var data = from e in db.T_STARTUPS
                   orderby e.ID descending
                   select new
                   {
                       e.ID,
                       e.INSDATE,
                       e.LUPDATE,
                       e.LOGO,
                       e.NAMEX,
                       e.IS_ACTIVE,
                       e.DESX,
                       START_DATE = e.START_DATE.Value.Date.ToShortDateString(),
                       e.INSTA_LINK,
                       e.FACEBOOK_LINK,
                       e.LOCATION_TYPE,
                       e.CATEGORY,
                       e.HAS_FEMALE_FOUNDER
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
    public IActionResult GetStartup([FromBody] JObject reqBody)
    {
        var Id = reqBody.GetParameter<int>("Id");

        var data = from e in db.T_STARTUPS
                   where e.ID == Id
                   select new
                   {
                       e.ID,
                       e.INSDATE,
                       e.LUPDATE,
                       e.START_DATE,
                       e.DESX,
                       e.LOCATION_TYPE,
                       e.IS_ACTIVE,
                       e.NAMEX,
                       LOGO = BaseURL + "/Image/" + e.LOGO,
                       e.FACEBOOK_LINK,
                       e.INSTA_LINK,
                       e.CATEGORY,
                       e.HAS_FEMALE_FOUNDER
                   };

        return this.Response(null, data);
    }

    [HttpPost]
    public async Task<IActionResult> AddStartup([FromBody] JObject reqBody)
    {
        var Namex = reqBody.GetParameter<string>("NAMEX");
        var Desc = reqBody.GetParameter<string>("DESX");
        var StartDate = reqBody.GetParameter<DateTime>("START_DATE");
        var LocationType = reqBody.GetParameter<string>("LOCATION_TYPE");
        var Category = reqBody.GetParameter<string>("CATEGORY");
        var Logo = reqBody.GetValue("LOGO");
        var InstaLink = reqBody.Value<string>("INSTA_LINK");
        var FacebookLink = reqBody.Value<string>("FACEBOOK_LINK");
        var HasFemaleFounder = reqBody.GetParameter<string>("HAS_FEMALE_FOUNDER");

        T_STARTUPS newStartup = new()
        {
            ID = null,
            INSDATE = DateTime.Now,
            LUPDATE = DateTime.Now,
            NAMEX = Namex,
            DESX = Desc,
            START_DATE = StartDate,
            LOCATION_TYPE = LocationType,
            CATEGORY = Category,
            FACEBOOK_LINK = FacebookLink,
            INSTA_LINK = InstaLink,
            IS_ACTIVE = Constants.Yes,
            HAS_FEMALE_FOUNDER = HasFemaleFounder
        };

        await db.T_STARTUPS.AddAsync(newStartup);
        await db.SaveChangesAsync();

        if (Logo.Any())
        {
            fileManager.UploadImage(db, Logo[0].ToString(), (int)newStartup.ID, "STARTUP");
        }



        await db.AuditAsync(jwt, Constants.AuditActionInsert, newStartup, $"Startup NAMEX: {newStartup.NAMEX}", true);

        return this.Response("Startup added successfully", null);
    }

    [HttpPost]
    public async Task<IActionResult> EditStartup([FromBody] JObject reqBody)
    {
        var EditEntityId = reqBody.GetParameter<int>("EditEntityId");
        var Namex = reqBody.GetParameter<string>("NAMEX");
        var Desc = reqBody.GetParameter<string>("DESX");
        var StartDate = reqBody.GetParameter<DateTime>("START_DATE");
        var LocationType = reqBody.GetParameter<string>("LOCATION_TYPE");
        var Category = reqBody.GetParameter<string>("CATEGORY");
        var Logo = reqBody.GetValue("LOGO");
        var InstaLink = reqBody.Value<string>("INSTA_LINK");
        var FacebookLink = reqBody.Value<string>("FACEBOOK_LINK");
        var HasFemaleFounder = reqBody.GetParameter<string>("HAS_FEMALE_FOUNDER");


        var Startup = db.T_STARTUPS.First(e => e.ID == EditEntityId);

        Startup.NAMEX = Namex;
        Startup.START_DATE = StartDate;
        Startup.DESX = Desc;
        Startup.CATEGORY = Category;
        Startup.LOCATION_TYPE = LocationType;
        Startup.INSTA_LINK = InstaLink;
        Startup.FACEBOOK_LINK = FacebookLink;
        Startup.HAS_FEMALE_FOUNDER = HasFemaleFounder;
        Startup.LUPDATE = DateTime.Now;

        if (Logo.Any())
        {
            fileManager.RemoveFile("Image", Startup.LOGO);

            fileManager.UploadImage(db, Logo[0].ToString(), (int)Startup.ID, "STARTUP");
        }

        db.T_STARTUPS.Update(Startup);

        await db.AuditAsync(jwt, Constants.AuditActionUpdate, Startup, $"Project NAMEX: {Startup.NAMEX}");

        await db.SaveChangesAsync();

        return this.Response("Project updated successfully", null);
    }

    [HttpPost]
    public async Task<IActionResult> ChangeStartupStatus([FromBody] JObject reqBody)
    {
        var Id = reqBody.GetParameter<int>("Id");

        var Startup = db.T_STARTUPS.First(e => e.ID == Id);

        if (Startup.IS_ACTIVE == Constants.Yes)
        {
            Startup.IS_ACTIVE = Constants.No;
        }
        else
        {
            Startup.IS_ACTIVE = Constants.Yes;
        }

        db.T_STARTUPS.Update(Startup);

        await db.AuditAsync(jwt, Constants.AuditActionUpdate, Startup, $"Startup NAMEX: {Startup.NAMEX}");

        await db.SaveChangesAsync();

        return this.Response("Startup changed status successfully", null);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteStartup([FromBody] JObject reqBody)
    {
        var Id = reqBody.GetParameter<int>("Id");

        var Startup = db.T_STARTUPS.First(e => e.ID == Id);

        db.T_STARTUPS.Remove(Startup);

        await db.AuditAsync(jwt, Constants.AuditActionDelete, Startup, $"Startup NAMEX: {Startup.NAMEX}");

        await db.SaveChangesAsync();

        return this.Response("Startup deleted successfully", null);
    }
}