using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("SAD_EmpCategory_Charges")]
public partial class SadEmpCategoryCharge
{
    [Column("EMPC_ID")]
    public int? EmpcId { get; set; }

    [Column("EMPC_YearID")]
    public int? EmpcYearId { get; set; }

    [Column("EMPC_CAT_ID")]
    public int? EmpcCatId { get; set; }

    [Column("EMPC_DAYS")]
    public int? EmpcDays { get; set; }

    [Column("EMPC_HOURS")]
    public int? EmpcHours { get; set; }

    [Column("EMPC_CHARGES", TypeName = "money")]
    public decimal? EmpcCharges { get; set; }

    [Column("EMPC_KMCharges", TypeName = "money")]
    public decimal? EmpcKmcharges { get; set; }

    [Column("EMPC_Remarks")]
    [StringLength(500)]
    [Unicode(false)]
    public string? EmpcRemarks { get; set; }

    [Column("EMPC_CreatedBy")]
    public int? EmpcCreatedBy { get; set; }

    [Column("EMPC_CreatedOn", TypeName = "datetime")]
    public DateTime? EmpcCreatedOn { get; set; }

    [Column("EMPC_UpdatedBy")]
    public int? EmpcUpdatedBy { get; set; }

    [Column("EMPC_UpdatedOn", TypeName = "datetime")]
    public DateTime? EmpcUpdatedOn { get; set; }

    [Column("EMPC_DeletedBy")]
    public int? EmpcDeletedBy { get; set; }

    [Column("EMPC_DeletedOn", TypeName = "datetime")]
    public DateTime? EmpcDeletedOn { get; set; }

    [Column("EMPC_RecalledBy")]
    public int? EmpcRecalledBy { get; set; }

    [Column("EMPC_RecalledOn", TypeName = "datetime")]
    public DateTime? EmpcRecalledOn { get; set; }

    [Column("EMPC_AppBy")]
    public int? EmpcAppBy { get; set; }

    [Column("EMPC_AppOn", TypeName = "datetime")]
    public DateTime? EmpcAppOn { get; set; }

    [Column("EMPC_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? EmpcIpaddress { get; set; }

    [Column("EMPC_DelFlag")]
    [StringLength(1)]
    [Unicode(false)]
    public string? EmpcDelFlag { get; set; }

    [Column("EMPC_Status")]
    [StringLength(10)]
    [Unicode(false)]
    public string? EmpcStatus { get; set; }

    [Column("EMPC_CompID")]
    public int? EmpcCompId { get; set; }

    [Column("EMPC_CRemarks")]
    [StringLength(500)]
    [Unicode(false)]
    public string? EmpcCremarks { get; set; }

    [Column("EMPC_CCreatedBy")]
    public int? EmpcCcreatedBy { get; set; }

    [Column("EMPC_CCreatedOn", TypeName = "datetime")]
    public DateTime? EmpcCcreatedOn { get; set; }

    [Column("EMPC_CUpdatedBy")]
    public int? EmpcCupdatedBy { get; set; }

    [Column("EMPC_CUpdatedOn", TypeName = "datetime")]
    public DateTime? EmpcCupdatedOn { get; set; }

    [Column("EMPC_CDeletedBy")]
    public int? EmpcCdeletedBy { get; set; }

    [Column("EMPC_CDeletedOn", TypeName = "datetime")]
    public DateTime? EmpcCdeletedOn { get; set; }

    [Column("EMPC_CRecalledBy")]
    public int? EmpcCrecalledBy { get; set; }

    [Column("EMPC_CRecalledOn", TypeName = "datetime")]
    public DateTime? EmpcCrecalledOn { get; set; }

    [Column("EMPC_CAppBy")]
    public int? EmpcCappBy { get; set; }

    [Column("EMPC_CAppOn", TypeName = "datetime")]
    public DateTime? EmpcCappOn { get; set; }

    [Column("EMPC_CDelFlag")]
    [StringLength(1)]
    [Unicode(false)]
    public string? EmpcCdelFlag { get; set; }

    [Column("EMPC_CStatus")]
    [StringLength(10)]
    [Unicode(false)]
    public string? EmpcCstatus { get; set; }
}
