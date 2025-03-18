using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class ScheduleNoteSecond
{
    public int SnsId { get; set; }

    public int? SnsCustId { get; set; }

    public string? SnsDescription { get; set; }

    public string? SnsCategory { get; set; }

    public decimal? SnsCyearBegShares { get; set; }

    public decimal? SnsCyearBegAmount { get; set; }

    public decimal? SnsPyearBegShares { get; set; }

    public decimal? SnsPyearBegAmount { get; set; }

    public decimal? SnsCyearAddShares { get; set; }

    public decimal? SnsCyearAddAmount { get; set; }

    public decimal? SnsPyearAddShares { get; set; }

    public decimal? SnsPyearAddAmount { get; set; }

    public decimal? SnsCyearEndShares { get; set; }

    public decimal? SnsCyearEndAmount { get; set; }

    public decimal? SnsPyearEndShares { get; set; }

    public decimal? SnsPyearEndAmount { get; set; }

    public int? SnsYearid { get; set; }

    public int? SnsCompId { get; set; }

    public string? SnsStatus { get; set; }

    public string? SnsDelflag { get; set; }

    public DateTime? SnsCron { get; set; }

    public int? SnsCrby { get; set; }

    public int? SnsUpdatedby { get; set; }

    public DateTime? SnsUpdatedon { get; set; }

    public string? SnsIpaddress { get; set; }
}
