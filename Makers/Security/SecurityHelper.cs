using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Makers.Database.Contexts;
using Makers.Database.Entities;
using Makers.Utilities;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using Dapper;

namespace Makers.Security;

public static class SecurityHelper
{
    private static readonly JsonSerializerSettings jsonSerializerSettings = new()
    {
        DateFormatString = "yyyy/MM/dd HH:mm:ss",
        Formatting = Formatting.Indented
    };

    public static async Task AuditAsync<T>(this Db db,
        IJwt user,
        string actionType,
        T data,
        string refDesc,
        bool saveChanges = false)
    {
        int? refId = null;
        string refType = string.Empty;

        var userRec = db.T_USERS.First(e => e.ID == user.UserId);

        if (data is not null && data.GetType().IsClass)
        {
            refId = (int?)data.GetType().GetProperty("ID")?.GetValue(data);
            refType = data.GetType().Name;
        }

        if (actionType == Constants.AuditActionUpdate &&
            refId is not null && !string.IsNullOrEmpty(refType))
        {
            var olds = db.T_AUDIT.Where(x => x.REF_ID == refId &&
                                             x.REF_TYPE == refType &&
                                             (x.ACTION_TYPE == Constants.AuditActionInsert ||
                                              x.ACTION_TYPE == Constants.AuditActionUpdate))
                .ToList();

            if (!olds.Any())
            {
                string sql = $"SELECT * FROM {refType} WHERE ID = {refId}";

                var oldData = db.Database.GetDbConnection()
                    .QueryFirst<T>(sql, commandType: CommandType.Text);

                if (typeof(T) == typeof(T_USERS))
                {
                    oldData.GetType().GetProperty("PASSWORD").SetValue(oldData, "");
                    oldData.GetType().GetProperty("SALT").SetValue(oldData, "");
                }

                T_AUDIT audit1 = new()
                {
                    ID = null,
                    USER_ID = user.UserId,
                    USER_TYPE = userRec.USER_ROLE,
                    AUDIT_DATE = DateTime.Now.AddMinutes(-1),
                    ACTION_TYPE = Constants.AuditActionInsert,
                    REF_ID = refId,
                    REF_TYPE = refType,
                    REF_DESC = "Insert By Makers before update",
                    REF_OBJECT = JsonConvert.SerializeObject(oldData, jsonSerializerSettings)
                };

                await db.AddAsync(audit1);
            }
        }

        T_AUDIT audit2 = new()
        {
            ID = null,
            USER_ID = user.UserId,
            USER_TYPE = userRec.USER_ROLE,
            AUDIT_DATE = DateTime.Now,
            ACTION_TYPE = actionType,
            REF_ID = refId,
            REF_TYPE = refType,
            REF_DESC = refDesc,
            REF_OBJECT = JsonConvert.SerializeObject(data, jsonSerializerSettings)
        };

        await db.AddAsync(audit2);

        if (saveChanges) await db.SaveChangesAsync();
    }

    public static async Task AuditAsync(this Db db,
        IJwt user,
        string actionType,
        string refDesc,
        bool saveChanges = false)
    {
        T_AUDIT audit = new()
        {
            ID = null,
            USER_ID = user.UserId,
            AUDIT_DATE = DateTime.Now,
            ACTION_TYPE = actionType,
            REF_DESC = refDesc
        };

        await db.AddAsync(audit);

        if (saveChanges) await db.SaveChangesAsync();
    }

    public static void ApplyPasswordRules(T_USERS user,
        string providedCurrentPw,
        string newPw,
        string newPwConfirm,
        string currentPwHashed,
        string currentSalt,
        string minLength,
        string checkForUppercase,
        string checkForLowercase,
        string checkForDigit,
        out string newHashedPw,
        out string newSalt)
    {
        Hasher.CheckHashedPassword(providedCurrentPw, currentSalt, out string providedCurrentPwHashed);

        if (currentPwHashed != providedCurrentPwHashed)
        {
            throw new Exception("Invalid current password");
        }

        if (providedCurrentPw == newPw)
        {
            throw new Exception("New password cannot be the same as old password");
        }

        if (newPw != newPwConfirm)
        {
            throw new Exception("New password is not the same as its confirmation");
        }

        if (newPw.Length < int.Parse(minLength))
        {
            throw new Exception($"Password must at least be {minLength} characters long");
        }

        bool hasUppercase = false, hasLowercase = false, hasDigit = false;

        foreach (var c in newPw)
        {
            if (char.IsUpper(c))
            {
                hasUppercase = true;
            }

            if (char.IsLower(c))
            {
                hasLowercase = true;
            }

            if (char.IsDigit(c))
            {
                hasDigit = true;
            }
        }

        if (checkForUppercase == "Y" && !hasUppercase)
        {
            throw new Exception("Password must contain at least one upper case character");
        }

        if (checkForLowercase == "Y" && !hasLowercase)
        {
            throw new Exception("Password must contain at least one lower case character");
        }

        if (checkForDigit == "Y" && !hasDigit)
        {
            throw new Exception("Password must contain at least one digit");
        }

        Hasher.GetHashedPassword(newPw, out newHashedPw, out newSalt);
    }

