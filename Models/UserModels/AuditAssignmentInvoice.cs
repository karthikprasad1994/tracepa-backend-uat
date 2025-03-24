using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("AuditAssignment_Invoice")]
public partial class AuditAssignmentInvoice
{
    [Column("AAI_ID")]
    public int? AaiId { get; set; }

    [Column("AAI_YearID")]
    public int? AaiYearId { get; set; }

    [Column("AAI_Cust_ID")]
    public int? AaiCustId { get; set; }

    [Column("AAI_BillingEntity_ID")]
    public int? AaiBillingEntityId { get; set; }

    [Column("AAI_InvoiceNo")]
    [StringLength(50)]
    [Unicode(false)]
    public string? AaiInvoiceNo { get; set; }

    [Column("AAI_InvoiceTypeID")]
    public int? AaiInvoiceTypeId { get; set; }

    [Column("AAI_TaxType1")]
    public int? AaiTaxType1 { get; set; }

    [Column("AAI_TaxType1Percentage", TypeName = "decimal(10, 2)")]
    public decimal? AaiTaxType1Percentage { get; set; }

    [Column("AAI_TaxType2")]
    public int? AaiTaxType2 { get; set; }

    [Column("AAI_TaxType2Percentage", TypeName = "decimal(10, 2)")]
    public decimal? AaiTaxType2Percentage { get; set; }

    [Column("AAI_Notes")]
    [StringLength(5000)]
    [Unicode(false)]
    public string? AaiNotes { get; set; }

    [Column("AAI_CrBy")]
    public int? AaiCrBy { get; set; }

    [Column("AAI_CrOn", TypeName = "datetime")]
    public DateTime? AaiCrOn { get; set; }

    [Column("AAI_IPAddress")]
    [StringLength(25)]
    [Unicode(false)]
    public string? AaiIpaddress { get; set; }

    [Column("AAI_CompID")]
    public int? AaiCompId { get; set; }

    [Column("AAI_AuthorizedSignatory")]
    public int? AaiAuthorizedSignatory { get; set; }
}
