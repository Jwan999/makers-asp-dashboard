using Makers.Integrations;
using Makers.Security;
using Makers.Utilities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Makers.Controllers;

public partial class DashboardController
{
    [HttpPost]
    public IActionResult GetUsers([FromBody] JObject reqBody)
    {
        //var PageNumber = reqBody.GetParameter<int>("PageNumber");
        //var PageSize = reqBody.GetParameter<int>("PageSize");

        var data = from e in db.T_USERS
                   join r in db.T_ROLES on e.ROLE_ID_FK equals r.ID into userRole
                   from role in userRole.DefaultIfEmpty()
                   orderby e.ID descending
                   select new
                   {
                       e.ID,
                       e.FULL_NAME,
                       e.USERNAME,
                       e.PHONE_NUMBER,
                       e.EMAIL,
                       e.BDATE,
                       e.INSDATE,
                       e.LUPDATE,
                       e.GENDER,
                       role.ROLE_NAME,
                       e.IS_BLOCKED,
                       e.BLOCK_DATE
                   };

        //var dataCount = data.Count();

        //var resultData = SecurityHelper.Paging(PageSize, dataCount, data.Skip((PageNumber - 1) * PageSize).Take(PageSize));

        return this.Response(null, data);
    }

    [HttpPost]
    public IActionResult GetUser([FromBody] JObject reqBody)
    {
        var Id = reqBody.GetParameter<int>("Id");

        var data = from e in db.T_USERS
                   where e.ID == Id
                   orderby e.ID descending
                   select new
                   {
                       e.ID,
                       e.FULL_NAME,
                       e.USERNAME,
                       e.PHONE_NUMBER,
                       e.EMAIL,
                       e.BDATE,
                       e.INSDATE,
                       e.LUPDATE,
                       e.GENDER,
                       e.ROLE_ID_FK,
                       e.IS_BLOCKED,
                       e.BLOCK_DATE
                   };

        return this.Response(null, data);
    }

    [HttpPost]
    public async Task<IActionResult> AddUser([FromBody] JObject reqBody)
    {
        var Username = reqBody.GetParameter<string>("USERNAME");
        var FullName = reqBody.GetParameter<string>("FULL_NAME");
        var PhoneNumber = reqBody.GetParameter<string>("PHONE_NUMBER");
        var Email = reqBody.GetParameter<string>("EMAIL");
        var Gender = reqBody.GetParameter<string>("GENDER");
        var Bdate = reqBody.GetParameter<DateTime?>("BDATE");
        var RoleIdFk = reqBody.GetParameter<int>("ROLE_ID_FK");

        PhoneNumber.VerifyAndCorrectPhone(out var correctedNumber);

        var defaultUserPassword = SecurityHelper.GenerateRandMakerstring(10);

        Hasher.GetHashedPassword(defaultUserPassword, out var hashedPassword, out var salt);

        T_USERS newUser = new()
        {
            ID = null,
            INSDATE = DateTime.Now,
            LUPDATE = DateTime.Now,
            USERNAME = Username,
            PASSWORD = hashedPassword,
            SALT = salt,
            IS_BLOCKED = Constants.No,
            FULL_NAME = FullName,
            PHONE_NUMBER = correctedNumber,
            EMAIL = Email,
            USER_ROLE = Constants.Admin,
            GENDER = Gender,
            BDATE = Bdate,
            ROLE_ID_FK = RoleIdFk,
            REGISTERED_BY = jwt.UserId,
        };

        try
        {
            Sms.SendCredential(newUser.PHONE_NUMBER, defaultUserPassword);

            logger.LogMsg($"Successfully delivered the credentials via WhatsApp to [{newUser.PHONE_NUMBER}]");
        }

        catch (Exception e)
        {
            logger.LogMsg($"Failed to deliver the credentials via WhatsApp to [{newUser.PHONE_NUMBER}], error: {e}");
        }

        await db.T_USERS.AddAsync(newUser);
        await db.SaveChangesAsync();
        await db.AuditAsync(jwt, Constants.AuditActionInsert, newUser, $"User username: {Username}", true);

        return this.Response("User added successfully", null);
    }

    [HttpPost]
    public async Task<IActionResult> EditUser([FromBody] JObject reqBody)
    {
        var EditEntityId = reqBody.GetParameter<int>("EditEntityId");
        var Username = reqBody.GetParameter<string>("USERNAME");
        var FullName = reqBody.GetParameter<string>("FULL_NAME");
        var PhoneNumber = reqBody.GetParameter<string>("PHONE_NUMBER");
        var Email = reqBody.GetParameter<string>("EMAIL");
        var Gender = reqBody.GetParameter<string>("GENDER");
        var Bdate = reqBody.GetParameter<DateTime?>("BDATE");
        var RoleIdFk = reqBody.GetParameter<int>("ROLE_ID_FK");

        var User = db.T_USERS.First(e => e.ID == EditEntityId);

        PhoneNumber.VerifyAndCorrectPhone(out var correctedNumber);

        User.USERNAME = Username;
        User.FULL_NAME = FullName;
        User.PHONE_NUMBER = correctedNumber;
        User.EMAIL = Email;
        User.GENDER = Gender;
        User.BDATE = Bdate;
        User.ROLE_ID_FK = RoleIdFk;
        User.USER_ROLE = Constants.Admin;
        User.LUPDATE = DateTime.Now;

        if (User.PHONE_NUMBER != correctedNumber)
        {
            User.OTP = SecurityHelper.GenerateRandMakerstring();

            db.T_USERS.Update(User);

            Sms.sendOtp(User.PHONE_NUMBER, User.OTP);

            logger.LogMsg($"User {jwt.USER_NAME} edited user {Username} and sent OTP: {User.OTP}");

            return this.Response("OTP was sent to you via WhatsApp", null);
        }

        db.T_USERS.Update(User);

        await db.AuditAsync(jwt, Constants.AuditActionUpdate, User, $"User username: {Username}");

        await db.SaveChangesAsync();

        return this.Response("User updated successfully", null);
    }

    [HttpPost]
    public async Task<IActionResult> ChangeUserStatus([FromBody] JObject reqBody)
    {
        var Id = reqBody.GetParameter<int>("Id");

        var User = db.T_USERS.First(e => e.ID == Id);

        if (User.IS_BLOCKED == Constants.No)
        {
            User.IS_BLOCKED = Constants.Yes;
        }
        else
        {
            User.IS_BLOCKED = Constants.No;
        }

        db.T_USERS.Update(User);

        await db.AuditAsync(jwt, Constants.AuditActionUpdate, User, $"User username: {User.USERNAME}");

        await db.SaveChangesAsync();

        return this.Response("User status changed successfully", null);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteUser([FromBody] JObject reqBody)
    {
        var Id = reqBody.GetParameter<int>("Id");

        var Users = db.T_USERS.First(e => e.ID == Id);

        db.T_USERS.Remove(Users);

        await db.AuditAsync(jwt, Constants.AuditActionDelete, Users, $"User Name: {Users.FULL_NAME}");

        await db.SaveChangesAsync();

        return this.Response("Users deleted successfully", null);
    }
}