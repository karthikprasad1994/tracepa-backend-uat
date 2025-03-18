using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class StandardAuditChecklistDetail
{
    public int? SacdId { get; set; }

    public int? SacdCustId { get; set; }

    public int? SacdAuditId { get; set; }

    public int? SacdAuditType { get; set; }

    public string? SacdHeading { get; set; }

    public string? SacdCheckpointId { get; set; }

    public int? SacdEmpId { get; set; }

    public int? SacdWorkType { get; set; }

    public string? SacdHrPrDay { get; set; }

    public DateTime? SacdStartDate { get; set; }

    public DateTime? SacdEndDate { get; set; }

    public string? SacdTotalHr { get; set; }

    public DateTime? SacdCron { get; set; }

    public int? SacdCrby { get; set; }

    public int? SacdUpdatedby { get; set; }

    public DateTime? SacdUpdatedon { get; set; }

    public string? SacdIpaddress { get; set; }

    public int? SacdCompId { get; set; }
}
