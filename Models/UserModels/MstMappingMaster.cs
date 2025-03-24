using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("MST_MAPPING_MASTER")]
public partial class MstMappingMaster
{
    [Column("MMM_ID")]
    public int? MmmId { get; set; }

    [Column("MMM_YearID")]
    public int? MmmYearId { get; set; }

    [Column("MMM_FunID")]
    public int? MmmFunId { get; set; }

    [Column("MMM_SEMID")]
    public int? MmmSemid { get; set; }

    [Column("MMM_PMID")]
    public int? MmmPmid { get; set; }

    [Column("MMM_SPMID")]
    public int? MmmSpmid { get; set; }

    [Column("MMM_RISKID")]
    public int? MmmRiskid { get; set; }

    [Column("MMM_Risk")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? MmmRisk { get; set; }

    [Column("MMM_ControlID")]
    public int? MmmControlId { get; set; }

    [Column("MMM_Control")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? MmmControl { get; set; }

    [Column("MMM_ChecksID")]
    public int? MmmChecksId { get; set; }

    [Column("MMM_CheckS")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? MmmCheckS { get; set; }

    [Column("MMM_InherentRiskID")]
    public int? MmmInherentRiskId { get; set; }

    [Column("MMM_InherentRisk")]
    [StringLength(100)]
    [Unicode(false)]
    public string? MmmInherentRisk { get; set; }

    [Column("MMM_DelFlag")]
    [StringLength(2)]
    [Unicode(false)]
    public string? MmmDelFlag { get; set; }

    [Column("MMM_Status")]
    [StringLength(2)]
    [Unicode(false)]
    public string? MmmStatus { get; set; }

    [Column("MMM_Module")]
    [StringLength(2)]
    [Unicode(false)]
    public string? MmmModule { get; set; }

    [Column("MMM_CrBy")]
    public int? MmmCrBy { get; set; }

    [Column("MMM_CrOn", TypeName = "datetime")]
    public DateTime? MmmCrOn { get; set; }

    [Column("MMM_UpdatedBy")]
    public int? MmmUpdatedBy { get; set; }

    [Column("MMM_UpdatedOn", TypeName = "datetime")]
    public DateTime? MmmUpdatedOn { get; set; }

    [Column("MMM_ApprovedBy")]
    public int? MmmApprovedBy { get; set; }

    [Column("MMM_ApprovedOn", TypeName = "datetime")]
    public DateTime? MmmApprovedOn { get; set; }

    [Column("MMM_DeletedBy")]
    public int? MmmDeletedBy { get; set; }

    [Column("MMM_DeletedOn", TypeName = "datetime")]
    public DateTime? MmmDeletedOn { get; set; }

    [Column("MMM_RecallBy")]
    public int? MmmRecallBy { get; set; }

    [Column("MMM_RecallOn", TypeName = "datetime")]
    public DateTime? MmmRecallOn { get; set; }

    [Column("MMM_IPaddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? MmmIpaddress { get; set; }

    [Column("MMM_CompID")]
    public int? MmmCompId { get; set; }

    [Column("MMM_SPMKey")]
    public int? MmmSpmkey { get; set; }

    [Column("MMM_RiskKey")]
    public int? MmmRiskKey { get; set; }

    [Column("MMM_ControlKey")]
    public int? MmmControlKey { get; set; }

    [Column("MMM_ChecksKey")]
    public int? MmmChecksKey { get; set; }

    [Column("MMM_CUSTID")]
    public int? MmmCustid { get; set; }
}
