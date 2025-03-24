using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Audit_TimeCostBudgetMaster")]
public partial class AuditTimeCostBudgetMaster
{
    [Column("ATCB_PKID")]
    public int? AtcbPkid { get; set; }

    [Column("ATCB_YearID")]
    public int? AtcbYearId { get; set; }

    [Column("ATCB_AuditCodeID")]
    public int? AtcbAuditCodeId { get; set; }

    [Column("ATCB_Type")]
    [StringLength(2)]
    [Unicode(false)]
    public string? AtcbType { get; set; }

    [Column("ATCB_TaskProcessID")]
    public int? AtcbTaskProcessId { get; set; }

    [Column("ATCB_TotalDays")]
    public int? AtcbTotalDays { get; set; }

    [Column("ATCB_TotalHours")]
    public int? AtcbTotalHours { get; set; }

    [Column("ATCB_TotalCost")]
    public double? AtcbTotalCost { get; set; }

    [Column("ATCB_DelFlag")]
    [StringLength(1)]
    [Unicode(false)]
    public string? AtcbDelFlag { get; set; }

    [Column("ATCB_Status")]
    [StringLength(25)]
    [Unicode(false)]
    public string? AtcbStatus { get; set; }

    [Column("ATCB_Createdby")]
    public int? AtcbCreatedby { get; set; }

    [Column("ATCB_Createdon", TypeName = "datetime")]
    public DateTime? AtcbCreatedon { get; set; }

    [Column("ATCB_Updatedby")]
    public int? AtcbUpdatedby { get; set; }

    [Column("ATCB_UpdatedOn", TypeName = "datetime")]
    public DateTime? AtcbUpdatedOn { get; set; }

    [Column("ATCB_Submittedby")]
    public int? AtcbSubmittedby { get; set; }

    [Column("ATCB_SubmittedOn", TypeName = "datetime")]
    public DateTime? AtcbSubmittedOn { get; set; }

    [Column("ATCB_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? AtcbIpaddress { get; set; }

    [Column("ATCB_CompID")]
    public int? AtcbCompId { get; set; }
}
