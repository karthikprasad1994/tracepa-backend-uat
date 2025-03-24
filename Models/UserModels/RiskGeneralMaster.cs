using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Risk_GeneralMaster")]
public partial class RiskGeneralMaster
{
    [Column("RAM_PKID")]
    public int? RamPkid { get; set; }

    [Column("RAM_YearID")]
    public int? RamYearId { get; set; }

    [Column("RAM_Code")]
    [StringLength(20)]
    [Unicode(false)]
    public string? RamCode { get; set; }

    [Column("RAM_Category")]
    [StringLength(20)]
    [Unicode(false)]
    public string? RamCategory { get; set; }

    [Column("RAM_Name")]
    [StringLength(200)]
    [Unicode(false)]
    public string? RamName { get; set; }

    [Column("RAM_Remarks")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? RamRemarks { get; set; }

    [Column("RAM_Score")]
    public int? RamScore { get; set; }

    [Column("RAM_StartValue")]
    public double? RamStartValue { get; set; }

    [Column("RAM_EndValue")]
    public double? RamEndValue { get; set; }

    [Column("RAM_Color")]
    [StringLength(50)]
    [Unicode(false)]
    public string? RamColor { get; set; }

    [Column("RAM_DelFlag")]
    [StringLength(2)]
    [Unicode(false)]
    public string? RamDelFlag { get; set; }

    [Column("RAM_Status")]
    [StringLength(2)]
    [Unicode(false)]
    public string? RamStatus { get; set; }

    [Column("RAM_CrBy")]
    public int? RamCrBy { get; set; }

    [Column("RAM_CrOn", TypeName = "datetime")]
    public DateTime? RamCrOn { get; set; }

    [Column("RAM_UpdatedBy")]
    public int? RamUpdatedBy { get; set; }

    [Column("RAM_UpdatedOn", TypeName = "datetime")]
    public DateTime? RamUpdatedOn { get; set; }

    [Column("RAM_ApprovedBy")]
    public int? RamApprovedBy { get; set; }

    [Column("RAM_ApprovedOn", TypeName = "datetime")]
    public DateTime? RamApprovedOn { get; set; }

    [Column("RAM_DeletedBy")]
    public int? RamDeletedBy { get; set; }

    [Column("RAM_DeletedOn", TypeName = "datetime")]
    public DateTime? RamDeletedOn { get; set; }

    [Column("RAM_RecallBy")]
    public int? RamRecallBy { get; set; }

    [Column("RAM_RecallOn", TypeName = "datetime")]
    public DateTime? RamRecallOn { get; set; }

    [Column("RAM_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? RamIpaddress { get; set; }

    [Column("RAM_CompID")]
    public int? RamCompId { get; set; }
}
