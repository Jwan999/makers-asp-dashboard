﻿namespace Makers.Database.Entities;

public class T_CONTESTS
{
    public int? ID { get; set; }
    public DateTime? INSDATE { get; set; }
    public DateTime? LUPDATE { get; set; }
    public int? INST { get; set; }
    public string NAMEX { get; set; }
    public string OVERVIEW { get; set; }
    public DateTime? START_DATE { get; set; }
    public int? DURATION { get; set; }
    public string IS_ACTIVE { get; set; }
    public string TYPEX { get; set; }
}