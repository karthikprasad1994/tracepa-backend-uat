using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Risk_RCSA_Details")]
public partial class RiskRcsaDetail
{
    [Column("RCSAD_PKID")]
    public int? RcsadPkid { get; set; }

    [Column("RCSAD_RCSAPKID")]
    public int? RcsadRcsapkid { get; set; }

    [Column("RCSAD_SEMID")]
    public int? RcsadSemid { get; set; }

    [Column("RCSAD_PMID")]
    public int? RcsadPmid { get; set; }

    [Column("RCSAD_SPMID")]
    public int? RcsadSpmid { get; set; }

    [Column("RCSAD_RiskID")]
    public int? RcsadRiskId { get; set; }

    [Column("RCSAD_RiskTypeID")]
    public int? RcsadRiskTypeId { get; set; }

    [Column("RCSAD_ImpactID")]
    public int? RcsadImpactId { get; set; }

    [Column("RCSAD_LikelihoodID")]
    public int? RcsadLikelihoodId { get; set; }

    [Column("RCSAD_RiskRating")]
    public int? RcsadRiskRating { get; set; }

    [Column("RCSAD_ControlID")]
    public int? RcsadControlId { get; set; }

    [Column("RCSAD_OES")]
    public int? RcsadOes { get; set; }

    [Column("RCSAD_DES")]
    public int? RcsadDes { get; set; }

    [Column("RCSAD_ControlRating")]
    public int? RcsadControlRating { get; set; }

    [Column("RCSAD_ChecksID")]
    public int? RcsadChecksId { get; set; }

    [Column("RCSAD_ResidualRiskRating")]
    public int? RcsadResidualRiskRating { get; set; }

    [Column("RCSAD_Remarks")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? RcsadRemarks { get; set; }

    [Column("RCSAD_RemarksRT")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? RcsadRemarksRt { get; set; }

    [Column("RCSAD_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? RcsadIpaddress { get; set; }

    [Column("RCSAD_CompID")]
    public int? RcsadCompId { get; set; }
}
