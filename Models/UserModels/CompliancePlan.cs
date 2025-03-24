using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Compliance_Plan")]
public partial class CompliancePlan
{
    [Column("CP_ID")]
    public int? CpId { get; set; }

    [Column("CP_YearID")]
    public int? CpYearId { get; set; }

    [Column("CP_CustomerID")]
    public int? CpCustomerId { get; set; }

    [Column("CP_ComplianceCode")]
    [StringLength(300)]
    [Unicode(false)]
    public string? CpComplianceCode { get; set; }

    [Column("CP_FunctionID")]
    public int? CpFunctionId { get; set; }

    [Column("CP_SubFunctionID")]
    public int? CpSubFunctionId { get; set; }

    [Column("CP_InherentRiskID")]
    public int? CpInherentRiskId { get; set; }

    [Column("CP_ReportTitle")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? CpReportTitle { get; set; }

    [Column("CP_ScheduledMonthID")]
    public int? CpScheduledMonthId { get; set; }

    [Column("CP_IsCurrentYear")]
    public int? CpIsCurrentYear { get; set; }

    [Column("CP_Remarks")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? CpRemarks { get; set; }

    [Column("CP_NetRatingID")]
    public int? CpNetRatingId { get; set; }

    [Column("CP_PlanStatus")]
    [StringLength(500)]
    [Unicode(false)]
    public string? CpPlanStatus { get; set; }

    [Column("CP_PlanCreatedBy")]
    public int? CpPlanCreatedBy { get; set; }

    [Column("CP_PlanCreatedOn", TypeName = "datetime")]
    public DateTime? CpPlanCreatedOn { get; set; }

    [Column("CP_PlanUpdatedBy")]
    public int? CpPlanUpdatedBy { get; set; }

    [Column("CP_PlanUpdatedOn", TypeName = "datetime")]
    public DateTime? CpPlanUpdatedOn { get; set; }

    [Column("CP_PlanSubmittedBy")]
    public int? CpPlanSubmittedBy { get; set; }

    [Column("CP_PlanSubmittedOn", TypeName = "datetime")]
    public DateTime? CpPlanSubmittedOn { get; set; }

    [Column("CP_ScheduleStatus")]
    [StringLength(500)]
    [Unicode(false)]
    public string? CpScheduleStatus { get; set; }

    [Column("CP_status")]
    [StringLength(100)]
    [Unicode(false)]
    public string? CpStatus { get; set; }

    [Column("CP_ComplianceStatus")]
    [StringLength(15)]
    [Unicode(false)]
    public string? CpComplianceStatus { get; set; }

    [Column("CP_ScheduleCreatedBy")]
    public int? CpScheduleCreatedBy { get; set; }

    [Column("CP_ScheduleCreatedOn", TypeName = "datetime")]
    public DateTime? CpScheduleCreatedOn { get; set; }

    [Column("CP_ScheduleUpdatedBy")]
    public int? CpScheduleUpdatedBy { get; set; }

    [Column("CP_ScheduleUpdatedOn", TypeName = "datetime")]
    public DateTime? CpScheduleUpdatedOn { get; set; }

    [Column("CP_ScheduleSubmittedBy")]
    public int? CpScheduleSubmittedBy { get; set; }

    [Column("CP_ScheduleSubmittedOn", TypeName = "datetime")]
    public DateTime? CpScheduleSubmittedOn { get; set; }

    [Column("CP_IPAddress")]
    [StringLength(100)]
    [Unicode(false)]
    public string? CpIpaddress { get; set; }

    [Column("CP_CompID")]
    public int? CpCompId { get; set; }
}
