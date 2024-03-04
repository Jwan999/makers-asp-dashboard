using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Security.Claims;

namespace Makers.Utilities;

public static class Extensions
{
    public static void LogMsg(this ILogger logger, string msg)
    {
        logger.LogWarning(msg);
    }

    public static parameterType GetParameter<parameterType>(this JObject reqBody, string parameterName)
    {
        try
        {
            var parameterValue = reqBody.GetValue(parameterName).Value<parameterType>();

            if (string.IsNullOrWhiteSpace(parameterValue.ToString()))
            {
                throw new Exception($"Invalid parameter {parameterName}");
            }

            if (parameterValue is string)
            {
                return (parameterType)Convert.ChangeType(parameterValue.ToString().Trim(), typeof(parameterType));
            }

            return parameterValue;
        }

        catch (Exception)
        {
            throw new Exception($"Invalid parameter {parameterName}");
        }
    }

    public static IActionResult Response(this Controller controller, string StatusDescription, object ResponseData, StatusCodes StatusCode = StatusCodes.Success)
    {
        return controller.Json(new { StatusCode = (int)StatusCode, StatusDescription, ResponseData });
    }

    public static bool VerifyAndCorrectPhone(this string phone, out string correctedPhone)
    {
        correctedPhone = string.Empty;

        try
        {
            string rawPhone = phone[phone.IndexOf('7')..];

            if (rawPhone.Length != 10 || rawPhone is null)
            {
                throw new Exception("Invalid Phone Number!");
            }

            correctedPhone = Constants.IraqPhoneNumKey + rawPhone;
            return true;

        }

        catch (ArgumentOutOfRangeException)
        {
            return false;
        }
    }

    private static Dictionary<string, object> SerializeRow(IEnumerable<string> cols, IDataReader reader)
    {
        var result = new Dictionary<string, object>();

        foreach (var col in cols)
        {
            result.Add(col, reader[col]);
        }

        return result;
    }

    public static string ExtractErrorMessage(this string errorMsg)
    {
        return errorMsg.Substring(errorMsg.IndexOf("<Makers_MSG_STR>") + 15, errorMsg.IndexOf("<Makers_MSG_END>") - errorMsg.IndexOf("<Makers_MSG_STR>") - 15);
    }

    public static int? ParseStringToNullableInt(this string s)
    {
        if (string.IsNullOrWhiteSpace(s) || s == "null")
        {
            return null;
        }

        else
        {
            return int.Parse(s);
        }
    }

    public static double? ParseStringToNullableDouble(this string s)
    {
        if (string.IsNullOrWhiteSpace(s))
        {
            return null;
        }

        else
        {
            return double.Parse(s);
        }
    }
    public static byte[] Base64Decode(this string s)
    {
        var paddingLength = 4 - s.Length % 4;

        if (paddingLength < 4)
        {
            s += new string('=', paddingLength);
        }

        return Convert.FromBase64String(s);
    }

    public static int GetUserId(this IHttpContextAccessor _httpContextAccessor)
    {
        try
        {
            if (_httpContextAccessor.HttpContext == null || _httpContextAccessor.HttpContext.RequestAborted.IsCancellationRequested)
                throw new Exception("Request Aborted");
            var id = int.TryParse(_httpContextAccessor.HttpContext.User.FindFirstValue("117115101114105100"), out int userId)
                ? userId
                : 0;
            return id;
        }
        catch { return 0; }
    }

    public static string CalculateExecutionTime(this DateTime end, DateTime start)
    {
        string message;

        var totalElapsedTime = end - start;

        var elapsedTimeInMinutes = totalElapsedTime.ToString("%m\\:ss");
        var elapsedTimeInSeconds = int.Parse(totalElapsedTime.ToString("ss"));

        if (totalElapsedTime.Minutes >= 1)
        {
            message = $"{elapsedTimeInMinutes} minute(s)";
        }

        else
        {
            message = $"{elapsedTimeInSeconds} second(s)";
        }

        return message;
    }
}

