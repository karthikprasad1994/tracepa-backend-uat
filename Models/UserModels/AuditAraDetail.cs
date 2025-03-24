using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Audit_ARA_Details")]
public partial class AuditAraDetail
{
    [Column("ARAD_PKID")]
    public int? AradPkid { get; set; }

    [Column("ARAD_ARAPKID")]
    public int? AradArapkid { get; set; }

    [Column("ARAD_SEMID")]
    public int? AradSemid { get; set; }

    [Column("ARAD_PMID")]
    public int? AradPmid { get; set; }

    [Column("ARAD_SPMID")]
    public int? AradSpmid { get; set; }

    [Column("ARAD_IssueHeading")]
    [StringLength(50)]
    [Unicode(false)]
    public string? AradIssueHeading { get; set; }

    [Column("ARAD_RiskID")]
    public int? AradRiskId { get; set; }

    [Column("ARAD_RiskTypeID")]
    public int? AradRiskTypeId { get; set; }

    [Column("ARAD_ImpactID")]
    public int? AradImpactId { get; set; }

    [Column("ARAD_LikelihoodID")]
    public int? AradLikelihoodId { get; set; }

    [Column("ARAD_RiskRating")]
    public int? AradRiskRating { get; set; }

    [Column("ARAD_ControlID")]
    public int? AradControlId { get; set; }

    [Column("ARAD_OES")]
    public int? AradOes { get; set; }

    [Column("ARAD_DES")]
    public int? AradDes { get; set; }

    [Column("ARAD_ControlRating")]
    public int? AradControlRating { get; set; }

    [Column("ARAD_ChecksID")]
    public int? AradChecksId { get; set; }

    [Column("ARAD_ResidualRiskRating")]
    public int? AradResidualRiskRating { get; set; }

    [Column("ARAD_Remarks")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? AradRemarks { get; set; }

    [Column("ARAD_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? AradIpaddress { get; set; }

    [Column("ARAD_CompID")]
    public int? AradCompId { get; set; }
}
