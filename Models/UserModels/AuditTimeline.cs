using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Audit_Timeline")]
public partial class AuditTimeline
{
    [Column("AT_ID")]
    public int? AtId { get; set; }

    [Column("AT_CustId")]
    public int? AtCustId { get; set; }

    [Column("AT_AuditId")]
    public int? AtAuditId { get; set; }

    [Column("AT_Heading")]
    public int? AtHeading { get; set; }

    [Column("AT_CheckpointId")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? AtCheckpointId { get; set; }

    [Column("AT_EmpId")]
    public int? AtEmpId { get; set; }

    [Column("AT_WorkType")]
    public int? AtWorkType { get; set; }

    [Column("AT_HrPrDay")]
    [StringLength(800)]
    [Unicode(false)]
    public string? AtHrPrDay { get; set; }

    [Column("AT_StartDate", TypeName = "datetime")]
    public DateTime? AtStartDate { get; set; }

    [Column("AT_EndDate", TypeName = "datetime")]
    public DateTime? AtEndDate { get; set; }

    [Column("AT_TotalHr")]
    [StringLength(800)]
    [Unicode(false)]
    public string? AtTotalHr { get; set; }

    [Column("AT_Comments")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? AtComments { get; set; }

    [Column("AT_DELFLG")]
    [StringLength(1)]
    [Unicode(false)]
    public string? AtDelflg { get; set; }

    [Column("AT_CRON", TypeName = "datetime")]
    public DateTime? AtCron { get; set; }

    [Column("AT_CRBY")]
    public int? AtCrby { get; set; }

    [Column("AT_APPROVEDBY")]
    public int? AtApprovedby { get; set; }

    [Column("AT_APPROVEDON", TypeName = "datetime")]
    public DateTime? AtApprovedon { get; set; }

    [Column("AT_STATUS")]
    [StringLength(2)]
    [Unicode(false)]
    public string? AtStatus { get; set; }

    [Column("AT_UPDATEDBY")]
    public int? AtUpdatedby { get; set; }

    [Column("AT_UPDATEDON", TypeName = "datetime")]
    public DateTime? AtUpdatedon { get; set; }

    [Column("AT_DELETEDBY")]
    public int? AtDeletedby { get; set; }

    [Column("AT_DELETEDON", TypeName = "datetime")]
    public DateTime? AtDeletedon { get; set; }

    [Column("AT_RECALLBY")]
    public int? AtRecallby { get; set; }

    [Column("AT_RECALLON", TypeName = "datetime")]
    public DateTime? AtRecallon { get; set; }

    [Column("AT_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? AtIpaddress { get; set; }

    [Column("AT_CompId")]
    public int? AtCompId { get; set; }
}
