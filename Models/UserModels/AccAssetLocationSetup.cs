using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Acc_AssetLocationSetup")]
public partial class AccAssetLocationSetup
{
    [Column("LS_ID")]
    public int? LsId { get; set; }

    [Column("LS_Description")]
    [StringLength(500)]
    [Unicode(false)]
    public string? LsDescription { get; set; }

    [Column("LS_DescCode")]
    [StringLength(500)]
    [Unicode(false)]
    public string? LsDescCode { get; set; }

    [Column("LS_Code")]
    [StringLength(500)]
    [Unicode(false)]
    public string? LsCode { get; set; }

    [Column("LS_LevelCode")]
    public int? LsLevelCode { get; set; }

    [Column("LS_ParentID")]
    public int? LsParentId { get; set; }

    [Column("LS_CreatedBy")]
    public int? LsCreatedBy { get; set; }

    [Column("LS_CreatedOn", TypeName = "datetime")]
    public DateTime? LsCreatedOn { get; set; }

    [Column("LS_UpdatedBy")]
    public int? LsUpdatedBy { get; set; }

    [Column("LS_UpdatedOn", TypeName = "datetime")]
    public DateTime? LsUpdatedOn { get; set; }

    [Column("LS_DelFlag")]
    [StringLength(1)]
    [Unicode(false)]
    public string? LsDelFlag { get; set; }

    [Column("LS_Status")]
    [StringLength(50)]
    [Unicode(false)]
    public string? LsStatus { get; set; }

    [Column("LS_YearID")]
    public int? LsYearId { get; set; }

    [Column("LS_CompID")]
    public int? LsCompId { get; set; }

    [Column("LS_CustId")]
    public int? LsCustId { get; set; }

    [Column("LS_ApprovedBy")]
    public int? LsApprovedBy { get; set; }

    [Column("LS_ApprovedOn", TypeName = "datetime")]
    public DateTime? LsApprovedOn { get; set; }

    [Column("LS_Opeartion")]
    [StringLength(1)]
    [Unicode(false)]
    public string? LsOpeartion { get; set; }

    [Column("LS_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? LsIpaddress { get; set; }
}
