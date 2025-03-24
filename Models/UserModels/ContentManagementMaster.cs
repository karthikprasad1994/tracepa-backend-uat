using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Content_Management_Master")]
public partial class ContentManagementMaster
{
    [Column("cmm_ID")]
    public int CmmId { get; set; }

    [Column("cmm_Code")]
    [StringLength(50)]
    [Unicode(false)]
    public string CmmCode { get; set; } = null!;

    [Column("cmm_Desc")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? CmmDesc { get; set; }

    [Column("cmm_Category")]
    [StringLength(5)]
    [Unicode(false)]
    public string? CmmCategory { get; set; }

    [Column("cms_Remarks")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? CmsRemarks { get; set; }

    [Column("cms_KeyComponent")]
    public int? CmsKeyComponent { get; set; }

    [Column("cms_Module")]
    [StringLength(1)]
    [Unicode(false)]
    public string? CmsModule { get; set; }

    [Column("cmm_Delflag")]
    [StringLength(1)]
    [Unicode(false)]
    public string CmmDelflag { get; set; } = null!;

    [Column("CMM_Status")]
    [StringLength(2)]
    [Unicode(false)]
    public string? CmmStatus { get; set; }

    [Column("CMM_UpdatedBy")]
    public int? CmmUpdatedBy { get; set; }

    [Column("CMM_UpdatedOn", TypeName = "datetime")]
    public DateTime? CmmUpdatedOn { get; set; }

    [Column("CMM_ApprovedBy")]
    public int? CmmApprovedBy { get; set; }

    [Column("CMM_ApprovedOn", TypeName = "datetime")]
    public DateTime? CmmApprovedOn { get; set; }

    [Column("CMM_DeletedBy")]
    public int? CmmDeletedBy { get; set; }

    [Column("CMM_DeletedOn", TypeName = "datetime")]
    public DateTime? CmmDeletedOn { get; set; }

    [Column("CMM_RecallBy")]
    public int? CmmRecallBy { get; set; }

    [Column("CMM_RecallOn", TypeName = "datetime")]
    public DateTime? CmmRecallOn { get; set; }

    [Column("CMM_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? CmmIpaddress { get; set; }

    [Column("CMM_CompID")]
    public int? CmmCompId { get; set; }

    [Column("CMM_RiskCategory")]
    public int? CmmRiskCategory { get; set; }

    [Column("CMM_CrBy")]
    public int? CmmCrBy { get; set; }

    [Column("CMM_CrOn", TypeName = "datetime")]
    public DateTime? CmmCrOn { get; set; }

    [Column("cmm_Rate", TypeName = "money")]
    public decimal? CmmRate { get; set; }

    [Column("CMM_Act")]
    [StringLength(50)]
    [Unicode(false)]
    public string? CmmAct { get; set; }

    [Column("CMM_HSNSAC")]
    [StringLength(100)]
    [Unicode(false)]
    public string? CmmHsnsac { get; set; }
}
