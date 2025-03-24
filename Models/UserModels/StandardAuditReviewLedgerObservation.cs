using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("StandardAudit_ReviewLedger_Observations")]
public partial class StandardAuditReviewLedgerObservation
{
    [Column("SRO_PKID")]
    public int? SroPkid { get; set; }

    [Column("SRO_AEU_ID")]
    public int? SroAeuId { get; set; }

    [Column("SRO_Level")]
    public int? SroLevel { get; set; }

    [Column("SRO_ObservationBy")]
    public int? SroObservationBy { get; set; }

    [Column("SRO_Observations")]
    [StringLength(8000)]
    [Unicode(false)]
    public string? SroObservations { get; set; }

    [Column("SRO_Date", TypeName = "datetime")]
    public DateTime? SroDate { get; set; }

    [Column("SRO_CompID")]
    public int? SroCompId { get; set; }

    [Column("SRO_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? SroIpaddress { get; set; }

    [Column("SRO_IsIssueRaised")]
    public int? SroIsIssueRaised { get; set; }

    [Column("SRO_EmailIds")]
    [StringLength(500)]
    [Unicode(false)]
    public string? SroEmailIds { get; set; }

    [Column("SRO_FinancialYearId")]
    public int? SroFinancialYearId { get; set; }

    [Column("SRO_AuditId")]
    public int? SroAuditId { get; set; }

    [Column("SRO_CustId")]
    public int? SroCustId { get; set; }

    [Column("SRO_AuditTypeId")]
    public int? SroAuditTypeId { get; set; }

    [Column("SRO_Status")]
    [StringLength(500)]
    [Unicode(false)]
    public string? SroStatus { get; set; }
}
