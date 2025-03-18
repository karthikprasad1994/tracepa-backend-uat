using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class MstSubentityMaster
{
    public int? SemId { get; set; }

    public int? SemEntId { get; set; }

    public string? SemCode { get; set; }

    public string? SemName { get; set; }

    public string? SemDesc { get; set; }

    public string? SemDelflg { get; set; }

    public string? SemStatus { get; set; }

    public DateTime? SemCron { get; set; }

    public int? SemCrby { get; set; }

    public int? SemUpdatedby { get; set; }

    public DateTime? SemUpdatedon { get; set; }

    public int? SemDeletedby { get; set; }

    public DateTime? SemDeletedon { get; set; }

    public int? SemApprovedby { get; set; }

    public DateTime? SemApprovedon { get; set; }

    public int? SemRecallby { get; set; }

    public DateTime? SemRecallon { get; set; }

    public string? SemIpaddress { get; set; }

    public int? SemCompId { get; set; }
}
