using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("GRACe_OverallBranchRating_Details")]
public partial class GraceOverallBranchRatingDetail
{
    [Column("GOD_PKID")]
    public int? GodPkid { get; set; }

    [Column("GOD_YearID")]
    public int? GodYearId { get; set; }

    [Column("GOD_CustID")]
    public int? GodCustId { get; set; }

    [Column("GOD_BranchID")]
    public int? GodBranchId { get; set; }

    [Column("GOD_BRRCoreProcessRatingID")]
    public int? GodBrrcoreProcessRatingId { get; set; }

    [Column("GOD_BRRCoreProcessScore")]
    public float? GodBrrcoreProcessScore { get; set; }

    [Column("GOD_BRRSupportProcessRatingID")]
    public int? GodBrrsupportProcessRatingId { get; set; }

    [Column("GOD_BRRSupportProcessScore")]
    public float? GodBrrsupportProcessScore { get; set; }

    [Column("GOD_BRRNetRatingID")]
    public int? GodBrrnetRatingId { get; set; }

    [Column("GOD_BRRNetScore")]
    public float? GodBrrnetScore { get; set; }

    [Column("GOD_BACoreProcessRatingID")]
    public int? GodBacoreProcessRatingId { get; set; }

    [Column("GOD_BACoreProcessScore")]
    public float? GodBacoreProcessScore { get; set; }

    [Column("GOD_BASupportProcessRatingID")]
    public int? GodBasupportProcessRatingId { get; set; }

    [Column("GOD_BASupportProcessScore")]
    public float? GodBasupportProcessScore { get; set; }

    [Column("GOD_BANetRatingID")]
    public int? GodBanetRatingId { get; set; }

    [Column("GOD_BANetScore")]
    public float? GodBanetScore { get; set; }

    [Column("GOD_BCMCoreProcessRatingID")]
    public int? GodBcmcoreProcessRatingId { get; set; }

    [Column("GOD_BCMCoreProcessScore")]
    public float? GodBcmcoreProcessScore { get; set; }

    [Column("GOD_BCMSupportProcessRatingID")]
    public int? GodBcmsupportProcessRatingId { get; set; }

    [Column("GOD_BCMSupportProcessScore")]
    public float? GodBcmsupportProcessScore { get; set; }

    [Column("GOD_BCMNetRatingID")]
    public int? GodBcmnetRatingId { get; set; }

    [Column("GOD_BCMNetScore")]
    public float? GodBcmnetScore { get; set; }

    [Column("GOD_CrBy")]
    public int? GodCrBy { get; set; }

    [Column("GOD_CrOn", TypeName = "datetime")]
    public DateTime? GodCrOn { get; set; }

    [Column("GOD_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? GodIpaddress { get; set; }

    [Column("GOD_CompID")]
    public int? GodCompId { get; set; }
}
