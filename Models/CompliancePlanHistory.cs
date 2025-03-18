using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class CompliancePlanHistory
{
    public int? CphId { get; set; }

    public int? CphYearId { get; set; }

    public int? CphCpid { get; set; }

    public int? CphCustomerId { get; set; }

    public int? CphFunctionId { get; set; }

    public int? CphSubFunctionId { get; set; }

    public int? CphScheduledMonthId { get; set; }

    public int? CphIsCurrentYear { get; set; }

    public string? CphStatus { get; set; }

    public string? CphRemarks { get; set; }

    public int? CphCreatedBy { get; set; }

    public DateTime? CphCreatedOn { get; set; }

    public string? CphIpaddress { get; set; }

    public int? CphCompId { get; set; }
}
