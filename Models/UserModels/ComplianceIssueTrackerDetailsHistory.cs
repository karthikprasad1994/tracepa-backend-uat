using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Compliance_IssueTracker_details_History")]
public partial class ComplianceIssueTrackerDetailsHistory
{
    [Column("CITH_PKID")]
    public int? CithPkid { get; set; }

    [Column("CITH_CITPKID")]
    public int? CithCitpkid { get; set; }

    [Column("CITH_CustomerID")]
    public int? CithCustomerId { get; set; }

    [Column("CITH_ComplianceCodeID")]
    public int? CithComplianceCodeId { get; set; }

    [Column("CITH_ChecklistID")]
    public int? CithChecklistId { get; set; }

    [Column("CITH_ActionPlan")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? CithActionPlan { get; set; }

    [Column("CITH_TargetDate", TypeName = "datetime")]
    public DateTime? CithTargetDate { get; set; }

    [Column("CITH_IssueStatus")]
    [StringLength(5)]
    [Unicode(false)]
    public string? CithIssueStatus { get; set; }

    [Column("CITH_ResponsibleFunctionID")]
    public int? CithResponsibleFunctionId { get; set; }

    [Column("CITH_FunctionManagerID")]
    public int? CithFunctionManagerId { get; set; }

    [Column("CITH_Remarks")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? CithRemarks { get; set; }

    [Column("CITH_CreatedBy")]
    public int? CithCreatedBy { get; set; }

    [Column("CITH_CreatedOn", TypeName = "datetime")]
    public DateTime? CithCreatedOn { get; set; }

    [Column("CITH_UpdatedBy")]
    public int? CithUpdatedBy { get; set; }

    [Column("CITH_UpdatedOn", TypeName = "datetime")]
    public DateTime? CithUpdatedOn { get; set; }

    [Column("CITH_IPAddress")]
    [StringLength(100)]
    [Unicode(false)]
    public string? CithIpaddress { get; set; }

    [Column("CITH_CompID")]
    public int? CithCompId { get; set; }
}
