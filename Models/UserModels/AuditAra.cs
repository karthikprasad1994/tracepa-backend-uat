using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Audit_ARA")]
public partial class AuditAra
{
    [Column("ARA_PKID")]
    public int? AraPkid { get; set; }

    [Column("ARA_FinancialYear")]
    public int? AraFinancialYear { get; set; }

    [Column("ARA_AuditCodeID")]
    public int? AraAuditCodeId { get; set; }

    [Column("ARA_FunID")]
    public int? AraFunId { get; set; }

    [Column("ARA_CustID")]
    public int? AraCustId { get; set; }

    [Column("ARA_NetScore")]
    public float? AraNetScore { get; set; }

    [Column("ARA_Status")]
    [StringLength(25)]
    [Unicode(false)]
    public string? AraStatus { get; set; }

    [Column("ARA_Comments")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? AraComments { get; set; }

    [Column("ARA_CrBy")]
    public int? AraCrBy { get; set; }

    [Column("ARA_CrOn", TypeName = "datetime")]
    public DateTime? AraCrOn { get; set; }

    [Column("ARA_UpdatedBy")]
    public int? AraUpdatedBy { get; set; }

    [Column("ARA_UpdatedOn", TypeName = "datetime")]
    public DateTime? AraUpdatedOn { get; set; }

    [Column("ARA_SubmittedBy")]
    public int? AraSubmittedBy { get; set; }

    [Column("ARA_SubmittedOn", TypeName = "datetime")]
    public DateTime? AraSubmittedOn { get; set; }

    [Column("ARA_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? AraIpaddress { get; set; }

    [Column("ARA_CompID")]
    public int? AraCompId { get; set; }
}
