using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("StandardAudit_AuditSummary_IFC")]
public partial class StandardAuditAuditSummaryIfc
{
    [Column("SAIFC_PKID")]
    public int? SaifcPkid { get; set; }

    [Column("SAIFC_SA_ID")]
    public int? SaifcSaId { get; set; }

    [Column("SAIFC_CustID")]
    public int? SaifcCustId { get; set; }

    [Column("SAIFC_YearID")]
    public int? SaifcYearId { get; set; }

    [Column("SAIFC_ReportDate", TypeName = "datetime")]
    public DateTime? SaifcReportDate { get; set; }

    [Column("SAIFC_ReportBy")]
    [StringLength(100)]
    [Unicode(false)]
    public string? SaifcReportBy { get; set; }

    [Column("SAIFC_Comments")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? SaifcComments { get; set; }

    [Column("SAIFC_ColumnCount")]
    public int? SaifcColumnCount { get; set; }

    [Column("SAIFC_AttachID")]
    public int? SaifcAttachId { get; set; }

    [Column("SAIFC_CrBy")]
    public int? SaifcCrBy { get; set; }

    [Column("SAIFC_CrOn", TypeName = "datetime")]
    public DateTime? SaifcCrOn { get; set; }

    [Column("SAIFC_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? SaifcIpaddress { get; set; }

    [Column("SAIFC_CompID")]
    public int? SaifcCompId { get; set; }
}
