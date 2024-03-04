using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Caching.Memory;
using Makers.Integrations;
using Makers.Security;
using Makers.Utilities;
using StatusCodes = Makers.Utilities.StatusCodes;

namespace Makers.Controllers;

public partial class DashboardController
{
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] JObject reqBody)
    {
        var Username = reqBody.GetParameter<string>("Username");
        var Password = reqBody.GetParameter<string>("Password");
        var BrowserId = reqBody.GetParameter<string>("BrowserId");
        var Browser = reqBody.Value<string>("Browser");
        var Os = reqBody.Value<string>("Os");

        var user = db.T_USERS.FirstOrDefault(e => e.USERNAME == Username && e.USER_ROLE == Constants.Admin);

        if (user is null)
        {
            throw new Exception($"User [{Username}] does not exist");
        }

        if (user.IS_BLOCKED == Constants.Yes)
        {
            throw new Exception("User is blocked, please contact your administrator");
        }

        Hasher.CheckHashedPassword(Password, user.SALT, out var hashedPassword);

        if (user.PASSWORD != hashedPassword)
        {
            user.FAILED_LOGIN_COUNTER += 1;
            user.LUPDATE = DateTime.Now;

            var maxFailedLogins = int.Parse(db.T_SYS_PARAMS.First(e => e.PARAMETER_NAME == Constants.SysParamUserMaxFailedLogins).PARAMETER_VALUE);

            if (user.FAILED_LOGIN_COUNTER >= maxFailedLogins)
            {
                user.IS_BLOCKED = Constants.Yes;
                user.BLOCK_DATE = DateTime.Now;

                db.T_USERS.Update(user);

                await db.SaveChangesAsync();

                return this.Response("You have exceeded your maximum login tries, your user is blocked", null, StatusCodes.Unauthorized);
            }

            db.T_USERS.Update(user);

            await db.SaveChangesAsync();

            return this.Response("Invalid username or password", null, StatusCodes.Unauthorized);
        }

     //   if (string.IsNullOrEmpty(user.DEVICE_TOKEN))
     //   {
            user.DEVICE_TOKEN = BrowserId;
            user.DEVICE_MODEL = Browser;
            user.DEVICE_OS = Os;
      //  }

        //else
        //{
        //    if (user.DEVICE_TOKEN != BrowserId)
        //    {
        //        var otpExpiration = db.T_SYS_PARAMS.First((e) => e.PARAMETER_NAME == Constants.SysParamOtpExpiration).PARAMETER_VALUE;

        //        user.OTP = SecurityHelper.GenerateRandMakerstring(10);
        //        user.OTP_EXP_DATE = DateTime.Now.AddMinutes(int.Parse(otpExpiration));
        //        user.LUPDATE = DateTime.Now;

        //        db.T_USERS.Update(user);

        //        await db.SaveChangesAsync();

        //        Sms.sendOtp(user.PHONE_NUMBER, user.OTP);

        //        return this.Response($"An OTP has been sent to number [{user.PHONE_NUMBER}]", new { username = user.USERNAME });
        //    }

        //    user.FAILED_LOGIN_COUNTER = 0;
        //    user.LAST_LOGIN_DATE = DateTime.Now;
        //}

        user.LUPDATE = DateTime.Now;

        db.T_USERS.Update(user);

        await db.SaveChangesAsync();

        var JWT = JwtHelper.GetJwt(cache.Get<string>(Constants.CacheKeyUsersTsk),
            Constants.JwtAudienceWebApplication,
            DateTime.Now.AddMinutes(cache.Get<int>(Constants.CacheKeyUsersJwtExpiration)),
            JwtHelper.SetClaims(db, user, cache));

        return this.Response("", new { JWT });
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword([FromBody] JObject reqBody)
    {
        var Id = reqBody.GetParameter<int>("Id");

        var defaultUserPassword = SecurityHelper.GenerateRandMakerstring(10);

        Hasher.GetHashedPassword(defaultUserPassword, out string hashedPassword, out string salt);

        var user = db.T_USERS.First(e => e.ID == Id);

        user.PASSWORD = hashedPassword;
        user.SALT = salt;
        user.LUPDATE = DateTime.Now;

        db.T_USERS.Update(user);

        await db.SaveChangesAsync();

        try
        {
            Sms.SendCredential(user.PHONE_NUMBER, defaultUserPassword);

            logger.LogMsg($"Successfully delivered the credentials via WhatsApp to [{user.PHONE_NUMBER}]");
        }

        catch (Exception e)
        {
            logger.LogMsg($"Failed to deliver the credentials via WhatsApp to [{user.PHONE_NUMBER}], error: {e}");

            return this.Response(
                $"Password has been reseted successfully, but were unable to deliver the credentials via WhatsApp to [{user.PHONE_NUMBER}], the error was {e.Message}",
                null);
        }

        logger.LogMsg($"User {user.USERNAME} reseted their password");

        return this.Response("Password has been reseted successfully", null);
    }

    //TODO: delete before release
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> FakeLogin([FromBody] JObject reqBody)
    {
        var Username = reqBody.GetParameter<string>("Username");

        var user = db.T_USERS.FirstOrDefault(entity => entity.USERNAME == Username);

        string JWT = JwtHelper.GetJwt(cache.Get<string>(Constants.CacheKeyUsersTsk),
            Constants.JwtAudienceWebApplication,
            DateTime.Now.AddMinutes(cache.Get<int>(Constants.CacheKeyUsersJwtExpiration)),
            JwtHelper.SetClaims(db, user, cache));

        return this.Response("Login successfully", JWT);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> VerifyOtp([FromBody] JObject reqBody)
    {
        var Username = reqBody.GetParameter<string>("Username");
        var OtpValue = reqBody.GetParameter<string>("OtpValue");
        var BrowserId = reqBody.GetParameter<string>("BrowserId");

        var user = db.T_USERS.FirstOrDefault(e => e.USERNAME == Username && e.DEVICE_TOKEN == BrowserId);

        var maxFailedOtps = int.Parse(db.T_SYS_PARAMS.First(e => e.PARAMETER_NAME == Constants.SysParamOtpFailCount).PARAMETER_VALUE);

        if (user is null || user.FAILED_OTP_COUNTER >= maxFailedOtps)
        {
            return this.Response(Constants.ResponseUnauthorized, null, StatusCodes.Unauthorized);
        }

        if (user.OTP != OtpValue || DateTime.Now >= user.OTP_EXP_DATE)
        {
            user.FAILED_OTP_COUNTER += 1;
            user.LUPDATE = DateTime.Now;

            db.T_USERS.Update(user);

            await db.SaveChangesAsync();

            return this.Response("Invalid or expired OTP", null, StatusCodes.Unauthorized);
        }

        var JWT = JwtHelper.GetJwt(cache.Get<string>(Constants.CacheKeyUsersTsk),
            Constants.JwtAudienceWebApplication,
            DateTime.Now.AddMinutes(cache.Get<int>(Constants.CacheKeyUsersJwtExpiration)),
            JwtHelper.SetClaims(db, user, cache));

        user.OTP = null;
        user.FAILED_LOGIN_COUNTER = 0;
        user.FAILED_OTP_COUNTER = 0;
        user.LAST_LOGIN_DATE = DateTime.Now;
        user.LUPDATE = DateTime.Now;

        db.T_USERS.Update(user);

        await db.SaveChangesAsync();

        return this.Response("", new { JWT });
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> ResendOtp([FromBody] JObject reqBody)
    {
        var Username = reqBody.GetParameter<string>("Username");

        var user = db.T_USERS.First(e => e.USERNAME == Username);

        var maxFailedOtps = int.Parse(db.T_SYS_PARAMS.First(e => e.PARAMETER_NAME == Constants.SysParamOtpFailCount).PARAMETER_VALUE);

        if (user.FAILED_OTP_COUNTER >= maxFailedOtps)
        {
            return this.Response("Maximum resend attempts reached", null, StatusCodes.Unauthorized);
        }

        var newOtpValue = SecurityHelper.GenerateRandMakerstring(10);
        var otpExpiration = db.T_SYS_PARAMS.First((e) => e.PARAMETER_NAME == Constants.SysParamOtpExpiration).PARAMETER_VALUE;


        user.OTP = newOtpValue;
        user.OTP_EXP_DATE = DateTime.Now.AddMinutes(int.Parse(otpExpiration));
        user.LUPDATE = DateTime.Now;

        db.T_USERS.Update(user);

        await db.SaveChangesAsync();

        Sms.sendOtp(user.PHONE_NUMBER, user.OTP);

        return this.Response("OTP has been resent successfully", new { username = user.USERNAME });
    }

    [HttpPost]
    public async Task<IActionResult> ChangePassword([FromBody] JObject reqBody)
    {
        var CurrentPassword = reqBody.GetParameter<string>("CurrentPassword");
        var NewPassword = reqBody.GetParameter<string>("NewPassword");
        var PasswordConfirmation = reqBody.GetParameter<string>("PasswordConfirmation");

        var user = db.T_USERS.First(e => e.ID == jwt.UserId);

        var minLength = db.T_SYS_PARAMS.First(e => e.PARAMETER_NAME == Constants.SysParamPwCheckMinLength).PARAMETER_VALUE;
        var checkForUppercase = db.T_SYS_PARAMS.First(e => e.PARAMETER_NAME == Constants.SysParamPwCheckUppercase).PARAMETER_VALUE;
        var checkForLowercase = db.T_SYS_PARAMS.First(e => e.PARAMETER_NAME == Constants.SysParamPwCheckLowercase).PARAMETER_VALUE;
        var checkForDigit = db.T_SYS_PARAMS.First(e => e.PARAMETER_NAME == Constants.SysParamPwCheckDigit).PARAMETER_VALUE;

        SecurityHelper.ApplyPasswordRules(user,
            CurrentPassword,
            NewPassword,
            PasswordConfirmation,
            user.PASSWORD,
            user.SALT,
            minLength,
            checkForUppercase,
            checkForLowercase,
            checkForDigit,
            out string newHashedPassword,
            out string newSalt);

        user.PASSWORD = newHashedPassword;
        user.SALT = newSalt;

        db.T_USERS.Update(user);

        await db.SaveChangesAsync();

        logger.LogMsg($"User {jwt.USER_NAME} changed their password");

        return this.Response("Password changed successfully, will redirect to login page", null);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateProfile([FromBody] JObject reqBody)
    {
        //TODO:check if you need to insert language in user table
        //var language = reqBody.GetParameter<string>("language");
        var FullName = reqBody.GetParameter<string>("FullName");
        var PhoneNumber = reqBody.GetParameter<string>("PhoneNumber");
        var Email = reqBody.Value<string>("Email");
        var Gender = reqBody.Value<string>("Gender");
        var Bdate = reqBody.Value<DateTime>("Bdate");

        logger.LogMsg($"User {jwt.USER_NAME} will try to update profile info");

        var User = db.T_USERS.First(e => e.ID == jwt.UserId);

        PhoneNumber.VerifyAndCorrectPhone(out var correctedNumber);

        User.FULL_NAME = FullName;
        User.PHONE_NUMBER = correctedNumber;
        User.EMAIL = Email;
        User.GENDER = Gender;
        User.BDATE = Bdate;
        User.LUPDATE = DateTime.Now;

        if (User.PHONE_NUMBER != correctedNumber)
        {
            User.OTP = SecurityHelper.GenerateRandMakerstring(10);

            db.T_USERS.Update(User);

            Sms.sendOtp(User.PHONE_NUMBER, User.OTP);

            logger.LogMsg($"User {jwt.USER_NAME} edited user {PhoneNumber} and sent OTP: {User.OTP}");

            return this.Response("OTP was sent to you via WhatsApp", null);
        }

        db.T_USERS.Update(User);

        await db.AuditAsync(jwt, Constants.AuditActionUpdate, User, $"User phone number: {PhoneNumber}");

        await db.SaveChangesAsync();

        return this.Response("Profile Updated successfully", null);
    }
    
    [HttpPost]
    public IActionResult GetPasswordRequirements()
    {
        var minLength = db.T_SYS_PARAMS.First(e => e.PARAMETER_NAME == Constants.SysParamPwCheckMinLength).PARAMETER_VALUE;
        var uppercase = db.T_SYS_PARAMS.First(e => e.PARAMETER_NAME == Constants.SysParamPwCheckUppercase).PARAMETER_VALUE;
        var lowercase = db.T_SYS_PARAMS.First(e => e.PARAMETER_NAME == Constants.SysParamPwCheckLowercase).PARAMETER_VALUE;
        var digit = db.T_SYS_PARAMS.First(e => e.PARAMETER_NAME == Constants.SysParamPwCheckDigit).PARAMETER_VALUE;

        var passwordRequirements = new List<string>
        {
            $"&#x2022; Password must be at least {minLength} characters long."
        };

        if (uppercase == "Y")
        {
            passwordRequirements.Add($"&#x2022; Password must contain at least one uppercase character.");
        }

        if (lowercase == "Y")
        {
            passwordRequirements.Add($"&#x2022; Password must contain at least one lowercase character.");
        }

        if (digit == "Y")
        {
            passwordRequirements.Add($"&#x2022; Password must contain at least one digit.");
        }

        passwordRequirements.Add($"&#x2022; Password must not contain your username.");

        return this.Response("", string.Join('\n', passwordRequirements));
    }
}