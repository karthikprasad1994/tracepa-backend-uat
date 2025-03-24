using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Risk_BRRPlanning")]
public partial class RiskBrrplanning
{
    [Column("BRRP_PKID")]
    public int BrrpPkid { get; set; }

    [Column("BRRP_YearId")]
    public int? BrrpYearId { get; set; }

    [Column("BRRP_CustId")]
    public int? BrrpCustId { get; set; }

    [Column("BRRP_SalesUnitCode")]
    public int? BrrpSalesUnitCode { get; set; }

    [Column("BRRP_BranchID")]
    public int? BrrpBranchId { get; set; }

    [Column("BRRP_RegionID")]
    public int? BrrpRegionId { get; set; }

    [Column("BRRP_ZoneID")]
    public int? BrrpZoneId { get; set; }

    [Column("BRRP_RiskScore")]
    public int? BrrpRiskScore { get; set; }

    [Column("BRRP_BRRRatingID")]
    public int? BrrpBrrratingId { get; set; }

    [Column("BRRP_BCMRatingID")]
    public int? BrrpBcmratingId { get; set; }

    [Column("BRRP_IARatingID")]
    public int? BrrpIaratingId { get; set; }

    [Column("BRRP_GrossControlScore")]
    public int? BrrpGrossControlScore { get; set; }

    [Column("BRRP_NetScore")]
    public int? BrrpNetScore { get; set; }

    [Column("BRRP_AAPlan")]
    public int? BrrpAaplan { get; set; }

    [Column("BRRP_Remarks")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? BrrpRemarks { get; set; }

    [Column("BRRP_Status")]
    [StringLength(2)]
    [Unicode(false)]
    public string? BrrpStatus { get; set; }

    [Column("BRRP_DelFlag")]
    [StringLength(1)]
    [Unicode(false)]
    public string? BrrpDelFlag { get; set; }

    [Column("BRRP_CrBy")]
    public int? BrrpCrBy { get; set; }

    [Column("BRRP_CrOn", TypeName = "datetime")]
    public DateTime? BrrpCrOn { get; set; }

    [Column("BRRP_UpdatedBy")]
    public int? BrrpUpdatedBy { get; set; }

    [Column("BRRP_UpdatedOn", TypeName = "datetime")]
    public DateTime? BrrpUpdatedOn { get; set; }

    [Column("BRRP_SubmittedBy")]
    public int? BrrpSubmittedBy { get; set; }

    [Column("BRRP_SubmittedOn", TypeName = "datetime")]
    public DateTime? BrrpSubmittedOn { get; set; }

    [Column("BRRP_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? BrrpIpaddress { get; set; }

    [Column("BRRP_CompID")]
    public int? BrrpCompId { get; set; }
}
