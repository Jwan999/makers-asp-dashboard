using System.ComponentModel.DataAnnotations.Schema;

namespace Makers.Database.Entities;

public class T_REPORTS
{
    public int? ID { get; set; }
    public DateTime? INSDATE { get; set; }
    public DateTime? LUPDATE { get; set; }
    public string REPORT_NAME { get; set; }
    public string REPORT_DESC { get; set; }

    [Column(TypeName = "CLOB")]
    public string QUERYX { get; set; }

    public string REPORT_CAT { get; set; }
    public DateTime? LAST_EXEC { get; set; }
}