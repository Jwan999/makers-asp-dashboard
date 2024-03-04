using Makers.Database.Entities;
using Makers.Security;
using Makers.Utilities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Makers.Controllers;

public partial class DashboardController
{
    [HttpPost]
    public IActionResult GetProjects([FromBody] JObject reqBody)
    {
        //var PageNumber = reqBody.GetParameter<int>("PageNumber");
        //var PageSize = reqBody.GetParameter<int>("PageSize");
        //var Filter = reqBody.GetParameter<string>("Filter");

        var data = from e in db.T_PROJECTS
                   let INST = (from pi in db.T_MAP_PROJ_INST
                               join i in db.T_INST on pi.INST_ID equals i.ID
                               where e.ID == pi.PROJ_ID
                               select i.NAMEX).ToList()
                   orderby e.ID descending
                   select new
                   {
                       e.ID,
                       e.INSDATE,
                       e.LUPDATE,
                       e.DURATION,
                       e.ICON,
                       INST = string.Join(" - ", INST),
                       e.NAMEX,
                       e.IS_ACTIVE,
                       e.OVERVIEW,
                       END_DATE = e.END_DATE.Value.Date.ToShortDateString(),
                       START_DATE = e.START_DATE.Value.Date.ToShortDateString(),
                       e.LINKX
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
    public IActionResult GetProject([FromBody] JObject reqBody)
    {
        var Id = reqBody.GetParameter<int>("Id");

        var data = from e in db.T_PROJECTS
                   let INST = (from pi in db.T_MAP_PROJ_INST
                               join i in db.T_INST on pi.INST_ID equals i.ID
                               where e.ID == pi.PROJ_ID
                               select i.ID).ToList()
                   where e.ID == Id
                   select new
                   {
                       e.ID,
                       e.INSDATE,
                       e.LUPDATE,
                       INST,
                       e.END_DATE,
                       e.START_DATE,
                       e.DURATION,
                       e.OVERVIEW,
                       e.IS_ACTIVE,
                       e.NAMEX,
                       ICON = BaseURL + "/Image/" + e.ICON,
                       e.LINKX
                   };

        return this.Response(null, data);
    }

    [HttpPost]
    public async Task<IActionResult> AddProject([FromBody] JObject reqBody)
    {
        var Namex = reqBody.GetParameter<string>("NAMEX");
        var Insts = reqBody.GetParameter<JToken>("INST");
        var Overview = reqBody.GetParameter<string>("OVERVIEW");
        var StartDate = reqBody.GetParameter<DateTime>("START_DATE");
        var Duration = reqBody.GetParameter<int>("DURATION");
        var Icon = reqBody.GetValue("ICON");
        var Linkx = reqBody.Value<string>("LINKX");

        T_PROJECTS newProject = new()
        {
            ID = null,
            INSDATE = DateTime.Now,
            LUPDATE = DateTime.Now,
            NAMEX = Namex,
            OVERVIEW = Overview,
            START_DATE = StartDate,
            END_DATE = StartDate.AddMonths(Duration),
            DURATION = Duration,
            IS_ACTIVE = Constants.Yes,
            LINKX = Linkx
        };

        await db.T_PROJECTS.AddAsync(newProject);
        await db.SaveChangesAsync();

        if (Icon.Any())
        {
            fileManager.UploadImage(db, Icon[0].ToString(), (int)newProject.ID, "PROJ");
        }

        if (Insts != null)
        {
            foreach (var inst in Insts)
            {
                T_MAP_PROJ_INST newProjInst = new()
                {
                    ID = null,
                    INSDATE = DateTime.Now,
                    INST_ID = int.Parse(inst.ToString()),
                    PROJ_ID = newProject.ID,
                };

                await db.T_MAP_PROJ_INST.AddAsync(newProjInst);
            }

            await db.SaveChangesAsync();
        }

        await db.AuditAsync(jwt, Constants.AuditActionInsert, newProject, $"Project NAMEX: {newProject.NAMEX}", true);

        return this.Response("Project added successfully", null);
    }

    [HttpPost]
    public async Task<IActionResult> EditProject([FromBody] JObject reqBody)
    {
        var EditEntityId = reqBody.GetParameter<int>("EditEntityId");
        var Namex = reqBody.GetParameter<string>("NAMEX");
        var Insts = reqBody.GetValue("INST");
        var Overview = reqBody.GetParameter<string>("OVERVIEW");
        var StartDate = reqBody.GetParameter<DateTime>("START_DATE");
        var Duration = reqBody.GetParameter<int>("DURATION");
        var Icon = reqBody.Value<JToken>("ICON");
        var Linkx = reqBody.Value<string>("LINKX");

        var Project = db.T_PROJECTS.First(e => e.ID == EditEntityId);

        Project.NAMEX = Namex;
        Project.OVERVIEW = Overview;
        Project.START_DATE = StartDate;
        Project.END_DATE = StartDate.AddMonths(Duration);
        Project.DURATION = Duration;
        Project.LINKX = Linkx;
        Project.LUPDATE = DateTime.Now;

        if (Icon.Any())
        {
            fileManager.RemoveFile("Image", Project.ICON);

            fileManager.UploadImage(db, Icon[0].ToString(), (int)Project.ID, "PROJ");
        }

        var proInst = db.T_MAP_PROJ_INST.Where(e => e.PROJ_ID == EditEntityId);

        db.T_MAP_PROJ_INST.RemoveRange(proInst);

        if (Insts != null)
        {
            foreach (var inst in Insts)
            {
                T_MAP_PROJ_INST newProjInst = new()
                {
                    ID = null,
                    INSDATE = DateTime.Now,
                    INST_ID = int.Parse(inst.ToString()),
                    PROJ_ID = Project.ID,
                };

                await db.T_MAP_PROJ_INST.AddAsync(newProjInst);
            }

            await db.SaveChangesAsync();
        }

        db.T_PROJECTS.Update(Project);

        await db.AuditAsync(jwt, Constants.AuditActionUpdate, Project, $"Project NAMEX: {Project.NAMEX}");

        await db.SaveChangesAsync();

        return this.Response("Project updated successfully", null);
    }

    [HttpPost]
    public async Task<IActionResult> ChangeProjectStatus([FromBody] JObject reqBody)
    {
        var Id = reqBody.GetParameter<int>("Id");

        var Project = db.T_PROJECTS.First(e => e.ID == Id);

        if (Project.IS_ACTIVE == Constants.Yes)
        {
            Project.IS_ACTIVE = Constants.No;
        }
        else
        {
            Project.IS_ACTIVE = Constants.Yes;
        }

        db.T_PROJECTS.Update(Project);

        await db.AuditAsync(jwt, Constants.AuditActionDelete, Project, $"Project NAMEX: {Project.NAMEX}");

        await db.SaveChangesAsync();

        return this.Response("Project changed status successfully", null);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteProject([FromBody] JObject reqBody)
    {
        var Id = reqBody.GetParameter<int>("Id");

        var Project = db.T_PROJECTS.First(e => e.ID == Id);

        db.T_PROJECTS.Remove(Project);

        await db.AuditAsync(jwt, Constants.AuditActionDelete, Project, $"Project NAMEX: {Project.NAMEX}");

        await db.SaveChangesAsync();

        return this.Response("Project deleted successfully", null);
    }
}