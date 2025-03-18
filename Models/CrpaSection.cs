using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class CrpaSection
{
    public int CasId { get; set; }

    public string CasCode { get; set; } = null!;

    public string? CasSectionname { get; set; }

    public int? CasPoints { get; set; }

    public string? CasDesc { get; set; }

    public string CasDelflg { get; set; } = null!;

    public DateTime? CasCron { get; set; }

    public int? CasCrby { get; set; }

    public int? CasApprovedby { get; set; }

    public DateTime? CasApprovedon { get; set; }

    public string? CasStatus { get; set; }

    public int? CasUpdatedby { get; set; }

    public DateTime? CasUpdatedon { get; set; }

    public int? CasDeletedby { get; set; }

    public DateTime? CasDeletedon { get; set; }

    public int? CasRecallby { get; set; }

    public DateTime? CasRecallon { get; set; }

    public string? CasIpaddress { get; set; }

    public int? CasCompId { get; set; }

    public int? CasYearid { get; set; }
}
