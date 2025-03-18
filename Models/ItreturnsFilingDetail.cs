using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class ItreturnsFilingDetail
{
    public int? ItrfdId { get; set; }

    public int? ItrfdItrId { get; set; }

    public string? ItrfdItrno { get; set; }

    public int? ItrfdFinancialYearId { get; set; }

    public int? ItrfdAssessmentYearId { get; set; }

    public decimal? ItrfdServiceChargeInInr { get; set; }

    public int? ItrfdStatus { get; set; }

    public string? ItrfdInvoiceMail { get; set; }

    public int? ItrfdAssignTo { get; set; }

    public int? ItrfdBillingEntityId { get; set; }

    public int? ItrfdCrBy { get; set; }

    public DateTime? ItrfdCrOn { get; set; }

    public int? ItrfdUpdatedBy { get; set; }

    public DateTime? ItrfdUpdateOn { get; set; }

    public string? ItrfdIpaddress { get; set; }

    public int? ItrfdCompId { get; set; }

    public int? ItrfdTab { get; set; }
}
