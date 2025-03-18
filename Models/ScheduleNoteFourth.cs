using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class ScheduleNoteFourth
{
    public int SnftId { get; set; }

    public int? SnftCustId { get; set; }

    public string? SnftDescription { get; set; }

    public string? SnftCategory { get; set; }

    public decimal? SnftNumShares { get; set; }

    public decimal? SnftTotalShares { get; set; }

    public decimal? SnftChangedShares { get; set; }

    public int? SnftYearid { get; set; }

    public int? SnftCompId { get; set; }

    public string? SnftStatus { get; set; }

    public string? SnftDelflag { get; set; }

    public DateTime? SnftCron { get; set; }

    public int? SnftCrby { get; set; }

    public int? SnftUpdatedby { get; set; }

    public DateTime? SnftUpdatedon { get; set; }

    public string? SnftIpaddress { get; set; }
}
