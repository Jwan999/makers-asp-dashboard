using Makers.Database.Entities;
using Makers.Security;
using Makers.Utilities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Makers.Controllers;

public partial class DashboardController
{
    [HttpPost]
    public IActionResult GetSuccessStories([FromBody] JObject reqBody)
    {
        //var PageNumber = reqBody.GetParameter<int>("PageNumber");
        //var PageSize = reqBody.GetParameter<int>("PageSize");
        //var Filter = reqBody.GetParameter<string>("Filter");

        var data = from e in db.T_SUCCESS_STORIES
                   orderby e.ID descending
                   select new
                   {
                       e.ID,
                       e.IS_ACTIVE,
                       e.NAMEX,
                       e.STORY,
                       e.PHONE_NUMBER,
                       e.EMAIL,
                       e.LINKX,
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
    public IActionResult GetSuccessStory([FromBody] JObject reqBody)
    {
        var Id = reqBody.GetParameter<int>("Id");

        var data = from e in db.T_SUCCESS_STORIES
                   where e.ID == Id
                   select e;

        return this.Response(null, data);
    }

    [HttpPost]
    public async Task<IActionResult> AddSuccessStory([FromBody] JObject reqBody)
    {
        var Namex = reqBody.GetParameter<string>("NAMEX");
        var Story = reqBody.GetParameter<string>("STORY");
        var PhoneNumber = reqBody.Value<string>("PHONE_NUMBER");
        var Email = reqBody.Value<string>("EMAIL");
        var Linkx = reqBody.Value<string>("LINKX");

        T_SUCCESS_STORIES newSuccessStory = new()
        {
            ID = null,
            INSDATE = DateTime.Now,
            LUPDATE = DateTime.Now,
            NAMEX = Namex,
            STORY = Story,
            PHONE_NUMBER = PhoneNumber,
            EMAIL = Email,
            LINKX = Linkx,
            IS_ACTIVE = Constants.Yes,
        };

        await db.T_SUCCESS_STORIES.AddAsync(newSuccessStory);

        await db.SaveChangesAsync();

        await db.AuditAsync(jwt, Constants.AuditActionInsert, newSuccessStory, $"SuccessStory NAMEX: {newSuccessStory.NAMEX}", true);

        return this.Response("SuccessStory added successfully", null);
    }

    [HttpPost]
    public async Task<IActionResult> EditSuccessStory([FromBody] JObject reqBody)
    {
        var EditEntityId = reqBody.GetParameter<int>("EditEntityId");
        var Namex = reqBody.GetParameter<string>("NAMEX");
        var Story = reqBody.GetParameter<string>("STORY");
        var PhoneNumber = reqBody.Value<string>("PHONE_NUMBER");
        var Email = reqBody.Value<string>("EMAIL");
        var Linkx = reqBody.Value<string>("LINKX");

        var SuccessStory = db.T_SUCCESS_STORIES.First(e => e.ID == EditEntityId);

        SuccessStory.NAMEX = Namex;
        SuccessStory.STORY = Story;
        SuccessStory.PHONE_NUMBER = PhoneNumber;
        SuccessStory.EMAIL = Email;
        SuccessStory.LINKX = Linkx;
        SuccessStory.LUPDATE = DateTime.Now;

        db.T_SUCCESS_STORIES.Update(SuccessStory);

        await db.AuditAsync(jwt, Constants.AuditActionUpdate, SuccessStory, $"SuccessStory NAMEX: {SuccessStory.NAMEX}");

        await db.SaveChangesAsync();

        return this.Response("SuccessStory updated successfully", null);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteSuccessStory([FromBody] JObject reqBody)
    {
        var Id = reqBody.GetParameter<int>("Id");

        var SuccessStory = db.T_SUCCESS_STORIES.First(e => e.ID == Id);

        db.T_SUCCESS_STORIES.Remove(SuccessStory);

        await db.AuditAsync(jwt, Constants.AuditActionDelete, SuccessStory, $"SuccessStory NAMEX: {SuccessStory.NAMEX}");

        await db.SaveChangesAsync();

        return this.Response("SuccessStory deleted successfully", null);
    }

    [HttpPost]
    public async Task<IActionResult> ChangeSuccessStoryStatus([FromBody] JObject reqBody)
    {
        var Id = reqBody.GetParameter<int>("Id");

        var SuccessStory = db.T_SUCCESS_STORIES.First(e => e.ID == Id);

        if (SuccessStory.IS_ACTIVE == Constants.Yes)
        {
            SuccessStory.IS_ACTIVE = Constants.No;
        }
        else
        {
            SuccessStory.IS_ACTIVE = Constants.Yes;
        }

        db.T_SUCCESS_STORIES.Update(SuccessStory);

        await db.AuditAsync(jwt, Constants.AuditActionUpdate, SuccessStory, $"SuccessStory NAMEX: {SuccessStory.NAMEX}");

        await db.SaveChangesAsync();

        return this.Response("SuccessStory changes status successfully", null);
    }
}