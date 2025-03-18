using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class CompliancePlan
{
    public int? CpId { get; set; }

    public int? CpYearId { get; set; }

    public int? CpCustomerId { get; set; }

    public string? CpComplianceCode { get; set; }

    public int? CpFunctionId { get; set; }

    public int? CpSubFunctionId { get; set; }

    public int? CpInherentRiskId { get; set; }

    public string? CpReportTitle { get; set; }

    public int? CpScheduledMonthId { get; set; }

    public int? CpIsCurrentYear { get; set; }

    public string? CpRemarks { get; set; }

    public int? CpNetRatingId { get; set; }

    public string? CpPlanStatus { get; set; }

    public int? CpPlanCreatedBy { get; set; }

    public DateTime? CpPlanCreatedOn { get; set; }

    public int? CpPlanUpdatedBy { get; set; }

    public DateTime? CpPlanUpdatedOn { get; set; }

    public int? CpPlanSubmittedBy { get; set; }

    public DateTime? CpPlanSubmittedOn { get; set; }

    public string? CpScheduleStatus { get; set; }

    public string? CpStatus { get; set; }

    public string? CpComplianceStatus { get; set; }

    public int? CpScheduleCreatedBy { get; set; }

    public DateTime? CpScheduleCreatedOn { get; set; }

    public int? CpScheduleUpdatedBy { get; set; }

    public DateTime? CpScheduleUpdatedOn { get; set; }

    public int? CpScheduleSubmittedBy { get; set; }

    public DateTime? CpScheduleSubmittedOn { get; set; }

    public string? CpIpaddress { get; set; }

    public int? CpCompId { get; set; }
}
