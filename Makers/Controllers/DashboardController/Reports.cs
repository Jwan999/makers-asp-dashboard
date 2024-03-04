using Makers.Database.Entities;
using Makers.Security;
using Makers.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace Makers.Controllers;

public partial class DashboardController
{
    [HttpPost]
    public IActionResult GetReports([FromBody] JObject reqBody)
    {
        var data = from r in db.T_REPORTS
                   orderby r.ID descending
                   select r;

        return this.Response(null, data);
    }

    [HttpPost]
    public IActionResult GetReport([FromBody] JObject reqBody)
    {
        var Id = reqBody.GetParameter<int>("Id");
        var data = from r in db.T_REPORTS
                   where r.ID == Id
                   select r;

        return this.Response(null, data);
    }

    [HttpPost]
    public async Task<IActionResult> AddReport([FromBody] JObject reqBody)
    {
        var ReportName = reqBody.GetParameter<string>("REPORT_NAME");
        var ReportDesc = reqBody.GetParameter<string>("REPORT_DESC");
        var ReportCat = reqBody.GetParameter<string>("REPORT_CAT");

        T_REPORTS newReport = new()
        {
            ID = null,
            INSDATE = DateTime.Now,
            LUPDATE = DateTime.Now,
            REPORT_NAME = ReportName,
            REPORT_DESC = ReportDesc,
            REPORT_CAT = ReportCat,
            LAST_EXEC = DateTime.Now,
        };

        await db.T_REPORTS.AddAsync(newReport);

        await db.SaveChangesAsync();

        return this.Response("Report added successfully", null);
    }

    [HttpPost]
    public async Task<IActionResult> EditReport([FromBody] JObject reqBody)
    {
        var EditEntityId = reqBody.GetParameter<int>("EditEntityId");
        var ReportName = reqBody.GetParameter<string>("REPORT_NAME");
        var ReportDesc = reqBody.GetParameter<string>("REPORT_DESC");
        var ReportCat = reqBody.GetParameter<string>("REPORT_CAT");

        var Report = db.T_REPORTS.First(e => e.ID == EditEntityId);

        Report.REPORT_NAME = ReportName;
        Report.REPORT_DESC = ReportDesc;
        Report.REPORT_CAT = ReportCat;
        Report.LUPDATE = DateTime.Now;

        db.T_REPORTS.Update(Report);

        await db.SaveChangesAsync();

        return this.Response("Report updated successfully", null);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteReport([FromBody] JObject reqBody)
    {
        var Id = reqBody.GetParameter<int>("Id");

        var Report = db.T_REPORTS.First(e => e.ID == Id);

        db.T_REPORTS.Remove(Report);

        await db.SaveChangesAsync();

        return this.Response("Report deleted successfully", null);
    }

    [HttpPost]
    public IActionResult GetReportQuery([FromBody] JObject reqBody)
    {
        var Id = reqBody.GetParameter<int>("CodeId");

        var data = (from r in db.T_REPORTS
                    where r.ID == Id
                    select r.QUERYX).First();

        return this.Response(null, data);
    }

    [HttpPost]
    public async Task<IActionResult> EditReportQuery([FromBody] JObject reqBody)
    {
        var Id = reqBody.GetParameter<int>("CodeId");
        var Query = reqBody.GetParameter<string>("Code");

        var report = db.T_REPORTS.First(e => e.ID == Id);

        foreach (var forbiddenTable in Constants.ForbiddenTables.Split(','))
        {
            if (Query.ToUpper().Contains(forbiddenTable))
            {
                return this.Response("Bad query", null);
            }
        }

        report.QUERYX = Query;

        report.LUPDATE = DateTime.Now;

        db.T_REPORTS.Update(report);

        await db.AuditAsync(jwt, Constants.AuditActionUpdate, report, $"Update Report Query Code Id: {Id}");

        await db.SaveChangesAsync();

        return this.Response("Query updated successfully", null);
    }

    [HttpPost]
    public async Task<IActionResult> ExecuteReport([FromBody] JObject reqBody)
    {
        var ReportId = reqBody.GetParameter<int>("ReportId");
        var IsParameterized = reqBody.GetParameter<bool>("IsParameterized");
        var ReportParams = IsParameterized ? reqBody.GetValue("ReportParams") as JObject : null;

        var report = db.T_REPORTS.First(e => e.ID == ReportId);

        string query = report.QUERYX;

        if (IsParameterized && ReportParams is not null)
        {
            foreach (var param in ReportParams)
            {
                query = query.Replace("&" + param.Key, param.Value.ToString());
            }
        }

        var result = new List<Dictionary<string, object>>();

        using (var command = db.Database.GetDbConnection().CreateCommand())
        {
            command.CommandText = query;

            await db.Database.OpenConnectionAsync();

            using (var reader = await command.ExecuteReaderAsync())
            {
                var columnNames = new List<string>();

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    columnNames.Add(reader.GetName(i));
                }

                while (await reader.ReadAsync())
                {
                    var row = new Dictionary<string, object>();

                    foreach (var columnName in columnNames)
                    {
                        row[columnName] = reader[columnName];
                    }

                    result.Add(row);
                }
            }
        }

        var executionTime = DateTime.Now.CalculateExecutionTime(DateTime.Now);

        if (result.Count == 0)
        {
            throw new Exception("Report is empty");
        }

        report.LAST_EXEC = DateTime.Now;

        db.T_REPORTS.Update(report);

        await db.AuditAsync(jwt, Constants.AuditActionExecute, $"Report Name: {report.REPORT_NAME}");

        await db.SaveChangesAsync();

        return this.Response($"Report executed successfully in {executionTime}", result);
    }
}