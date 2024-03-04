namespace Makers.Database.Entities;

public class T_MAP_ROUTE_CLAIM
{
    public int? ID { get; set; }
    public DateTime INSDATE { get; set; }
    public DateTime LUPDATE { get; set; }
    public int? ROUTE_ID { get; set; }
    public int? CLAIM_ID { get; set; }
}