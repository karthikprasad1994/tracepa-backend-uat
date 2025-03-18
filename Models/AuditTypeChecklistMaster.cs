using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class AuditTypeChecklistMaster
{
    public int? AcmId { get; set; }

    public string? AcmCode { get; set; }

    public int? AcmAuditTypeId { get; set; }

    public string? AcmHeading { get; set; }

    public string? AcmCheckpoint { get; set; }

    public string? AcmDelflg { get; set; }

    public DateTime? AcmCron { get; set; }

    public int? AcmCrby { get; set; }

    public int? AcmApprovedby { get; set; }

    public DateTime? AcmApprovedon { get; set; }

    public string? AcmStatus { get; set; }

    public int? AcmUpdatedby { get; set; }

    public DateTime? AcmUpdatedon { get; set; }

    public int? AcmDeletedby { get; set; }

    public DateTime? AcmDeletedon { get; set; }

    public int? AcmRecallby { get; set; }

    public DateTime? AcmRecallon { get; set; }

    public string? AcmIpaddress { get; set; }

    public int? AcmCompId { get; set; }

    public string? AcmAssertions { get; set; }
}
