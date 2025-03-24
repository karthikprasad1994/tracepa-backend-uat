using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Risk_BRRChecklist_Details")]
public partial class RiskBrrchecklistDetail
{
    [Column("BRRD_PKID")]
    public int? BrrdPkid { get; set; }

    [Column("BRRD_BRRPKID")]
    public int? BrrdBrrpkid { get; set; }

    [Column("BRRD_RCMID")]
    public int? BrrdRcmid { get; set; }

    [Column("BRRD_FunctionID")]
    public int? BrrdFunctionId { get; set; }

    [Column("BRRD_AreaID")]
    public int? BrrdAreaId { get; set; }

    [Column("BRRD_RefNo")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? BrrdRefNo { get; set; }

    [Column("BRRD_CheckPoint")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? BrrdCheckPoint { get; set; }

    [Column("BRRD_MethodologyID")]
    public int? BrrdMethodologyId { get; set; }

    [Column("BRRD_Methodology")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? BrrdMethodology { get; set; }

    [Column("BRRD_SampleSizeID")]
    public int? BrrdSampleSizeId { get; set; }

    [Column("BRRD_SampleSizeName")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? BrrdSampleSizeName { get; set; }

    [Column("BRRD_RiskCategory")]
    [StringLength(500)]
    [Unicode(false)]
    public string? BrrdRiskCategory { get; set; }

    [Column("BRRD_YESNONA")]
    public int? BrrdYesnona { get; set; }

    [Column("BRRD_Status")]
    [StringLength(2)]
    [Unicode(false)]
    public string? BrrdStatus { get; set; }

    [Column("BRRD_DelFlag")]
    [StringLength(1)]
    [Unicode(false)]
    public string? BrrdDelFlag { get; set; }

    [Column("BRRD_IssueDetails")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? BrrdIssueDetails { get; set; }

    [Column("BRRD_OWeightage")]
    public double? BrrdOweightage { get; set; }

    [Column("BRRD_RiskScore")]
    public int? BrrdRiskScore { get; set; }

    [Column("BRRD_WeightedRiskScore")]
    public double? BrrdWeightedRiskScore { get; set; }

    [Column("BRRD_Annexure")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? BrrdAnnexure { get; set; }

    [Column("BRRD_FunType")]
    [StringLength(1)]
    [Unicode(false)]
    public string? BrrdFunType { get; set; }

    [Column("BRRD_AttachID")]
    public int? BrrdAttachId { get; set; }

    [Column("BRRD_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? BrrdIpaddress { get; set; }

    [Column("BRRD_CompID")]
    public int? BrrdCompId { get; set; }
}
