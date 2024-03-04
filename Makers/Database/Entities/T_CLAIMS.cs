namespace Makers.Database.Entities;

public class T_CLAIMS
{
    public int? ID { get; set; }
    public DateTime? INSERT_DATE { get; set; }
    public DateTime? LAST_UPDATE { get; set; }
    public string CLAIM_NAME { get; set; }
    public string CLAIM_VALUE { get; set; }
    public string GROUP_ID { get; set; }
}