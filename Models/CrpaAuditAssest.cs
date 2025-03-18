using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class CrpaAuditAssest
{
    public int? CaPkid { get; set; }

    public string? CaAsgNo { get; set; }

    public int? CaFinancialYear { get; set; }

    public int? CaLocationid { get; set; }

    public int? CaSectionid { get; set; }

    public DateTime? CaDate { get; set; }

    public string? CaNameOfOpsHead { get; set; }

    public string? CaAddress { get; set; }

    public string? CaNameOfUnitPresident { get; set; }

    public string? CaAuditorname { get; set; }

    public float? CaNetScore { get; set; }

    public int? CaCrBy { get; set; }

    public DateTime? CaCrOn { get; set; }

    public int? CaUpdatedBy { get; set; }

    public DateTime? CaUpdatedOn { get; set; }

    public int? CaAsubmittedBy { get; set; }

    public DateTime? CaAsubmittedOn { get; set; }

    public int? CaBsubmittedBy { get; set; }

    public DateTime? CaBsubmittedOn { get; set; }

    public string? CaStatus { get; set; }

    public string? CaIpaddress { get; set; }

    public int? CaCompId { get; set; }
}
