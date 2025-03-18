using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class AuditAssignmentInvoice
{
    public int? AaiId { get; set; }

    public int? AaiYearId { get; set; }

    public int? AaiCustId { get; set; }

    public int? AaiBillingEntityId { get; set; }

    public string? AaiInvoiceNo { get; set; }

    public int? AaiInvoiceTypeId { get; set; }

    public int? AaiTaxType1 { get; set; }

    public decimal? AaiTaxType1Percentage { get; set; }

    public int? AaiTaxType2 { get; set; }

    public decimal? AaiTaxType2Percentage { get; set; }

    public string? AaiNotes { get; set; }

    public int? AaiCrBy { get; set; }

    public DateTime? AaiCrOn { get; set; }

    public string? AaiIpaddress { get; set; }

    public int? AaiCompId { get; set; }

    public int? AaiAuthorizedSignatory { get; set; }
}
