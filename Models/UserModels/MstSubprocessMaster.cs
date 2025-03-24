using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("MST_SUBPROCESS_MASTER")]
public partial class MstSubprocessMaster
{
    [Column("SPM_ID")]
    public int? SpmId { get; set; }

    [Column("SPM_ENT_ID")]
    public int? SpmEntId { get; set; }

    [Column("SPM_SEM_ID")]
    public int? SpmSemId { get; set; }

    [Column("SPM_PM_ID")]
    public int? SpmPmId { get; set; }

    [Column("SPM_CODE")]
    [StringLength(20)]
    [Unicode(false)]
    public string? SpmCode { get; set; }

    [Column("SPM_NAME")]
    [StringLength(500)]
    [Unicode(false)]
    public string? SpmName { get; set; }

    [Column("SPM_Desc")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? SpmDesc { get; set; }

    [Column("SPM_IsKey")]
    public int? SpmIsKey { get; set; }

    [Column("SPM_DELFLG")]
    [StringLength(2)]
    [Unicode(false)]
    public string? SpmDelflg { get; set; }

    [Column("SPM_STATUS")]
    [StringLength(2)]
    [Unicode(false)]
    public string? SpmStatus { get; set; }

    [Column("SPM_CRBY")]
    public int? SpmCrby { get; set; }

    [Column("SPM_CRON", TypeName = "datetime")]
    public DateTime? SpmCron { get; set; }

    [Column("SPM_UPDATEDBY")]
    public int? SpmUpdatedby { get; set; }

    [Column("SPM_UPDATEDON", TypeName = "datetime")]
    public DateTime? SpmUpdatedon { get; set; }

    [Column("SPM_DELETEDBY")]
    public int? SpmDeletedby { get; set; }

    [Column("SPM_DELETEDON", TypeName = "datetime")]
    public DateTime? SpmDeletedon { get; set; }

    [Column("SPM_APPROVEDBY")]
    public int? SpmApprovedby { get; set; }

    [Column("SPM_APPROVEDON", TypeName = "datetime")]
    public DateTime? SpmApprovedon { get; set; }

    [Column("SPM_RECALLBY")]
    public int? SpmRecallby { get; set; }

    [Column("SPM_RECALLON", TypeName = "datetime")]
    public DateTime? SpmRecallon { get; set; }

    [Column("SPM_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? SpmIpaddress { get; set; }

    [Column("SPM_CompID")]
    public int? SpmCompId { get; set; }
}
