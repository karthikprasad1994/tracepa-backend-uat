using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("AuditAssignment_Schedule")]
public partial class AuditAssignmentSchedule
{
    [Column("AAS_ID")]
    public int? AasId { get; set; }

    [Column("AAS_AssignmentNo")]
    [StringLength(50)]
    [Unicode(false)]
    public string? AasAssignmentNo { get; set; }

    [Column("AAS_CustID")]
    public int? AasCustId { get; set; }

    [Column("AAS_PartnerID")]
    public int? AasPartnerId { get; set; }

    [Column("AAS_YearID")]
    public int? AasYearId { get; set; }

    [Column("AAS_MonthID")]
    public int? AasMonthId { get; set; }

    [Column("AAS_TaskID")]
    public int? AasTaskId { get; set; }

    [Column("AAS_Status")]
    public int? AasStatus { get; set; }

    [Column("AAS_AttachID")]
    public int? AasAttachId { get; set; }

    [Column("AAS_CrBy")]
    public int? AasCrBy { get; set; }

    [Column("AAS_CrOn", TypeName = "datetime")]
    public DateTime? AasCrOn { get; set; }

    [Column("AAS_UpdatedBy")]
    public int? AasUpdatedBy { get; set; }

    [Column("AAS_UpdatedOn", TypeName = "datetime")]
    public DateTime? AasUpdatedOn { get; set; }

    [Column("AAS_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? AasIpaddress { get; set; }

    [Column("AAS_CompID")]
    public int? AasCompId { get; set; }

    [Column("AAS_AdvancePartialBilling")]
    public int? AasAdvancePartialBilling { get; set; }

    [Column("AAS_BillingType")]
    public int? AasBillingType { get; set; }

    [Column("AAS_AssessmentYearID")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? AasAssessmentYearId { get; set; }

    [Column("AAS_IsComplianceAsg")]
    public int? AasIsComplianceAsg { get; set; }

    [Column("AAS_FolderPath")]
    [StringLength(500)]
    [Unicode(false)]
    public string? AasFolderPath { get; set; }
}
