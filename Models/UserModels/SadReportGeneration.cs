using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("SAD_ReportGeneration")]
public partial class SadReportGeneration
{
    [Column("RG_Id")]
    public int? RgId { get; set; }

    [Column("RG_CustomerId")]
    public int? RgCustomerId { get; set; }

    [Column("RG_Signedby")]
    public int? RgSignedby { get; set; }

    [Column("RG_YearId")]
    public int? RgYearId { get; set; }

    [Column("RG_ReportType")]
    public int? RgReportType { get; set; }

    [Column("RG_Module")]
    public int? RgModule { get; set; }

    [Column("RG_Report")]
    public int? RgReport { get; set; }

    [Column("RG_Heading")]
    public int? RgHeading { get; set; }

    [Column("RG_Description")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? RgDescription { get; set; }

    [Column("RG_CrBy")]
    public int? RgCrBy { get; set; }

    [Column("RG_CrOn", TypeName = "datetime")]
    public DateTime? RgCrOn { get; set; }

    [Column("RG_UpdatedBy")]
    public int? RgUpdatedBy { get; set; }

    [Column("RG_UpdatedOn", TypeName = "datetime")]
    public DateTime? RgUpdatedOn { get; set; }

    [Column("RG_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? RgIpaddress { get; set; }

    [Column("RG_FinancialYear")]
    public int? RgFinancialYear { get; set; }

    [Column("RG_Compid")]
    public int? RgCompid { get; set; }

    [Column("RG_Partner")]
    public int? RgPartner { get; set; }

    [Column("RG_AuditId")]
    public int? RgAuditId { get; set; }

    [Column("RG_UDIN")]
    [StringLength(100)]
    [Unicode(false)]
    public string? RgUdin { get; set; }

    [Column("RG_UDINdate", TypeName = "datetime")]
    public DateTime? RgUdindate { get; set; }
}
