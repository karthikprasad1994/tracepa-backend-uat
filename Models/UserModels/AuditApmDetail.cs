using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Audit_APM_Details")]
public partial class AuditApmDetail
{
    [Column("APM_ID")]
    public int? ApmId { get; set; }

    [Column("APM_YearID")]
    public int? ApmYearId { get; set; }

    [Column("APM_AuditCode")]
    [StringLength(50)]
    [Unicode(false)]
    public string? ApmAuditCode { get; set; }

    [Column("APM_CustID")]
    public int? ApmCustId { get; set; }

    [Column("APM_FunctionID")]
    public int? ApmFunctionId { get; set; }

    [Column("APM_AuditorsRoleID")]
    public int? ApmAuditorsRoleId { get; set; }

    [Column("APM_AuditTeamsID")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? ApmAuditTeamsId { get; set; }

    [Column("APM_BranchID")]
    public int? ApmBranchId { get; set; }

    [Column("APM_PartnersID")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? ApmPartnersId { get; set; }

    [Column("APM_TStartDate", TypeName = "datetime")]
    public DateTime? ApmTstartDate { get; set; }

    [Column("APM_TEndDate", TypeName = "datetime")]
    public DateTime? ApmTendDate { get; set; }

    [Column("APM_EstimatedEffortDays")]
    public int? ApmEstimatedEffortDays { get; set; }

    [Column("APM_Objectives")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? ApmObjectives { get; set; }

    [Column("APM_CustomerRemarks")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? ApmCustomerRemarks { get; set; }

    [Column("APM_AuditorsRemarks")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? ApmAuditorsRemarks { get; set; }

    [Column("APM_Audit_Confirm")]
    public int? ApmAuditConfirm { get; set; }

    [Column("APM_Audit_Confirm_Yes")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? ApmAuditConfirmYes { get; set; }

    [Column("APM_CrBy")]
    public int? ApmCrBy { get; set; }

    [Column("APM_CrOn", TypeName = "datetime")]
    public DateTime? ApmCrOn { get; set; }

    [Column("APM_UpdatedBy")]
    public int? ApmUpdatedBy { get; set; }

    [Column("APM_UpdatedOn", TypeName = "datetime")]
    public DateTime? ApmUpdatedOn { get; set; }

    [Column("APM_AppBy")]
    public int? ApmAppBy { get; set; }

    [Column("APM_AppOn", TypeName = "datetime")]
    public DateTime? ApmAppOn { get; set; }

    [Column("APM_Subject")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? ApmSubject { get; set; }

    [Column("APM_Body")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? ApmBody { get; set; }

    [Column("APM_TOEmail")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? ApmToemail { get; set; }

    [Column("APM_CCEmail")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? ApmCcemail { get; set; }

    [Column("APM_APMStatus")]
    [StringLength(50)]
    [Unicode(false)]
    public string? ApmApmstatus { get; set; }

    [Column("APM_APMCRStatus")]
    [StringLength(50)]
    [Unicode(false)]
    public string? ApmApmcrstatus { get; set; }

    [Column("APM_APMTAStatus")]
    [StringLength(50)]
    [Unicode(false)]
    public string? ApmApmtastatus { get; set; }

    [Column("APM_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? ApmIpaddress { get; set; }

    [Column("APM_AttachID")]
    public int? ApmAttachId { get; set; }

    [Column("APM_CompID")]
    public int? ApmCompId { get; set; }

    [Column("APM_StatusID")]
    public int? ApmStatusId { get; set; }

    [Column("APM_PGEDetailId")]
    public int? ApmPgedetailId { get; set; }
}
