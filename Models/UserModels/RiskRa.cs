using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("Risk_RA")]
public partial class RiskRa
{
    [Column("RA_PKID")]
    public int? RaPkid { get; set; }

    [Column("RA_AsgNo")]
    [StringLength(25)]
    [Unicode(false)]
    public string? RaAsgNo { get; set; }

    [Column("RA_FinancialYear")]
    public int? RaFinancialYear { get; set; }

    [Column("RA_CustID")]
    public int? RaCustId { get; set; }

    [Column("RA_FunID")]
    public int? RaFunId { get; set; }

    [Column("RA_Comments")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? RaComments { get; set; }

    [Column("RA_NetScore")]
    public float? RaNetScore { get; set; }

    [Column("RA_CrBy")]
    public int? RaCrBy { get; set; }

    [Column("RA_CrOn", TypeName = "datetime")]
    public DateTime? RaCrOn { get; set; }

    [Column("RA_UpdatedBy")]
    public int? RaUpdatedBy { get; set; }

    [Column("RA_UpdatedOn", TypeName = "datetime")]
    public DateTime? RaUpdatedOn { get; set; }

    [Column("RA_SubmittedBy")]
    public int? RaSubmittedBy { get; set; }

    [Column("RA_SubmittedOn", TypeName = "datetime")]
    public DateTime? RaSubmittedOn { get; set; }

    [Column("RA_ReAssignBy")]
    public int? RaReAssignBy { get; set; }

    [Column("RA_ReAssignOn", TypeName = "datetime")]
    public DateTime? RaReAssignOn { get; set; }

    [Column("RA_ApprovedBy")]
    public int? RaApprovedBy { get; set; }

    [Column("RA_ApprovedOn", TypeName = "datetime")]
    public DateTime? RaApprovedOn { get; set; }

    [Column("RA_MasterStatus")]
    [StringLength(25)]
    [Unicode(false)]
    public string? RaMasterStatus { get; set; }

    [Column("RA_Status")]
    [StringLength(25)]
    [Unicode(false)]
    public string? RaStatus { get; set; }

    [Column("RA_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? RaIpaddress { get; set; }

    [Column("RA_CompID")]
    public int? RaCompId { get; set; }
}
