using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Audit_CostBudgetDetails")]
public partial class AuditCostBudgetDetail
{
    [Column("CBD_ID")]
    public int? CbdId { get; set; }

    [Column("CBD_YearID")]
    public int? CbdYearId { get; set; }

    [Column("CBD_AuditCodeID")]
    public int? CbdAuditCodeId { get; set; }

    [Column("CBD_DescID")]
    public int? CbdDescId { get; set; }

    [Column("CBD_UserID")]
    public int? CbdUserId { get; set; }

    [Column("CBD_Per_Head")]
    public double? CbdPerHead { get; set; }

    [Column("CBD_CrBy")]
    public int? CbdCrBy { get; set; }

    [Column("CBD_CrOn", TypeName = "datetime")]
    public DateTime? CbdCrOn { get; set; }

    [Column("CBD_UpdateBy")]
    public int? CbdUpdateBy { get; set; }

    [Column("CBD_UpdatedOn", TypeName = "datetime")]
    public DateTime? CbdUpdatedOn { get; set; }

    [Column("CBD_ApprovedBy")]
    public int? CbdApprovedBy { get; set; }

    [Column("CBD_ApprovedOn", TypeName = "datetime")]
    public DateTime? CbdApprovedOn { get; set; }

    [Column("CBD_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? CbdIpaddress { get; set; }

    [Column("CBD_Status")]
    [StringLength(25)]
    [Unicode(false)]
    public string? CbdStatus { get; set; }

    [Column("CBD_CompID")]
    public int? CbdCompId { get; set; }
}
