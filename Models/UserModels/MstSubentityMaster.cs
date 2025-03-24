using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("MST_SUBENTITY_MASTER")]
public partial class MstSubentityMaster
{
    [Column("SEM_ID")]
    public int? SemId { get; set; }

    [Column("SEM_Ent_ID")]
    public int? SemEntId { get; set; }

    [Column("SEM_CODE")]
    [StringLength(20)]
    [Unicode(false)]
    public string? SemCode { get; set; }

    [Column("SEM_NAME")]
    [StringLength(500)]
    [Unicode(false)]
    public string? SemName { get; set; }

    [Column("SEM_Desc")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? SemDesc { get; set; }

    [Column("SEM_DELFLG")]
    [StringLength(2)]
    [Unicode(false)]
    public string? SemDelflg { get; set; }

    [Column("SEM_STATUS")]
    [StringLength(2)]
    [Unicode(false)]
    public string? SemStatus { get; set; }

    [Column("SEM_CRON", TypeName = "datetime")]
    public DateTime? SemCron { get; set; }

    [Column("SEM_CRBY")]
    public int? SemCrby { get; set; }

    [Column("SEM_UPDATEDBY")]
    public int? SemUpdatedby { get; set; }

    [Column("SEM_UPDATEDON", TypeName = "datetime")]
    public DateTime? SemUpdatedon { get; set; }

    [Column("SEM_DELETEDBY")]
    public int? SemDeletedby { get; set; }

    [Column("SEM_DELETEDON", TypeName = "datetime")]
    public DateTime? SemDeletedon { get; set; }

    [Column("SEM_APPROVEDBY")]
    public int? SemApprovedby { get; set; }

    [Column("SEM_APPROVEDON", TypeName = "datetime")]
    public DateTime? SemApprovedon { get; set; }

    [Column("SEM_RECALLBY")]
    public int? SemRecallby { get; set; }

    [Column("SEM_RECALLON", TypeName = "datetime")]
    public DateTime? SemRecallon { get; set; }

    [Column("SEM_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? SemIpaddress { get; set; }

    [Column("SEM_CompID")]
    public int? SemCompId { get; set; }
}
