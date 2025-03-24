using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("GRACe_OverallFunctionRating_Details")]
public partial class GraceOverallFunctionRatingDetail
{
    [Column("GOD_PKID")]
    public int? GodPkid { get; set; }

    [Column("GOD_YearID")]
    public int? GodYearId { get; set; }

    [Column("GOD_CustID")]
    public int? GodCustId { get; set; }

    [Column("GOD_FunID")]
    public int? GodFunId { get; set; }

    [Column("GOD_SubFunID")]
    public int? GodSubFunId { get; set; }

    [Column("GOD_RANetScore")]
    public float? GodRanetScore { get; set; }

    [Column("GOD_RANetRatingID")]
    public int? GodRanetRatingId { get; set; }

    [Column("GOD_IANetScore")]
    public float? GodIanetScore { get; set; }

    [Column("GOD_IAMNetRatingID")]
    public int? GodIamnetRatingId { get; set; }

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
