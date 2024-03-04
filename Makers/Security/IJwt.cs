namespace Makers.Security;

public interface IJwt
{
    int UserId { get; set; }
    string USER_NAME { get; set; }
    int RoleId { get; set; }
    string LastLoginDate { get; set; }
}