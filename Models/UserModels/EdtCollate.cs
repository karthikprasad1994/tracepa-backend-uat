using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("edt_collate")]
public partial class EdtCollate
{
    [Column("CLT_COLLATENO", TypeName = "numeric(18, 0)")]
    public decimal? CltCollateno { get; set; }

    [Column("CLT_COLLATEREF")]
    [StringLength(200)]
    public string? CltCollateref { get; set; }

    [Column("CLT_CREATOR", TypeName = "numeric(5, 0)")]
    public decimal? CltCreator { get; set; }

    [Column("CLT_ALLOW", TypeName = "numeric(1, 0)")]
    public decimal? CltAllow { get; set; }

    [Column("CLT_CREATEDON", TypeName = "datetime")]
    public DateTime? CltCreatedon { get; set; }

    [Column("CLT_Comment")]
    [StringLength(200)]
    [Unicode(false)]
    public string? CltComment { get; set; }

    [Column("clt_Group")]
    public int? CltGroup { get; set; }

    [Column("clt_operation")]
    [StringLength(10)]
    [Unicode(false)]
    public string? CltOperation { get; set; }

    [Column("clt_operationby")]
    public int? CltOperationby { get; set; }

    [Column("CLT_DelFlag")]
    [StringLength(2)]
    [Unicode(false)]
    public string? CltDelFlag { get; set; }

    [Column("CLT_UPDATEDBY")]
    public int? CltUpdatedby { get; set; }

    [Column("CLT_UPDATEDON", TypeName = "datetime")]
    public DateTime? CltUpdatedon { get; set; }

    [Column("CLT_RECALLBY")]
    public int? CltRecallby { get; set; }

    [Column("CLT_RECALLON", TypeName = "datetime")]
    public DateTime? CltRecallon { get; set; }

    [Column("CLT_DELETEDBY")]
    public int? CltDeletedby { get; set; }

    [Column("CLT_DELETEDON", TypeName = "datetime")]
    public DateTime? CltDeletedon { get; set; }

    [Column("CLT_APPROVEDBY")]
    public int? CltApprovedby { get; set; }

    [Column("CLT_APPROVEDON", TypeName = "datetime")]
    public DateTime? CltApprovedon { get; set; }

    [Column("CLT_CompId")]
    public int? CltCompId { get; set; }

    [Column("CLT_IPAddress")]
    [StringLength(50)]
    [Unicode(false)]
    public string? CltIpaddress { get; set; }
}
