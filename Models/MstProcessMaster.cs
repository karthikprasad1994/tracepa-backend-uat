using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class MstProcessMaster
{
    public int? PmId { get; set; }

    public int? PmEntId { get; set; }

    public int? PmSemId { get; set; }

    public string? PmCode { get; set; }

    public string? PmName { get; set; }

    public string? PmDesc { get; set; }

    public string? PmDelflg { get; set; }

    public string? PmStatus { get; set; }

    public int? PmCrby { get; set; }

    public DateTime? PmCron { get; set; }

    public int? PmUpdatedby { get; set; }

    public DateTime? PmUpdatedon { get; set; }

    public int? PmDeletedby { get; set; }

    public DateTime? PmDeletedon { get; set; }

    public int? PmApprovedby { get; set; }

    public DateTime? PmApprovedon { get; set; }

    public int? PmRecallby { get; set; }

    public DateTime? PmRecallon { get; set; }

    public string? PmIpaddress { get; set; }

    public int? PmCompId { get; set; }
}
