using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class SadReportGeneration
{
    public int? RgId { get; set; }

    public int? RgCustomerId { get; set; }

    public int? RgSignedby { get; set; }

    public int? RgYearId { get; set; }

    public int? RgReportType { get; set; }

    public int? RgModule { get; set; }

    public int? RgReport { get; set; }

    public int? RgHeading { get; set; }

    public string? RgDescription { get; set; }

    public int? RgCrBy { get; set; }

    public DateTime? RgCrOn { get; set; }

    public int? RgUpdatedBy { get; set; }

    public DateTime? RgUpdatedOn { get; set; }

    public string? RgIpaddress { get; set; }

    public int? RgFinancialYear { get; set; }

    public int? RgCompid { get; set; }

    public int? RgPartner { get; set; }

    public int? RgAuditId { get; set; }

    public string? RgUdin { get; set; }

    public DateTime? RgUdindate { get; set; }
}
