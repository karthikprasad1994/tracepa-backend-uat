using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Acc_AssetMaster")]
public partial class AccAssetMaster
{
    [Column("AM_ID")]
    public int? AmId { get; set; }

    [Column("AM_Description")]
    [StringLength(500)]
    [Unicode(false)]
    public string? AmDescription { get; set; }

    [Column("AM_Code")]
    [StringLength(500)]
    [Unicode(false)]
    public string? AmCode { get; set; }

    [Column("AM_LevelCode")]
    public int? AmLevelCode { get; set; }

    [Column("AM_ParentID")]
    public int? AmParentId { get; set; }

    [Column("AM_WDVITAct", TypeName = "money")]
    public decimal? AmWdvitact { get; set; }

    [Column("AM_ITRate", TypeName = "money")]
    public decimal? AmItrate { get; set; }

    [Column("AM_ResidualValue")]
    public int? AmResidualValue { get; set; }

    [Column("AM_CreatedBy")]
    public int? AmCreatedBy { get; set; }

    [Column("AM_CreatedOn", TypeName = "datetime")]
    public DateTime? AmCreatedOn { get; set; }

    [Column("AM_UpdatedBy")]
    public int? AmUpdatedBy { get; set; }

    [Column("AM_UpdatedOn", TypeName = "datetime")]
    public DateTime? AmUpdatedOn { get; set; }

    [Column("AM_DelFlag")]
    [StringLength(1)]
    [Unicode(false)]
    public string? AmDelFlag { get; set; }

    [Column("AM_Status")]
    [StringLength(50)]
    [Unicode(false)]
    public string? AmStatus { get; set; }

    [Column("AM_YearID")]
    public int? AmYearId { get; set; }

    [Column("AM_CompID")]
    public int? AmCompId { get; set; }

    [Column("AM_CustId")]
    public int? AmCustId { get; set; }

    [Column("AM_ApprovedBy")]
    public int? AmApprovedBy { get; set; }

    [Column("AM_ApprovedOn", TypeName = "datetime")]
    public DateTime? AmApprovedOn { get; set; }

    [Column("AM_Opeartion")]
    [StringLength(1)]
    [Unicode(false)]
    public string? AmOpeartion { get; set; }

    [Column("AM_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? AmIpaddress { get; set; }

    [Column("AM_OriginalCost", TypeName = "money")]
    public decimal? AmOriginalCost { get; set; }
}
