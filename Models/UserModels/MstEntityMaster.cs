using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("MSt_Entity_Master")]
public partial class MstEntityMaster
{
    [Column("ENT_ID")]
    public int EntId { get; set; }

    [Column("ENT_CODE")]
    [StringLength(20)]
    [Unicode(false)]
    public string EntCode { get; set; } = null!;

    [Column("ENT_ENTITYNAME")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? EntEntityname { get; set; }

    [Column("ENT_DELFLG")]
    [StringLength(1)]
    [Unicode(false)]
    public string EntDelflg { get; set; } = null!;

    [Column("ENT_ORDER")]
    public int? EntOrder { get; set; }

    [Column("ENT_CRON", TypeName = "datetime")]
    public DateTime? EntCron { get; set; }

    [Column("ENT_CRBY")]
    public int? EntCrby { get; set; }

    [Column("ENT_APPROVEDBY")]
    public int? EntApprovedby { get; set; }

    [Column("ENT_APPROVEDON", TypeName = "datetime")]
    public DateTime? EntApprovedon { get; set; }

    [Column("ENT_RRPSTATUS")]
    [StringLength(10)]
    [Unicode(false)]
    public string? EntRrpstatus { get; set; }

    [Column("Ent_Branch")]
    [StringLength(4)]
    [Unicode(false)]
    public string? EntBranch { get; set; }

    [Column("Ent_Module")]
    public int? EntModule { get; set; }

    [Column("Ent_OrgId")]
    public int? EntOrgId { get; set; }

    [Column("ENT_KRI")]
    public int? EntKri { get; set; }

    [Column("Ent_FunOwnerID")]
    public int? EntFunOwnerId { get; set; }

    [Column("ENT_STATUS")]
    [StringLength(2)]
    [Unicode(false)]
    public string? EntStatus { get; set; }

    [Column("ENT_UPDATEDBY")]
    public int? EntUpdatedby { get; set; }

    [Column("ENT_UPDATEDON", TypeName = "datetime")]
    public DateTime? EntUpdatedon { get; set; }

    [Column("ENT_DELETEDBY")]
    public int? EntDeletedby { get; set; }

    [Column("ENT_DELETEDON", TypeName = "datetime")]
    public DateTime? EntDeletedon { get; set; }

    [Column("ENT_RECALLBY")]
    public int? EntRecallby { get; set; }

    [Column("ENT_RECALLON", TypeName = "datetime")]
    public DateTime? EntRecallon { get; set; }

    [Column("ENT_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? EntIpaddress { get; set; }

    [Column("ENT_CompId")]
    public int? EntCompId { get; set; }

    [Column("ENT_Desc")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? EntDesc { get; set; }

    [Column("Ent_FunManagerID")]
    public int? EntFunManagerId { get; set; }

    [Column("Ent_FunSPOCID")]
    public int? EntFunSpocid { get; set; }
}
