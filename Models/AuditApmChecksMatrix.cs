using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class AuditApmChecksMatrix
{
    public int? ApmcmPkid { get; set; }

    public int? ApmcmApmpkid { get; set; }

    public int? ApmcmYearId { get; set; }

    public int? ApmcmCustId { get; set; }

    public int? ApmcmFunctionId { get; set; }

    public int? ApmcmSubFunctionId { get; set; }

    public int? ApmcmProcessId { get; set; }

    public int? ApmcmSubProcessId { get; set; }

    public int? ApmcmRiskId { get; set; }

    public int? ApmcmControlId { get; set; }

    public int? ApmcmChecksId { get; set; }

    public int? ApmcmMmmid { get; set; }

    public string? ApmcmStatus { get; set; }

    public string? ApmcmIpaddress { get; set; }

    public int? ApmcmCompId { get; set; }
}
