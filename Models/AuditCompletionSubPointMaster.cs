using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class AuditCompletionSubPointMaster
{
    public int? AsmId { get; set; }

    public string? AsmCode { get; set; }

    public int? AsmCheckpointId { get; set; }

    public string? AsmSubPoint { get; set; }

    public string? AsmRemarks { get; set; }

    public string? AsmDelflg { get; set; }

    public DateTime? AsmCron { get; set; }

    public int? AsmCrby { get; set; }

    public int? AsmApprovedby { get; set; }

    public DateTime? AsmApprovedon { get; set; }

    public string? AsmStatus { get; set; }

    public int? AsmUpdatedby { get; set; }

    public DateTime? AsmUpdatedon { get; set; }

    public int? AsmDeletedby { get; set; }

    public DateTime? AsmDeletedon { get; set; }

    public int? AsmRecallby { get; set; }

    public DateTime? AsmRecallon { get; set; }

    public string? AsmIpaddress { get; set; }

    public int? AsmCompId { get; set; }
}
