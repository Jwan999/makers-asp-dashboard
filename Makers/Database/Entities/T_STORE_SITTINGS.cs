namespace Makers.Database.Entities;

public class T_STORE_SETTINGS
{
    public int? ID { get; set; }
    public DateTime? INSDATE { get; set; }
    public DateTime? LUPDATE { get; set; }
    public string PARAMETER_NAME { get; set; }
    public string PARAMETER_VALUE { get; set; }
    public string PARAMETER_DESCRIPTION { get; set; }
}