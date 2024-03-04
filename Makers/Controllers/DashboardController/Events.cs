using Makers.Database.Entities;
using Makers.Security;
using Makers.Utilities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Makers.Controllers;

public partial class DashboardController
{
    [HttpPost]
    public IActionResult GetEvents([FromBody] JObject reqBody)
    {
        //var PageNumber = reqBody.GetParameter<int>("PageNumber");
        //var PageSize = reqBody.GetParameter<int>("PageSize");
        //var Filter = reqBody.GetParameter<string>("Filter");

        var Id = reqBody.GetParameter<int>("PROJ_ID");

        var data = from e in db.T_EVENTS
                   orderby e.ID descending
                   where e.PROJ_ID == Id
                   select new
                   {
                       e.NAMEX,
                       e.ID,
                       PROJ = db.T_PROJECTS.FirstOrDefault(p => p.ID == e.PROJ_ID).NAMEX,
                       e.OVERVIEW,
                       e.LINKX,
                       e.LOCATIONX,
                       e.IS_ACTIVE,
                       e.INSDATE,
                       e.LUPDATE,
                       DATEX = e.DATEX.Value.Date.ToShortDateString(),
                       e.PARTICIPANTS_NUM
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
    public IActionResult GetEvent([FromBody] JObject reqBody)
    {
        var Id = reqBody.GetParameter<int>("Id");

        var data = from e in db.T_EVENTS
                   where e.ID == Id
                   select e;

        return this.Response(null, data);
    }

    [HttpPost]
    public async Task<IActionResult> AddEvent([FromBody] JObject reqBody)
    {
        var ProjId = reqBody.GetParameter<int>("PROJ_ID");
        var Namex = reqBody.GetParameter<string>("NAMEX");
        var Description = reqBody.GetParameter<string>("OVERVIEW");
        var Datex = reqBody.GetParameter<DateTime>("DATEX");
        var Duration = reqBody.GetParameter<int>("DURATION");
        var ParticipantsNum = reqBody.Value<string>("PARTICIPANTS_NUM").ParseStringToNullableInt();
        var Locationx = reqBody.Value<string>("LOCATIONX");
        var Linx = reqBody.Value<string>("LINKX");

        T_EVENTS newEvent = new()
        {
            ID = null,
            INSDATE = DateTime.Now,
            LUPDATE = DateTime.Now,
            PROJ_ID = ProjId,
            NAMEX = Namex,
            OVERVIEW = Description,
            DATEX = Datex,
            DURATION = Duration,
            PARTICIPANTS_NUM = ParticipantsNum,
            LOCATIONX = Locationx,
            LINKX = Linx,
            IS_ACTIVE = Constants.Yes,
        };

        await db.T_EVENTS.AddAsync(newEvent);

        await db.SaveChangesAsync();

        await db.AuditAsync(jwt, Constants.AuditActionInsert, newEvent, $"Event NAMEX: {Namex}", true);

        return this.Response("Event added successfully", null);
    }

    [HttpPost]
    public async Task<IActionResult> EditEvent([FromBody] JObject reqBody)
    {
        var EditEntityId = reqBody.GetParameter<int>("EditEntityId");
        var ProjId = reqBody.GetParameter<int>("PROJ_ID");
        var Namex = reqBody.GetParameter<string>("NAMEX");
        var Description = reqBody.GetParameter<string>("OVERVIEW");
        var Datex = reqBody.GetParameter<DateTime>("DATEX");
        var Duration = reqBody.GetParameter<int>("DURATION");
        var ParticipantsNum = reqBody.Value<string>("PARTICIPANTS_NUM").ParseStringToNullableInt();
        var Locationx = reqBody.Value<string>("LOCATIONX");
        var Linx = reqBody.Value<string>("LINKX");

        var Event = db.T_EVENTS.First(e => e.ID == EditEntityId);

        Event.PROJ_ID = ProjId;
        Event.NAMEX = Namex;
        Event.OVERVIEW = Description;
        Event.DATEX = Datex;
        Event.DURATION = Duration;
        Event.PARTICIPANTS_NUM = ParticipantsNum;
        Event.LOCATIONX = Locationx;
        Event.LINKX = Linx;
        Event.LUPDATE = DateTime.Now;

        db.T_EVENTS.Update(Event);

        await db.AuditAsync(jwt, Constants.AuditActionUpdate, Event, $"Event NAMEX: {Namex}");

        await db.SaveChangesAsync();

        return this.Response("Event updated successfully", null);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteEvent([FromBody] JObject reqBody)
    {
        var Id = reqBody.GetParameter<int>("Id");

        var Event = db.T_EVENTS.First(e => e.ID == Id);

        db.T_EVENTS.Remove(Event);

        await db.AuditAsync(jwt, Constants.AuditActionDelete, Event, $"Event NAMEX: {Event.NAMEX}");

        await db.SaveChangesAsync();

        return this.Response("Event deleted successfully", null);
    }

    [HttpPost]
    public async Task<IActionResult> ChangeEventStatus([FromBody] JObject reqBody)
    {
        var Id = reqBody.GetParameter<int>("Id");

        var Event = db.T_EVENTS.First(e => e.ID == Id);

        if (Event.IS_ACTIVE == Constants.Yes)
        {
            Event.IS_ACTIVE = Constants.No;
        }
        else
        {
            Event.IS_ACTIVE = Constants.Yes;
        }

        db.T_EVENTS.Update(Event);

        await db.AuditAsync(jwt, Constants.AuditActionUpdate, Event, $"Event NAMEX: {Event.NAMEX}");

        await db.SaveChangesAsync();

        return this.Response("Event changed status successfully", null);
    }
}