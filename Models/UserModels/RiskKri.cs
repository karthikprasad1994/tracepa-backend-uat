using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Risk_KRI")]
public partial class RiskKri
{
    [Column("KRI_PKID")]
    public int? KriPkid { get; set; }

    [Column("KRI_CategoryID")]
    public int? KriCategoryId { get; set; }

    [Column("KRI_RiskID")]
    public int? KriRiskId { get; set; }

    [Column("KRI_SubCategoryID")]
    public int? KriSubCategoryId { get; set; }

    [Column("KRI_RiskDescription")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? KriRiskDescription { get; set; }

    [Column("KRI_PeriodID")]
    public int? KriPeriodId { get; set; }

    [Column("KRI_MeasureID")]
    public int? KriMeasureId { get; set; }

    [Column("KRI_AttachID")]
    public int? KriAttachId { get; set; }

    [Column("KRI_DelFlag")]
    [StringLength(2)]
    [Unicode(false)]
    public string? KriDelFlag { get; set; }

    [Column("KRI_STATUS")]
    [StringLength(3)]
    [Unicode(false)]
    public string? KriStatus { get; set; }

    [Column("KRI_CrBy")]
    public int? KriCrBy { get; set; }

    [Column("KRI_CrOn", TypeName = "datetime")]
    public DateTime? KriCrOn { get; set; }

    [Column("KRI_DeletedBy")]
    public int? KriDeletedBy { get; set; }

    [Column("KRI_DeletedOn", TypeName = "datetime")]
    public DateTime? KriDeletedOn { get; set; }

    [Column("KRI_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? KriIpaddress { get; set; }

    [Column("KRI_CompId")]
    public int? KriCompId { get; set; }

    [Column("KRI_YearID")]
    public int? KriYearId { get; set; }
}
