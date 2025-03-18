using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class QaAssessment
{
    public int? QaPkid { get; set; }

    public string? QaCode { get; set; }

    public int? QaFinancialYear { get; set; }

    public int? QaCustid { get; set; }

    public int? QaFunid { get; set; }

    public DateTime? QaStartDate { get; set; }

    public DateTime? QaEndDate { get; set; }

    public string? QaAuditorteam { get; set; }

    public string? QaAudittitle { get; set; }

    public string? QaDelflag { get; set; }

    public int? QaCrBy { get; set; }

    public DateTime? QaCrOn { get; set; }

    public int? QaUpdatedBy { get; set; }

    public DateTime? QaUpdatedOn { get; set; }

    public int? QaSavedBy { get; set; }

    public DateTime? QaSavedOn { get; set; }

    public string? QaAudstatus { get; set; }

    public string? QaWpstatus { get; set; }

    public string? QaIpaddress { get; set; }

    public int? QaCompId { get; set; }
}
