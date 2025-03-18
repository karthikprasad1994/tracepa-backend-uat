using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class RiskKri
{
    public int? KriPkid { get; set; }

    public int? KriCategoryId { get; set; }

    public int? KriRiskId { get; set; }

    public int? KriSubCategoryId { get; set; }

    public string? KriRiskDescription { get; set; }

    public int? KriPeriodId { get; set; }

    public int? KriMeasureId { get; set; }

    public int? KriAttachId { get; set; }

    public string? KriDelFlag { get; set; }

    public string? KriStatus { get; set; }

    public int? KriCrBy { get; set; }

    public DateTime? KriCrOn { get; set; }

    public int? KriDeletedBy { get; set; }

    public DateTime? KriDeletedOn { get; set; }

    public string? KriIpaddress { get; set; }

    public int? KriCompId { get; set; }

    public int? KriYearId { get; set; }
}
