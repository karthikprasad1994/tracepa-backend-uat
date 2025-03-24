using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Audit_TimeCostBudgetDetails")]
public partial class AuditTimeCostBudgetDetail
{
    [Column("ATCD_PKID")]
    public int? AtcdPkid { get; set; }

    [Column("ATCD_ATCBID")]
    public int? AtcdAtcbid { get; set; }

    [Column("ATCD_YearID")]
    public int? AtcdYearId { get; set; }

    [Column("ATCD_Type")]
    [StringLength(2)]
    [Unicode(false)]
    public string? AtcdType { get; set; }

    [Column("ATCD_TaskProcessID")]
    public int? AtcdTaskProcessId { get; set; }

    [Column("ATCD_AuditCodeID")]
    public int? AtcdAuditCodeId { get; set; }

    [Column("ATCD_UserID")]
    public int? AtcdUserId { get; set; }

    [Column("ATCD_Hours")]
    public int? AtcdHours { get; set; }

    [Column("ATCD_HoursPerDay")]
    public int? AtcdHoursPerDay { get; set; }

    [Column("ATCD_Days")]
    public int? AtcdDays { get; set; }

    [Column("ATCD_Cost")]
    public double? AtcdCost { get; set; }

    [Column("ATCD_CostPerDay")]
    public double? AtcdCostPerDay { get; set; }

    [Column("ATCD_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? AtcdIpaddress { get; set; }

    [Column("ATCD_CompID")]
    public int? AtcdCompId { get; set; }
}