    public static string GetSecret(string encodedConfig)
    {
        try
        {
            if (encodedConfig is null)
            {
                throw new Exception($"Config not found: [{encodedConfig}]");
            }

            return Encoding.ASCII.GetString(Convert.FromBase64String(encodedConfig));
        }

        catch (Exception e)
        {
            throw new Exception($"Could not decode the secret [{encodedConfig}], the error was: {e}");
        }
    }

    public static string GenerateRandMakerstring(int length = 8, string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789")
    {
        Random random = new();

        char[] chars = new char[length];

        for (int i = 0; i < length; i++)
            chars[i] = validChars[random.Next(0, validChars.Length)];

        return new string(chars);
    }

    public static JObject RemoveSensetiveReqData(object reqData)
    {
        JObject cleanObj = new();

        if (reqData is not null)
        {
            if (reqData is string)
            {
                cleanObj = JObject.Parse(reqData as string).DeepClone() as JObject;
            }

            else
            {
                cleanObj = (reqData as JObject).DeepClone() as JObject;
            }

            cleanObj.Remove("Password");
            cleanObj.Remove("CurrentPassword");
            cleanObj.Remove("NewPassword");
            cleanObj.Remove("PasswordConfirmation");
            cleanObj.Remove("Code");
            cleanObj.Remove("PW");
            cleanObj.Remove("PARAMETER_VALUE");
            cleanObj.Remove("TOKEN");
        }

        return cleanObj;
    }

    public static string Encrypt(string content, string plainTextKey)
    {
        if (string.IsNullOrEmpty(content))
        {
            return string.Empty;
        }

        var key = Encoding.UTF8.GetBytes(plainTextKey);

        using var aesAlg = Aes.Create();

        using var encryptor = aesAlg.CreateEncryptor(key, aesAlg.IV);

        using var msEncrypt = new MemoryStream();

        using (var csEncrept = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
        {
            using var swEncrypt = new StreamWriter(csEncrept);

            swEncrypt.Write(content);
        }

        var iv = aesAlg.IV;
        var decryptedContent = msEncrypt.ToArray();
        var result = new byte[iv.Length + decryptedContent.Length];
        Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
        Buffer.BlockCopy(decryptedContent, 0, result, iv.Length, decryptedContent.Length);

        return Convert.ToBase64String(result);
    }

    public static string Decrypt(string content, string plainTextKey)
    {
        if (string.IsNullOrEmpty(content))
        {
            return string.Empty;
        }

        var fullCipher = Convert.FromBase64String(content);
        var iv = new byte[16];
        var cipher = new byte[fullCipher.Length - iv.Length];

        Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
        Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, fullCipher.Length - iv.Length);
        var key = Encoding.UTF8.GetBytes(plainTextKey);

        using var aesAlg = Aes.Create();

        using var decrptor = aesAlg.CreateDecryptor(key, iv);

        string result;

        using (var msDecrypt = new MemoryStream(cipher))
        {
            using var csDecrypt = new CryptoStream(msDecrypt, decrptor, CryptoStreamMode.Read);

            using var srDecrypt = new StreamReader(csDecrypt);

            result = srDecrypt.ReadToEnd();
        }

        return result;
    }

    public static string GetPlainMek(Db db)
    {
        var MekBase64 = (from p in db.T_SYS_PARAMS
            where p.PARAMETER_NAME == Constants.SysParamMek
            select p.PARAMETER_VALUE).First();

        return Encoding.ASCII.GetString(Convert.FromBase64String(MekBase64));
    }

    public static string BuildServicesApiTsk(Db db, Configurations config)
    {
        var right = (from p in db.T_SYS_PARAMS
            where p.PARAMETER_NAME == Constants.SysParamRServicesApiTsk
            select p.PARAMETER_VALUE).First();

        return $"{Encoding.ASCII.GetString(Convert.FromBase64String(config.LServicesApiTsk))}{Encoding.ASCII.GetString(Convert.FromBase64String(right))}"
            .Insert(11, "#")
            .Remove(50);
    }

    public static object Paging(int pageSize, int totalItems, object Data)
    {
        var TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);

        return new
        {
            Data,
            Paging = new { TotalItems = totalItems, TotalPages = TotalPages },
        };
    }
}