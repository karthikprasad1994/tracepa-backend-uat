using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Audit_WorkPaper_History")]
public partial class AuditWorkPaperHistory
{
    [Column("AWPH_PKID")]
    public int? AwphPkid { get; set; }

    [Column("AWPH_WPID")]
    public int? AwphWpid { get; set; }

    [Column("AWPH_AuditID")]
    public int? AwphAuditId { get; set; }

    [Column("AWPH_CustID")]
    public int? AwphCustId { get; set; }

    [Column("AWPH_FunctionID")]
    public int? AwphFunctionId { get; set; }

    [Column("AWPH_ReviewerRemarks")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? AwphReviewerRemarks { get; set; }

    [Column("AWPH_AuditorRemarks")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? AwphAuditorRemarks { get; set; }

    [Column("AWPH_RRCrBy")]
    public int? AwphRrcrBy { get; set; }

    [Column("AWPH_RRCrOn", TypeName = "datetime")]
    public DateTime? AwphRrcrOn { get; set; }

    [Column("AWPH_ARCrBy")]
    public int? AwphArcrBy { get; set; }

    [Column("AWPH_ARCrOn", TypeName = "datetime")]
    public DateTime? AwphArcrOn { get; set; }

    [Column("AWPH_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? AwphIpaddress { get; set; }

    [Column("AWPH_CompID")]
    public int? AwphCompId { get; set; }
}
