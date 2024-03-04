namespace Makers.Database.Entities;

public class T_AUDIT
{
    public int? ID { get; set; }
    public int? USER_ID { get; set; }
    public string USER_TYPE { get; set; }
    public DateTime AUDIT_DATE { get; set; }
    public string ACTION_TYPE { get; set; }
    public int? REF_ID { get; set; }
    public string REF_TYPE { get; set; }
    public string REF_DESC { get; set; }
    public string REF_OBJECT { get; set; }
}