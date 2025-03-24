using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Audit_TimeSheet")]
public partial class AuditTimeSheet
{
    [Column("TS_PKID")]
    public int? TsPkid { get; set; }

    [Column("TS_AuditCodeID")]
    public int? TsAuditCodeId { get; set; }

    [Column("TS_FunID")]
    public int? TsFunId { get; set; }

    [Column("TS_CustID")]
    public int? TsCustId { get; set; }

    [Column("TS_TaskID")]
    public int? TsTaskId { get; set; }

    [Column("TS_TaskType")]
    [StringLength(2)]
    [Unicode(false)]
    public string? TsTaskType { get; set; }

    [Column("TS_UserID")]
    public int? TsUserId { get; set; }

    [Column("TS_Date", TypeName = "datetime")]
    public DateTime? TsDate { get; set; }

    [Column("TS_Comments")]
    [StringLength(500)]
    [Unicode(false)]
    public string? TsComments { get; set; }

    [Column("TS_Hours", TypeName = "decimal(18, 0)")]
    public decimal? TsHours { get; set; }

    [Column("TS_IsApproved")]
    [StringLength(1)]
    [Unicode(false)]
    public string? TsIsApproved { get; set; }

    [Column("TS_Approver_Remarks")]
    [StringLength(2000)]
    [Unicode(false)]
    public string? TsApproverRemarks { get; set; }

    [Column("TS_DESCID")]
    public int? TsDescid { get; set; }

    [Column("TS_DELFLG")]
    [StringLength(1)]
    [Unicode(false)]
    public string? TsDelflg { get; set; }

    [Column("TS_STATUS")]
    [StringLength(2)]
    [Unicode(false)]
    public string? TsStatus { get; set; }

    [Column("TS_CRON", TypeName = "datetime")]
    public DateTime? TsCron { get; set; }

    [Column("TS_CRBY")]
    public int? TsCrby { get; set; }

    [Column("TS_UpdatedOn", TypeName = "datetime")]
    public DateTime? TsUpdatedOn { get; set; }

    [Column("TS_UpdatedBy")]
    public int? TsUpdatedBy { get; set; }

    [Column("TS_APPROVEDBY")]
    public int? TsApprovedby { get; set; }

    [Column("TS_APPROVEDON", TypeName = "datetime")]
    public DateTime? TsApprovedon { get; set; }

    [Column("TS_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? TsIpaddress { get; set; }

    [Column("TS_YearID")]
    public int? TsYearId { get; set; }

    [Column("TS_CompID")]
    public int? TsCompId { get; set; }
}
