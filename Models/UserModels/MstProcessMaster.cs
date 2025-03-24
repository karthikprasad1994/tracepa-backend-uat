using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("MST_PROCESS_MASTER")]
public partial class MstProcessMaster
{
    [Column("PM_ID")]
    public int? PmId { get; set; }

    [Column("PM_ENT_ID")]
    public int? PmEntId { get; set; }

    [Column("PM_SEM_ID")]
    public int? PmSemId { get; set; }

    [Column("PM_CODE")]
    [StringLength(20)]
    [Unicode(false)]
    public string? PmCode { get; set; }

    [Column("PM_NAME")]
    [StringLength(500)]
    [Unicode(false)]
    public string? PmName { get; set; }

    [Column("PM_Desc")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? PmDesc { get; set; }

    [Column("PM_DELFLG")]
    [StringLength(2)]
    [Unicode(false)]
    public string? PmDelflg { get; set; }

    [Column("PM_STATUS")]
    [StringLength(2)]
    [Unicode(false)]
    public string? PmStatus { get; set; }

    [Column("PM_CRBY")]
    public int? PmCrby { get; set; }

    [Column("PM_CRON", TypeName = "datetime")]
    public DateTime? PmCron { get; set; }

    [Column("PM_UPDATEDBY")]
    public int? PmUpdatedby { get; set; }

    [Column("PM_UPDATEDON", TypeName = "datetime")]
    public DateTime? PmUpdatedon { get; set; }

    [Column("PM_DELETEDBY")]
    public int? PmDeletedby { get; set; }

    [Column("PM_DELETEDON", TypeName = "datetime")]
    public DateTime? PmDeletedon { get; set; }

    [Column("PM_APPROVEDBY")]
    public int? PmApprovedby { get; set; }

    [Column("PM_APPROVEDON", TypeName = "datetime")]
    public DateTime? PmApprovedon { get; set; }

    [Column("PM_RECALLBY")]
    public int? PmRecallby { get; set; }

    [Column("PM_RECALLON", TypeName = "datetime")]
    public DateTime? PmRecallon { get; set; }

    [Column("PM_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? PmIpaddress { get; set; }

    [Column("PM_CompID")]
    public int? PmCompId { get; set; }
}
