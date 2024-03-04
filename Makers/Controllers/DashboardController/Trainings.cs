using Makers.Database.Entities;
using Makers.Security;
using Makers.Utilities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Makers.Controllers;

public partial class DashboardController
{
    [HttpPost]
    public IActionResult GetTrainings([FromBody] JObject reqBody)
    {
        //var PageNumber = reqBody.GetParameter<int>("PageNumber");
        //var PageSize = reqBody.GetParameter<int>("PageSize");
        //var Filter = reqBody.GetParameter<string>("Filter");

        var data = from e in db.T_TRAINING
                   let TRAINERS = (from tt in db.T_MAP_TRAINING_TRAINERS
                                   join t in db.T_TRAINERS on tt.TRAINER_ID equals t.ID
                                   where tt.TRAINING_ID == e.ID
                                   select t.NAMEX).ToList()
                   let INST = (from tt in db.T_MAP_PROJ_INST
                               join i in db.T_INST on tt.INST_ID equals i.ID
                               where tt.PROJ_ID == e.PROJECT
                               select i.NAMEX).ToList()
                   orderby e.ID descending

                   select new
                   {
                       e.ID,
                       e.INSDATE,
                       e.LUPDATE,
                       e.NAMEX,
                       e.TYPEX,
                       e.LEC_NUM,
                       TRAINING_DAYS = string.IsNullOrWhiteSpace(e.TRAINING_DAYS) ? null : e.TRAINING_DAYS.Replace(",", " - "),
                       e.HOURSX,
                       END_DATE = e.END_DATE.Value.Date.ToShortDateString(),
                       START_DATE = e.START_DATE.Value.Date.ToShortDateString(),
                       e.ATTENDANCE_TYPE,
                       e.IS_PAID,
                       e.PRICEX,
                       e.IS_ACTIVE,
                       PROJECT = db.T_PROJECTS.First(p => p.ID == e.PROJECT).NAMEX,
                       TRAINERS = string.Join(" - ", TRAINERS),
                       INST = string.Join(" - ", INST),
                       e.PROGRESS
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
    public IActionResult GetTraining([FromBody] JObject reqBody)
    {
        var Id = reqBody.GetParameter<int>("Id");

        var trainers = (from tt in db.T_MAP_TRAINING_TRAINERS
                        join t in db.T_TRAINERS on tt.TRAINER_ID equals t.ID
                        where tt.TRAINING_ID == Id
                        select t.ID).ToList();
        var data = (from e in db.T_TRAINING
                    where e.ID == Id
                    select new
                    {
                        e.ID,
                        e.INSDATE,
                        e.LUPDATE,
                        e.NAMEX,
                        e.TYPEX,
                        e.LEC_NUM,
                        TRAINING_DAYS = string.IsNullOrWhiteSpace(e.TRAINING_DAYS) ? null : e.TRAINING_DAYS.Split(',', StringSplitOptions.RemoveEmptyEntries),
                        e.HOURSX,
                        e.START_DATE,
                        e.END_DATE,
                        e.ATTENDANCE_TYPE,
                        e.IS_PAID,
                        e.PRICEX,
                        e.PROJECT,
                        e.IS_ACTIVE,
                        TRAINERS = trainers,
                        e.PROGRESS
                    }).ToList();

        return this.Response(null, data);
    }

    [HttpPost]
    public async Task<IActionResult> AddTraining([FromBody] JObject reqBody)
    {
        var Namex = reqBody.GetParameter<string>("NAMEX");
        var Typex = reqBody.GetParameter<string>("TYPEX");
        var StartDate = reqBody.GetParameter<DateTime>("START_DATE");
        var EndDate = reqBody.GetParameter<DateTime>("END_DATE");
        var AttendanceType = reqBody.GetParameter<string>("ATTENDANCE_TYPE");
        var Hoursx = reqBody.GetParameter<int>("HOURSX");
        var LecNum = reqBody.GetParameter<int>("LEC_NUM");
        var IsPaid = reqBody.GetParameter<string>("IS_PAID");
        var Pricex = reqBody.Value<string>("PRICEX").ParseStringToNullableDouble();
        var ProjectId = reqBody.GetParameter<int>("PROJECT");
        var Progress = reqBody.GetParameter<string>("PROGRESS");
        var TrainingDays = reqBody.GetValue("TRAINING_DAYS");
        var Trainers = reqBody.GetValue("TRAINERS");

        if (Pricex is not null)
        {
            if (Pricex / 10000 < 1)
            {
                throw new Exception("Price should not be less than 10000 IQD");
            }
        }

        T_TRAINING newTraining = new()
        {
            ID = null,
            INSDATE = DateTime.Now,
            LUPDATE = DateTime.Now,
            NAMEX = Namex,
            TYPEX = Typex,
            START_DATE = StartDate,
            END_DATE = EndDate,
            ATTENDANCE_TYPE = AttendanceType,
            HOURSX = Hoursx,
            LEC_NUM = LecNum,
            TRAINING_DAYS = string.Join(',', TrainingDays),
            IS_PAID = IsPaid,
            PRICEX = Pricex,
            PROJECT = ProjectId,
            IS_ACTIVE = Constants.Yes,
            PROGRESS = Progress
        };

        await db.T_TRAINING.AddAsync(newTraining);

        await db.SaveChangesAsync();

        if (Trainers != null)
        {
            foreach (var trainer in Trainers)
            {
                T_MAP_TRAINING_TRAINERS newTrainingTrainer = new()
                {
                    ID = null,
                    INSDATE = DateTime.Now,
                    TRAINER_ID = int.Parse(trainer.ToString()),
                    TRAINING_ID = newTraining.ID,
                };

                await db.T_MAP_TRAINING_TRAINERS.AddAsync(newTrainingTrainer);
            }

            await db.SaveChangesAsync();
        }

        await db.AuditAsync(jwt, Constants.AuditActionInsert, newTraining, $"Training NAMEX: {newTraining.NAMEX}", true);

        return this.Response("Training added successfully", null);
    }

    [HttpPost]
    public async Task<IActionResult> EditTraining([FromBody] JObject reqBody)
    {
        var EditEntityId = reqBody.GetParameter<int>("EditEntityId");
        var Namex = reqBody.GetParameter<string>("NAMEX");
        var Typex = reqBody.GetParameter<string>("TYPEX");
        var StartDate = reqBody.GetParameter<DateTime>("START_DATE");
        var EndDate = reqBody.GetParameter<DateTime>("END_DATE");
        var AttendanceType = reqBody.GetParameter<string>("ATTENDANCE_TYPE");
        var Hoursx = reqBody.GetParameter<int>("HOURSX");
        var LecNum = reqBody.GetParameter<int>("LEC_NUM");
        var IsPaid = reqBody.GetParameter<string>("IS_PAID");
        var Pricex = reqBody.Value<string>("PRICEX").ParseStringToNullableDouble();
        var ProjectId = reqBody.GetParameter<int>("PROJECT");
        var Progress = reqBody.GetParameter<string>("PROGRESS");
        var TrainingDays = reqBody.GetValue("TRAINING_DAYS");
        var Trainers = reqBody.GetValue("TRAINERS");

        if (Pricex is not null)
        {
            if (Pricex / 10000 < 1)
            {
                throw new Exception("Price should not be less than 10000 IQD");
            }
        }

        var Training = db.T_TRAINING.First(e => e.ID == EditEntityId);

        Training.NAMEX = Namex;
        Training.TYPEX = Typex;
        Training.START_DATE = StartDate;
        Training.END_DATE = EndDate;
        Training.ATTENDANCE_TYPE = AttendanceType;
        Training.HOURSX = Hoursx;
        Training.LEC_NUM = LecNum;
        Training.TRAINING_DAYS = string.Join(',', TrainingDays);
        Training.IS_PAID = IsPaid;
        Training.PRICEX = Pricex;
        Training.PROJECT = ProjectId;
        Training.PROGRESS = Progress;
        Training.LUPDATE = DateTime.Now;

        var trainers = db.T_MAP_TRAINING_TRAINERS.Where(e => e.TRAINING_ID == EditEntityId);

        db.T_MAP_TRAINING_TRAINERS.RemoveRange(trainers);

        await db.SaveChangesAsync();

        if (Trainers != null)
        {
            foreach (var trainer in Trainers)
            {
                T_MAP_TRAINING_TRAINERS newTrainingTrainer = new()
                {
                    ID = null,
                    INSDATE = DateTime.Now,
                    TRAINER_ID = int.Parse(trainer.ToString()),
                    TRAINING_ID = Training.ID,
                };

                await db.T_MAP_TRAINING_TRAINERS.AddAsync(newTrainingTrainer);
            }

            await db.SaveChangesAsync();
        }

        db.T_TRAINING.Update(Training);

        await db.AuditAsync(jwt, Constants.AuditActionUpdate, Training, $"Training NAMEX: {Training.NAMEX}");

        await db.SaveChangesAsync();

        return this.Response("Training updated successfully", null);
    }

    [HttpPost]
    public async Task<IActionResult> ChangeTrainingStatus([FromBody] JObject reqBody)
    {
        var Id = reqBody.GetParameter<int>("Id");

        var Training = db.T_TRAINING.First(e => e.ID == Id);

        if (Training.IS_ACTIVE == Constants.Yes)
        {
            Training.IS_ACTIVE = Constants.No;
        }
        else
        {
            Training.IS_ACTIVE = Constants.Yes;
        }

        db.T_TRAINING.Update(Training);

        await db.AuditAsync(jwt, Constants.AuditActionUpdate, Training, $"Training NAMEX: {Training.NAMEX}");

        await db.SaveChangesAsync();

        return this.Response("Training changed status successfully", null);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteTraining([FromBody] JObject reqBody)
    {
        var Id = reqBody.GetParameter<int>("Id");

        var Training = db.T_TRAINING.First(e => e.ID == Id);

        db.T_TRAINING.Remove(Training);

        await db.AuditAsync(jwt, Constants.AuditActionDelete, Training, $"Training CODEX: {Training.NAMEX}");

        await db.SaveChangesAsync();

        return this.Response("Training deleted successfully", null);
    }
}