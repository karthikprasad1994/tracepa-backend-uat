using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class NotificationMaster
{
    public int? NmId { get; set; }

    public string? NmCode { get; set; }

    public string NmName { get; set; } = null!;

    public string? NmDesc { get; set; }

    public int? NmFrequency { get; set; }

    public DateTime? NmDuedate { get; set; }

    public int? NmPosition { get; set; }

    public string? NmDelflg { get; set; }

    public DateTime? NmCron { get; set; }

    public int? NmCrby { get; set; }

    public int? NmApprovedby { get; set; }

    public DateTime? NmApprovedon { get; set; }

    public string? NmStatus { get; set; }

    public int? NmUpdatedby { get; set; }

    public DateTime? NmUpdatedon { get; set; }

    public int? NmDeletedby { get; set; }

    public DateTime? NmDeletedon { get; set; }

    public int? NmRecallby { get; set; }

    public DateTime? NmRecallon { get; set; }

    public string? NmIpaddress { get; set; }

    public int? NmCompId { get; set; }

    public int? NmNoOfDays { get; set; }
}
