namespace Makers.Utilities;

public class Constants
{
    public const string GlobalAdmin = "Global Admin";
    public const int GlobalAdminId = 1;
    public const int WebsiteUsersId = 3;
        
    public const long KB = 1024;
    public const long MB = 1048576;
    public const long GB = 1073741824;

    public const string appId = "479965cb-1564-40df-bd66-26216218da45";

    public const string AuthSchemeUsers = "AUTH_SCHEME_USERS";
    public const string AuthSchemeServicesApi = "AUTH_SCHEME_SERVICES_API";

    public const string JwtAudienceServicesClients = "MakersServicesClients";
    public const string JwtAudienceWebApplication = "MakersWebApplication";

    public const string ForbiddenTables = "T_USERS,T_CLAIMS,T_ROLES,T_LOGS,T_REPORTS,T_SYS_PARAMS,T_MAP_ROLES_CLAIMS,T_AUDIT,T_ROUTE,T_MAP_ROUTE_CLAIM";

    public const string IraqPhoneNumKey = "964";

    public const string SysParamPwCheckUppercase = "PASSWORD_CHECK_UPPERCASE";
    public const string SysParamPwCheckLowercase = "PASSWORD_CHECK_LOWERCASE";
    public const string SysParamPwCheckDigit = "PASSWORD_CHECK_DIGIT";
    public const string SysParamPwCheckMinLength = "PASSWORD_CHECK_MIN_LENGTH";
    public const string SysParamPwPreviousXAllowed = "PASSWORD_PREVIOUS_X_ALLOWED";
    public const string SysParamMek = "MEK";
    public const string SysParamUserMaxFailedLogins = "USER_MAX_FAILED_LOGINS";
    public const string SysParamOtpExpiration = "OTP_EXPIRATION";
    public const string SysParamOtpResendCount = "OTP_RESEND_COUNT";
    public const string SysParamOtpFailCount = "OTP_FAIL_COUNT";
    public const string SysParamRServicesApiTsk = "R_SERVICES_API_TSK";

    public const string CacheKeyRouteClaims = "ROUTE_CLAIMS";
    public const string CacheKeyUsersTsk = "USERS_TSK";
    public const string CacheKeyUsersJwtExpiration = "JWT_EXP";

    public const string AuditActionInsert = "INSERT";
    public const string AuditActionUpdate = "UPDATE";
    public const string AuditActionDelete = "DELETE";
    public const string AuditActionUpload = "UPLOAD";
    public const string AuditActionExecute = "EXECUTE";
    public const string AuditActionDownload = "DOWNLOAD";

    public const string Yes = "Y";
    public const string No = "N";

    public const string Admin = "ADMIN";
    public const string Customer = "CUSTOMER";

    public const string StatusNew = "NEW";
    public const string StatusClosed = "CLOSED";
    public const string BuyForMeStatusPending = "PENDING";
    public const string BuyForMeStatusAccepted = "ACCEPTED";
    public const string BuyForMeStatusRejected = "REJECTED";

    public const string ProductStatusPending = "PENDING";
    public const string ProductStatusReject = "REJECT";
    public const string ProductStatusaApprove = "APPROVE";
    public const string StatusPending = "PENDING";
    public const string StatusResolved = "RESOLVED";
    public const string TipsHowToBook = "How To Book";

    public const string ResponseUnauthorized = "Unauthorized";
}