using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class RiskRa
{
    public int? RaPkid { get; set; }

    public string? RaAsgNo { get; set; }

    public int? RaFinancialYear { get; set; }

    public int? RaCustId { get; set; }

    public int? RaFunId { get; set; }

    public string? RaComments { get; set; }

    public float? RaNetScore { get; set; }

    public int? RaCrBy { get; set; }

    public DateTime? RaCrOn { get; set; }

    public int? RaUpdatedBy { get; set; }

    public DateTime? RaUpdatedOn { get; set; }

    public int? RaSubmittedBy { get; set; }

    public DateTime? RaSubmittedOn { get; set; }

    public int? RaReAssignBy { get; set; }

    public DateTime? RaReAssignOn { get; set; }

    public int? RaApprovedBy { get; set; }

    public DateTime? RaApprovedOn { get; set; }

    public string? RaMasterStatus { get; set; }

    public string? RaStatus { get; set; }

    public string? RaIpaddress { get; set; }

    public int? RaCompId { get; set; }
}
