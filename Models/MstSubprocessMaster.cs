using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class MstSubprocessMaster
{
    public int? SpmId { get; set; }

    public int? SpmEntId { get; set; }

    public int? SpmSemId { get; set; }

    public int? SpmPmId { get; set; }

    public string? SpmCode { get; set; }

    public string? SpmName { get; set; }

    public string? SpmDesc { get; set; }

    public int? SpmIsKey { get; set; }

    public string? SpmDelflg { get; set; }

    public string? SpmStatus { get; set; }

    public int? SpmCrby { get; set; }

    public DateTime? SpmCron { get; set; }

    public int? SpmUpdatedby { get; set; }

    public DateTime? SpmUpdatedon { get; set; }

    public int? SpmDeletedby { get; set; }

    public DateTime? SpmDeletedon { get; set; }

    public int? SpmApprovedby { get; set; }

    public DateTime? SpmApprovedon { get; set; }

    public int? SpmRecallby { get; set; }

    public DateTime? SpmRecallon { get; set; }

    public string? SpmIpaddress { get; set; }

    public int? SpmCompId { get; set; }
}
