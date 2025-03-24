using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("AuditAssignmentSubTask_Master")]
public partial class AuditAssignmentSubTaskMaster
{
    [Column("AM_ID")]
    public int? AmId { get; set; }

    [Column("AM_CODE")]
    [StringLength(20)]
    [Unicode(false)]
    public string? AmCode { get; set; }

    [Column("AM_Name")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? AmName { get; set; }

    [Column("AM_AuditAssignmentID")]
    public int? AmAuditAssignmentId { get; set; }

    [Column("AM_BillingTypeID")]
    public int? AmBillingTypeId { get; set; }

    [Column("AM_Desc")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? AmDesc { get; set; }

    [Column("AM_DELFLG")]
    [StringLength(1)]
    [Unicode(false)]
    public string? AmDelflg { get; set; }

    [Column("AM_CRON", TypeName = "datetime")]
    public DateTime? AmCron { get; set; }

    [Column("AM_CRBY")]
    public int? AmCrby { get; set; }

    [Column("AM_APPROVEDBY")]
    public int? AmApprovedby { get; set; }

    [Column("AM_APPROVEDON", TypeName = "datetime")]
    public DateTime? AmApprovedon { get; set; }

    [Column("AM_STATUS")]
    [StringLength(2)]
    [Unicode(false)]
    public string? AmStatus { get; set; }

    [Column("AM_UPDATEDBY")]
    public int? AmUpdatedby { get; set; }

    [Column("AM_UPDATEDON", TypeName = "datetime")]
    public DateTime? AmUpdatedon { get; set; }

    [Column("AM_DELETEDBY")]
    public int? AmDeletedby { get; set; }

    [Column("AM_DELETEDON", TypeName = "datetime")]
    public DateTime? AmDeletedon { get; set; }

    [Column("AM_RECALLBY")]
    public int? AmRecallby { get; set; }

    [Column("AM_RECALLON", TypeName = "datetime")]
    public DateTime? AmRecallon { get; set; }

    [Column("AM_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? AmIpaddress { get; set; }

    [Column("AM_CompId")]
    public int? AmCompId { get; set; }
}
