namespace Makers.Database.Entities;

public class T_SYS_PARAMS
{
    public int? ID { get; set; }
    public DateTime? INSDATE { get; set; }
    public DateTime? LUPDATE { get; set; }
    public string PARAMETER_NAME { get; set; }
    public string PARAMETER_VALUE { get; set; }
    public string PARAMETER_DESCRIPTION { get; set; }
    public char? IS_SECRET { get; set; }
    public string IS_ACTIVE { get; set; }
}