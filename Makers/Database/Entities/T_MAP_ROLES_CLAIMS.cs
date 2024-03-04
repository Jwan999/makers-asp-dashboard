namespace Makers.Database.Entities;

public class T_MAP_ROLES_CLAIMS
{
    public int? ID { get; set; }
    public DateTime? INSDATE { get; set; }
    public DateTime? LUPDATE { get; set; }
    public int? ROLE_ID { get; set; }
    public string CLAIM_VALUE { get; set; }
}