using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class AuditAra
{
    public int? AraPkid { get; set; }

    public int? AraFinancialYear { get; set; }

    public int? AraAuditCodeId { get; set; }

    public int? AraFunId { get; set; }

    public int? AraCustId { get; set; }

    public float? AraNetScore { get; set; }

    public string? AraStatus { get; set; }

    public string? AraComments { get; set; }

    public int? AraCrBy { get; set; }

    public DateTime? AraCrOn { get; set; }

    public int? AraUpdatedBy { get; set; }

    public DateTime? AraUpdatedOn { get; set; }

    public int? AraSubmittedBy { get; set; }

    public DateTime? AraSubmittedOn { get; set; }

    public string? AraIpaddress { get; set; }

    public int? AraCompId { get; set; }
}
