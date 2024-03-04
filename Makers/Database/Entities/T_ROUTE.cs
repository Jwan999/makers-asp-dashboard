namespace Makers.Database.Entities;

public class T_ROUTE
{
    public int? ID { get; set; }
    public DateTime? INSDATE { get; set; }
    public DateTime? LUPDATE { get; set; }
    public string ROUTE_NAME { get; set; }
    public string HAS_CLAIMS { get; set; }
}