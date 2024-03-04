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
        public IActionResult GetStudents([FromBody] JObject reqBody)
        {
            //var PageNumber = reqBody.GetParameter<int>("PageNumber");
            //var PageSize = reqBody.GetParameter<int>("PageSize");
            //var Filter = reqBody.GetParameter<string>("Filter");

            var data = from e in db.T_STUDENTS
                       orderby e.ID descending
                       select new
                       {
                           e.ID,
                           e.IS_ACTIVE,
                           e.NAMEX,
                           e.GENDER,
                           TRAINING = db.T_TRAINING.FirstOrDefault(t => t.ID == e.TRAINING).NAMEX,
                           e.EMAIL,
                           e.PHONE_NUMBER,
                           e.UNIVERSITY,
                           e.FIELD,
                           e.AGE,
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
        public IActionResult GetStudent([FromBody] JObject reqBody)
        {
            var Id = reqBody.GetParameter<int>("Id");

            var data = from e in db.T_STUDENTS
                       where e.ID == Id
                       select e;

            return this.Response(null, data);
        }

        [HttpPost]
        public async Task<IActionResult> AddStudent([FromBody] JObject reqBody)
        {
            var Namex = reqBody.GetParameter<string>("NAMEX");
            var Gender = reqBody.GetParameter<string>("GENDER");
            var Training = reqBody.GetParameter<int>("TRAINING");
            var Email = reqBody.Value<string>("EMAIL");
            var PhoneNumber = reqBody.GetParameter<string>("PHONE_NUMBER");
            var University = reqBody.GetParameter<string>("UNIVERSITY");
            var Field = reqBody.GetParameter<string>("FIELD");
            var Age = reqBody.Value<string>("AGE").ParseStringToNullableInt();

            PhoneNumber.VerifyAndCorrectPhone(out PhoneNumber);

            T_STUDENTS newStudent = new()
            {
                ID = null,
                INSDATE = DateTime.Now,
                LUPDATE = DateTime.Now,
                NAMEX = Namex,
                GENDER = Gender,
                TRAINING = Training,
                EMAIL = Email,
                PHONE_NUMBER = PhoneNumber,
                UNIVERSITY = University,
                FIELD = Field,
                AGE = Age,
                IS_ACTIVE = Constants.Yes
            };

            await db.T_STUDENTS.AddAsync(newStudent);

            await db.SaveChangesAsync();

            await db.AuditAsync(jwt, Constants.AuditActionInsert, newStudent, $"Student NAMEX: {newStudent.NAMEX}", true);

            return this.Response("Student added successfully", null);
        }

        [HttpPost]
        public async Task<IActionResult> EditStudent([FromBody] JObject reqBody)
        {
            var EditEntityId = reqBody.GetParameter<int>("EditEntityId");
            var Namex = reqBody.GetParameter<string>("NAMEX");
            var Gender = reqBody.GetParameter<string>("GENDER");
            var Training = reqBody.GetParameter<int>("TRAINING");
            var Email = reqBody.Value<string>("EMAIL");
            var PhoneNumber = reqBody.GetParameter<string>("PHONE_NUMBER");
            var University = reqBody.GetParameter<string>("UNIVERSITY");
            var Field = reqBody.GetParameter<string>("FIELD");
            var Age = reqBody.Value<string>("AGE").ParseStringToNullableInt();

            var Student = db.T_STUDENTS.First(e => e.ID == EditEntityId);

            PhoneNumber.VerifyAndCorrectPhone(out PhoneNumber);

            Student.NAMEX = Namex;
            Student.GENDER = Gender;
            Student.TRAINING = Training;
            Student.EMAIL = Email;
            Student.PHONE_NUMBER = PhoneNumber;
            Student.UNIVERSITY = University;
            Student.FIELD = Field;
            Student.AGE = Age;
            Student.LUPDATE = DateTime.Now;

            db.T_STUDENTS.Update(Student);

            await db.AuditAsync(jwt, Constants.AuditActionUpdate, Student, $"Student NAMEX: {Student.NAMEX}");

            await db.SaveChangesAsync();

            return this.Response("Student updated successfully", null);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteStudent([FromBody] JObject reqBody)
        {
            var Id = reqBody.GetParameter<int>("Id");

            var Student = db.T_STUDENTS.First(e => e.ID == Id);

            db.T_STUDENTS.Remove(Student);

            await db.AuditAsync(jwt, Constants.AuditActionDelete, Student, $"Student NAMEX: {Student.NAMEX}");

            await db.SaveChangesAsync();

            return this.Response("Student deleted successfully", null);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeStudentStatus([FromBody] JObject reqBody)
        {
            var Id = reqBody.GetParameter<int>("Id");

            var Student = db.T_STUDENTS.First(e => e.ID == Id);

            if (Student.IS_ACTIVE == Constants.Yes)
            {
                Student.IS_ACTIVE = Constants.No;
            }
            else
            {
                Student.IS_ACTIVE = Constants.Yes;
            }
            db.T_STUDENTS.Update(Student);

            await db.AuditAsync(jwt, Constants.AuditActionUpdate, Student, $"Student NAMEX: {Student.NAMEX}");

            await db.SaveChangesAsync();

            return this.Response("Student changes status successfully", null);
        }
    }
}

