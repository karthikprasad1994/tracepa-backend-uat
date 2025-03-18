using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class ScheduleNoteThird
{
    public int SntId { get; set; }

    public int? SntCustId { get; set; }

    public string? SntDescription { get; set; }

    public string? SntCategory { get; set; }

    public decimal? SntCyearShares { get; set; }

    public decimal? SntCyearAmount { get; set; }

    public decimal? SntPyearShares { get; set; }

    public decimal? SntPyearAmount { get; set; }

    public int? SntYearid { get; set; }

    public int? SntCompId { get; set; }

    public string? SntStatus { get; set; }

    public string? SntDelflag { get; set; }

    public DateTime? SntCron { get; set; }

    public int? SntCrby { get; set; }

    public int? SntUpdatedby { get; set; }

    public DateTime? SntUpdatedon { get; set; }

    public string? SntIpaddress { get; set; }
}
