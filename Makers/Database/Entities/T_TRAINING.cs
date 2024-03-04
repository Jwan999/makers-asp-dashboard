namespace Makers.Database.Entities;

public class T_TRAINING
{
    public int? ID { get; set; }
    public DateTime? INSDATE { get; set; }
    public DateTime? LUPDATE { get; set; }
    public string NAMEX { get; set; }
    public string TYPEX { get; set; }
    public DateTime? START_DATE { get; set; }
    public DateTime? END_DATE { get; set; }
    public string ATTENDANCE_TYPE { get; set; }
    public int? HOURSX { get; set; }
    public int? LEC_NUM { get; set; }
    public string TRAINING_DAYS { get; set; }
    public string IS_PAID { get; set; }
    public double? PRICEX { get; set; }
    public int? PROJECT { get; set; }
    public string IS_ACTIVE { get; set; }
    public string PROGRESS { get; set; }
}