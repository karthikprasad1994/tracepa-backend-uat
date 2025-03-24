using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Risk_RA_Details")]
public partial class RiskRaDetail
{
    [Column("RAD_PKID")]
    public int? RadPkid { get; set; }

    [Column("RAD_RAPKID")]
    public int? RadRapkid { get; set; }

    [Column("RAD_SEMID")]
    public int? RadSemid { get; set; }

    [Column("RAD_PMID")]
    public int? RadPmid { get; set; }

    [Column("RAD_SPMID")]
    public int? RadSpmid { get; set; }

    [Column("RAD_RiskID")]
    public int? RadRiskId { get; set; }

    [Column("RAD_RiskTypeID")]
    public int? RadRiskTypeId { get; set; }

    [Column("RAD_ImpactID")]
    public int? RadImpactId { get; set; }

    [Column("RAD_LikelihoodID")]
    public int? RadLikelihoodId { get; set; }

    [Column("RAD_RiskRating")]
    public int? RadRiskRating { get; set; }

    [Column("RAD_ControlID")]
    public int? RadControlId { get; set; }

    [Column("RAD_OES")]
    public int? RadOes { get; set; }

    [Column("RAD_DES")]
    public int? RadDes { get; set; }

    [Column("RAD_ControlRating")]
    public int? RadControlRating { get; set; }

    [Column("RAD_ChecksID")]
    public int? RadChecksId { get; set; }

    [Column("RAD_ResidualRiskRating")]
    public int? RadResidualRiskRating { get; set; }

    [Column("RAD_Remarks")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? RadRemarks { get; set; }

    [Column("RAD_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? RadIpaddress { get; set; }

    [Column("RAD_CompID")]
    public int? RadCompId { get; set; }
}
