using Makers.Database.Entities;
using Makers.Security;
using Makers.Utilities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Makers.Controllers;

public partial class DashboardController
{
    [HttpPost]
    public IActionResult GetInstitutions([FromBody] JObject reqBody)
    {
        //var PageNumber = reqBody.GetParameter<int>("PageNumber");
        //var PageSize = reqBody.GetParameter<int>("PageSize");
        //var Filter = reqBody.Value<string>("Filter");

        var data = from e in db.T_INST
                   orderby e.ID descending
                   select new
                   {
                       e.ID,
                       e.IS_ACTIVE,
                       e.LOGO,
                       e.NAMEX,
                       e.INSDATE,
                       e.LUPDATE,
                       START_DATE = e.START_DATE.Value.Date.ToShortDateString()
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
    public IActionResult GetInstitution([FromBody] JObject reqBody)
    {
        var Id = reqBody.GetParameter<int>("Id");

        var data = from e in db.T_INST
                   where e.ID == Id
                   select new
                   {
                       e.ID,
                       e.INSDATE,
                       e.LUPDATE,
                       e.NAMEX,
                       e.START_DATE,
                       LOGO = BaseURL + "/Image/" + e.LOGO,
                       e.IS_ACTIVE
                   };

        return this.Response(null, data);
    }

    [HttpPost]
    public async Task<IActionResult> AddInstitution([FromBody] JObject reqBody)
    {
        var Namex = reqBody.GetParameter<string>("NAMEX");
        var StartDate = reqBody.GetParameter<DateTime>("START_DATE");
        var Icon = reqBody.Value<JToken>("ICON");

        T_INST newInstitution = new()
        {
            ID = null,
            INSDATE = DateTime.Now,
            LUPDATE = DateTime.Now,
            NAMEX = Namex,
            START_DATE = StartDate,
            IS_ACTIVE = Constants.Yes
        };

        await db.T_INST.AddAsync(newInstitution);
        await db.SaveChangesAsync();

        if (Icon.Any())
        {
            fileManager.UploadImage(db, Icon[0].ToString(), (int)newInstitution.ID, "INST");
        }

        await db.AuditAsync(jwt, Constants.AuditActionInsert, newInstitution, $"Institution CODEX: {Namex}", true);

        return this.Response("Institution added successfully", null);
    }

    [HttpPost]
    public async Task<IActionResult> EditInstitution([FromBody] JObject reqBody)
    {
        var EditEntityId = reqBody.GetParameter<int>("EditEntityId");
        var Namex = reqBody.GetParameter<string>("NAMEX");
        var StartDate = reqBody.GetParameter<DateTime>("START_DATE");
        var Icon = reqBody.Value<JToken>("ICON");

        var Institution = db.T_INST.First(e => e.ID == EditEntityId);

        Institution.NAMEX = Namex;
        Institution.START_DATE = StartDate;
        Institution.LUPDATE = DateTime.Now;

        if (Icon.Any())
        {
            fileManager.RemoveFile("Image", Institution.LOGO);

            fileManager.UploadImage(db, Icon[0].ToString(), (int)Institution.ID, "INST");
        }

        db.T_INST.Update(Institution);

        await db.AuditAsync(jwt, Constants.AuditActionUpdate, Institution, $"Institution CODEX:  {Namex}");

        await db.SaveChangesAsync();

        return this.Response("Institution updated successfully", null);
    }

    [HttpPost]
    public async Task<IActionResult> ChangeInstitutionStatus([FromBody] JObject reqBody)
    {
        var Id = reqBody.GetParameter<int>("Id");

        var Institution = db.T_INST.First(e => e.ID == Id);

        if (Institution.IS_ACTIVE == Constants.Yes)
        {
            Institution.IS_ACTIVE = Constants.No;
        }
        else
        {
            Institution.IS_ACTIVE = Constants.Yes;
        }

        db.T_INST.Update(Institution);

        await db.AuditAsync(jwt, Constants.AuditActionUpdate, Institution, $"Institution CODEX: {Institution.NAMEX}");

        await db.SaveChangesAsync();

        return this.Response("Institution deactivated successfully", null);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteInstitution([FromBody] JObject reqBody)
    {
        var Id = reqBody.GetParameter<int>("Id");

        var Institution = db.T_INST.First(e => e.ID == Id);

        db.T_INST.Remove(Institution);

        await db.AuditAsync(jwt, Constants.AuditActionDelete, Institution, $"Institution CODEX: {Institution.NAMEX}");

        await db.SaveChangesAsync();

        return this.Response("Institution deleted successfully", null);
    }
}