using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class ComplianceIssueTrackerDetailsHistory
{
    public int? CithPkid { get; set; }

    public int? CithCitpkid { get; set; }

    public int? CithCustomerId { get; set; }

    public int? CithComplianceCodeId { get; set; }

    public int? CithChecklistId { get; set; }

    public string? CithActionPlan { get; set; }

    public DateTime? CithTargetDate { get; set; }

    public string? CithIssueStatus { get; set; }

    public int? CithResponsibleFunctionId { get; set; }

    public int? CithFunctionManagerId { get; set; }

    public string? CithRemarks { get; set; }

    public int? CithCreatedBy { get; set; }

    public DateTime? CithCreatedOn { get; set; }

    public int? CithUpdatedBy { get; set; }

    public DateTime? CithUpdatedOn { get; set; }

    public string? CithIpaddress { get; set; }

    public int? CithCompId { get; set; }
}
