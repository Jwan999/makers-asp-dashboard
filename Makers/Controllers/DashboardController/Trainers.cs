using Makers.Database.Entities;
using Makers.Security;
using Makers.Utilities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Makers.Controllers
{
    public partial class DashboardController
    {
        [HttpPost]
        public IActionResult GetTrainers([FromBody] JObject reqBody)
        {
            //var PageNumber = reqBody.GetParameter<int>("PageNumber");
            //var PageSize = reqBody.GetParameter<int>("PageSize");
            //var Filter = reqBody.GetParameter<string>("Filter");

            var data = from e in db.T_TRAINERS
                       orderby e.ID descending
                       select e;

            //if (!string.IsNullOrEmpty(Filter))
            //{
            //    data = data.Where(e => e.NAMEX.Contains(Filter)).OrderByDescending(e => e.ID);
            //}

            //var dataCount = data.Count();

            //var resultData = SecurityHelper.Paging(PageSize, dataCount, data.Skip((PageNumber - 1) * PageSize).Take(PageSize));

            return this.Response(null, data);
        }

        [HttpPost]
        public IActionResult GetTrainer([FromBody] JObject reqBody)
        {
            var Id = reqBody.GetParameter<int>("Id");

            var data = from e in db.T_TRAINERS
                       where e.ID == Id
                       select e;

            return this.Response(null, data);
        }

        [HttpPost]
        public async Task<IActionResult> AddTrainer([FromBody] JObject reqBody)
        {
            var Name = reqBody.GetParameter<string>("NAMEX");
            var Phonex = reqBody.GetParameter<string>("PHONEX");
            var Emailx = reqBody.GetParameter<string>("EMAILX");
            var IsExternal = reqBody.GetParameter<string>("IS_EXTERNAL");
            var Img = reqBody.Value<string>("IMG");

            T_TRAINERS newTrainer = new()
            {
                ID = null,
                INSDATE = DateTime.Now,
                LUPDATE = DateTime.Now,
                PHONEX = Phonex,
                EMAILX = Emailx,
                NAMEX = Name,
                IS_EXTERNAL = IsExternal,
                IS_ACTIVE = Constants.Yes,
                IMG = Img
            };

            await db.T_TRAINERS.AddAsync(newTrainer);
            await db.SaveChangesAsync();

            await db.AuditAsync(jwt, Constants.AuditActionInsert, newTrainer, $"Trainer NAMEX: {newTrainer.NAMEX}", true);

            return this.Response("Trainer added successfully", null);
        }

        [HttpPost]
        public async Task<IActionResult> EditTrainer([FromBody] JObject reqBody)
        {
            var EditEntityId = reqBody.GetParameter<int>("EditEntityId");
            var Name = reqBody.GetParameter<string>("NAMEX");
            var Phonex = reqBody.GetParameter<string>("PHONEX");
            var Emailx = reqBody.GetParameter<string>("EMAILX");
            var IsExternal = reqBody.GetParameter<string>("IS_EXTERNAL");
            var Img = reqBody.Value<string>("IMG");

            var Trainer = db.T_TRAINERS.First(e => e.ID == EditEntityId);

            Trainer.NAMEX = Name;
            Trainer.PHONEX = Phonex;
            Trainer.EMAILX = Emailx;
            Trainer.IS_EXTERNAL = IsExternal;
            Trainer.IMG = Img;
            Trainer.LUPDATE = DateTime.Now;
            db.T_TRAINERS.Update(Trainer);

            await db.AuditAsync(jwt, Constants.AuditActionUpdate, Trainer, $"Trainer NAMEX: {Trainer.NAMEX}");

            await db.SaveChangesAsync();

            return this.Response("Trainer updated successfully", null);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeTrainerStatus([FromBody] JObject reqBody)
        {
            var Id = reqBody.GetParameter<int>("Id");

            var Trainer = db.T_TRAINERS.First(e => e.ID == Id);

            if (Trainer.IS_ACTIVE == Constants.Yes)
            {
                Trainer.IS_ACTIVE = Constants.No;
            }
            else
            {
                Trainer.IS_ACTIVE = Constants.Yes;
            }

            db.T_TRAINERS.Update(Trainer);

            await db.AuditAsync(jwt, Constants.AuditActionUpdate, Trainer, $"Trainer NAMEX: {Trainer.NAMEX}");

            await db.SaveChangesAsync();

            return this.Response("Trainer status changed successfully", null);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTrainer([FromBody] JObject reqBody)
        {
            var Id = reqBody.GetParameter<int>("Id");

            var Trainer = db.T_TRAINERS.First(e => e.ID == Id);

            db.T_TRAINERS.Remove(Trainer);

            await db.AuditAsync(jwt, Constants.AuditActionDelete, Trainer, $"Trainer Name: {Trainer.NAMEX}");

            await db.SaveChangesAsync();

            return this.Response("Trainer deleted successfully", null);
        }
    }
}

