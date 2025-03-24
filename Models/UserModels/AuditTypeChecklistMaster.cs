using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("AuditType_Checklist_Master")]
public partial class AuditTypeChecklistMaster
{
    [Column("ACM_ID")]
    public int? AcmId { get; set; }

    [Column("ACM_Code")]
    [StringLength(20)]
    [Unicode(false)]
    public string? AcmCode { get; set; }

    [Column("ACM_AuditTypeID")]
    public int? AcmAuditTypeId { get; set; }

    [Column("ACM_Heading")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? AcmHeading { get; set; }

    [Column("ACM_Checkpoint")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? AcmCheckpoint { get; set; }

    [Column("ACM_DELFLG")]
    [StringLength(1)]
    [Unicode(false)]
    public string? AcmDelflg { get; set; }

    [Column("ACM_CRON", TypeName = "datetime")]
    public DateTime? AcmCron { get; set; }

    [Column("ACM_CRBY")]
    public int? AcmCrby { get; set; }

    [Column("ACM_APPROVEDBY")]
    public int? AcmApprovedby { get; set; }

    [Column("ACM_APPROVEDON", TypeName = "datetime")]
    public DateTime? AcmApprovedon { get; set; }

    [Column("ACM_STATUS")]
    [StringLength(2)]
    [Unicode(false)]
    public string? AcmStatus { get; set; }

    [Column("ACM_UPDATEDBY")]
    public int? AcmUpdatedby { get; set; }

    [Column("ACM_UPDATEDON", TypeName = "datetime")]
    public DateTime? AcmUpdatedon { get; set; }

    [Column("ACM_DELETEDBY")]
    public int? AcmDeletedby { get; set; }

    [Column("ACM_DELETEDON", TypeName = "datetime")]
    public DateTime? AcmDeletedon { get; set; }

    [Column("ACM_RECALLBY")]
    public int? AcmRecallby { get; set; }

    [Column("ACM_RECALLON", TypeName = "datetime")]
    public DateTime? AcmRecallon { get; set; }

    [Column("ACM_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? AcmIpaddress { get; set; }

    [Column("ACM_CompId")]
    public int? AcmCompId { get; set; }
}
