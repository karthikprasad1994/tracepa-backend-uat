using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class CrpaProcess
{
    public int CapId { get; set; }

    public string CapCode { get; set; } = null!;

    public string? CapProcessname { get; set; }

    public int? CapPoints { get; set; }

    public int? CapSectionid { get; set; }

    public int? CapSubsectionid { get; set; }

    public string? CapDesc { get; set; }

    public string CapDelflg { get; set; } = null!;

    public DateTime? CapCron { get; set; }

    public int? CapCrby { get; set; }

    public int? CapApprovedby { get; set; }

    public DateTime? CapApprovedon { get; set; }

    public string? CapStatus { get; set; }

    public int? CapUpdatedby { get; set; }

    public DateTime? CapUpdatedon { get; set; }

    public int? CapDeletedby { get; set; }

    public DateTime? CapDeletedon { get; set; }

    public int? CapRecallby { get; set; }

    public DateTime? CapRecallon { get; set; }

    public string? CapIpaddress { get; set; }

    public int? CapCompId { get; set; }

    public int? CapYearid { get; set; }
}
