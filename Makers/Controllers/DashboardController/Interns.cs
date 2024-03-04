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
        public IActionResult GetInterns([FromBody] JObject reqBody)
        {
            //var PageNumber = reqBody.GetParameter<int>("PageNumber");
            //var PageSize = reqBody.GetParameter<int>("PageSize");
            //var Filter = reqBody.GetParameter<string>("Filter");

            var data = from e in db.T_INTERNS
                       orderby e.ID descending
                       select new
                       {
                           e.ID,
                           e.NAMEX,
                           PROJ = db.T_PROJECTS.FirstOrDefault(p => p.ID == e.PROJ).NAMEX,
                           e.POSITION,
                           e.SALARY,
                           START_DATE = e.START_DATE.Value.ToShortDateString(),
                           e.IS_ACTIVE,
                           e.LINKX,
                           e.CITY,
                           e.EMAIL,
                           e.AGE,
                           e.GENDER,
                           e.PHONE_NUMBER
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
        public IActionResult GetIntern([FromBody] JObject reqBody)
        {
            var Id = reqBody.GetParameter<int>("Id");

            var data = from e in db.T_INTERNS
                       where e.ID == Id
                       select e;

            return this.Response(null, data);
        }

        [HttpPost]
        public async Task<IActionResult> AddIntern([FromBody] JObject reqBody)
        {
            var Namex = reqBody.GetParameter<string>("NAMEX");
            var Proj = reqBody.GetParameter<int>("PROJ");
            var Gender = reqBody.GetParameter<string>("GENDER");
            var City = reqBody.GetParameter<string>("CITY");
            var Email = reqBody.GetParameter<string>("EMAIL");
            var PhoneNumber = reqBody.GetParameter<string>("PHONE_NUMBER");
            var Age = reqBody.GetParameter<int>("AGE");
            var Position = reqBody.GetParameter<string>("POSITION");
            var Salary = reqBody.Value<string>("SALARY").ParseStringToNullableInt();
            var Linkx = reqBody.Value<string>("LINKX");
            var StartDate = reqBody.Value<DateTime>("START_DATE");

            PhoneNumber.VerifyAndCorrectPhone(out PhoneNumber);

            T_INTERNS newIntern = new()
            {
                ID = null,
                INSDATE = DateTime.Now,
                LUPDATE = DateTime.Now,
                NAMEX = Namex,
                PROJ = Proj,
                GENDER = Gender,
                CITY = City,
                EMAIL = Email,
                PHONE_NUMBER = PhoneNumber,
                AGE = Age,
                POSITION = Position,
                SALARY = Salary,
                LINKX = Linkx,
                START_DATE = StartDate,
                IS_ACTIVE = Constants.Yes,
            };

            await db.T_INTERNS.AddAsync(newIntern);

            await db.SaveChangesAsync();

            await db.AuditAsync(jwt, Constants.AuditActionInsert, newIntern, $"Intern NAMEX: {Namex}", true);

            return this.Response("Intern added successfully", null);
        }

        [HttpPost]
        public async Task<IActionResult> EditIntern([FromBody] JObject reqBody)
        {
            var EditEntityId = reqBody.GetParameter<int>("EditEntityId");
            var Namex = reqBody.GetParameter<string>("NAMEX");
            var Proj = reqBody.GetParameter<int>("PROJ");
            var Gender = reqBody.GetParameter<string>("GENDER");
            var City = reqBody.GetParameter<string>("CITY");
            var Email = reqBody.GetParameter<string>("EMAIL");
            var PhoneNumber = reqBody.GetParameter<string>("PHONE_NUMBER");
            var Age = reqBody.GetParameter<int>("AGE");
            var Position = reqBody.GetParameter<string>("POSITION");
            var Salary = reqBody.Value<string>("SALARY").ParseStringToNullableInt();
            var Linkx = reqBody.Value<string>("LINKX");
            var StartDate = reqBody.Value<DateTime>("START_DATE");

            var Intern = db.T_INTERNS.First(e => e.ID == EditEntityId);

            PhoneNumber.VerifyAndCorrectPhone(out PhoneNumber);

            Intern.NAMEX = Namex;
            Intern.PROJ = Proj;
            Intern.GENDER = Gender;
            Intern.CITY = City;
            Intern.EMAIL = Email;
            Intern.PHONE_NUMBER = PhoneNumber;
            Intern.AGE = Age;
            Intern.POSITION = Position;
            Intern.SALARY = Salary;
            Intern.LINKX = Linkx;
            Intern.START_DATE = StartDate;
            Intern.LUPDATE = DateTime.Now;

            db.T_INTERNS.Update(Intern);

            await db.AuditAsync(jwt, Constants.AuditActionUpdate, Intern, $"Intern NAMEX: {Namex}");

            await db.SaveChangesAsync();

            return this.Response("Intern updated successfully", null);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteIntern([FromBody] JObject reqBody)
        {
            var Id = reqBody.GetParameter<int>("Id");

            var Intern = db.T_INTERNS.First(e => e.ID == Id);

            db.T_INTERNS.Remove(Intern);

            await db.AuditAsync(jwt, Constants.AuditActionDelete, Intern, $"Intern NAMEX: {Intern.NAMEX}");

            await db.SaveChangesAsync();

            return this.Response("Intern deleted successfully", null);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeInternStatus([FromBody] JObject reqBody)
        {
            var Id = reqBody.GetParameter<int>("Id");

            var Intern = db.T_INTERNS.First(e => e.ID == Id);

            if (Intern.IS_ACTIVE == Constants.Yes)
            {
                Intern.IS_ACTIVE = Constants.No;
            }
            else
            {
                Intern.IS_ACTIVE = Constants.Yes;
            }

            db.T_INTERNS.Update(Intern);

            await db.AuditAsync(jwt, Constants.AuditActionUpdate, Intern, $"Intern NAMEX: {Intern.NAMEX}");

            await db.SaveChangesAsync();

            return this.Response("Intern changed status successfully", null);
        }
    }
}

