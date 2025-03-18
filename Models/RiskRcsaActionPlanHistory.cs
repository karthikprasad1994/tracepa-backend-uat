using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class RiskRcsaActionPlanHistory
{
    public int? RahPkid { get; set; }

    public int? RahRcsapkid { get; set; }

    public int? RahCustid { get; set; }

    public int? RahFunid { get; set; }

    public int? RahFinancialYear { get; set; }

    public string? RahFactorIncrease { get; set; }

    public string? RahFactorDecrease { get; set; }

    public string? RahActionPlan { get; set; }

    public DateTime? RahTargetDate { get; set; }

    public int? RahCrBy { get; set; }

    public DateTime? RahCrOn { get; set; }

    public int? RahCompId { get; set; }
}
