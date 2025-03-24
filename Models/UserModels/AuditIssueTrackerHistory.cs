using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Audit_IssueTracker_History")]
public partial class AuditIssueTrackerHistory
{
    [Column("AITH_PKID")]
    public int? AithPkid { get; set; }

    [Column("AITH_IssuePKID")]
    public int? AithIssuePkid { get; set; }

    [Column("AITH_AuditID")]
    public int? AithAuditId { get; set; }

    [Column("AITH_CustID")]
    public int? AithCustId { get; set; }

    [Column("AITH_FunctionID")]
    public int? AithFunctionId { get; set; }

    [Column("AITH_ReviewerRemarks")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? AithReviewerRemarks { get; set; }

    [Column("AITH_AuditorRemarks")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? AithAuditorRemarks { get; set; }

    [Column("AITH_RRCrBy")]
    public int? AithRrcrBy { get; set; }

    [Column("AITH_RRCrOn", TypeName = "datetime")]
    public DateTime? AithRrcrOn { get; set; }

    [Column("AITH_ARCrBy")]
    public int? AithArcrBy { get; set; }

    [Column("AITH_ARCrOn", TypeName = "datetime")]
    public DateTime? AithArcrOn { get; set; }

    [Column("AITH_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? AithIpaddress { get; set; }

    [Column("AITH_CompID")]
    public int? AithCompId { get; set; }
}
