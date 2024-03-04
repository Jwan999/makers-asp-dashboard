namespace Makers.Database.Entities;

public class T_EVENTS
{
    public int? ID { get; set; }
    public DateTime? INSDATE { get; set; }
    public DateTime? LUPDATE { get; set; }
    public int? PROJ_ID { get; set; }
    public string NAMEX { get; set; }
    public string OVERVIEW { get; set; }
    public DateTime? DATEX { get; set; }
    public int? DURATION { get; set; }
    public int? PARTICIPANTS_NUM { get; set; }
    public string LOCATIONX { get; set; }
    public string IS_ACTIVE { get; set; }
    public string LINKX { get; set; }
}