public class T_USERS
{
    public int? ID { get; set; }
    public DateTime? INSDATE { get; set; }
    public DateTime? LUPDATE { get; set; }
    public string USERNAME { get; set; }
    public string PASSWORD { get; set; }
    public string SALT { get; set; }
    public string FULL_NAME { get; set; }
    public string PHONE_NUMBER { get; set; }
    public string EMAIL { get; set; }
    public string USER_ROLE { get; set; }
    public string GENDER { get; set; }
    public DateTime? BDATE { get; set; }
    public int? ROLE_ID_FK { get; set; }
    public string OTP { get; set; }
    public int? FAILED_OTP_COUNTER { get; set; }
    public DateTime? OTP_EXP_DATE { get; set; }
    public string DEVICE_TOKEN { get; set; }
    public string DEVICE_OS { get; set; }
    public string DEVICE_MODEL { get; set; }
    public DateTime? LAST_LOGIN_DATE { get; set; }
    public int? FAILED_LOGIN_COUNTER { get; set; }
    public string IS_BLOCKED { get; set; }
    public DateTime? BLOCK_DATE { get; set; }
    public int? REGISTERED_BY { get; set; }
}