using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class RiskBrrissueTrackerHistory
{
    public int? BrrithPkid { get; set; }

    public int? BrrithBbritpkid { get; set; }

    public int? BrrithCustId { get; set; }

    public int? BrrithAsgNo { get; set; }

    public string? BrrithActionPlan { get; set; }

    public DateTime? BrrithTargetDate { get; set; }

    public int? BrrithOpenCloseStatus { get; set; }

    public string? BrrithRemarks { get; set; }

    public int? BrrithCrBy { get; set; }

    public DateTime? BrrithCrOn { get; set; }

    public int? BrrithUpdatedBy { get; set; }

    public DateTime? BrrithUpdatedOn { get; set; }

    public string? BrrithIpaddress { get; set; }

    public int? BrrithCompId { get; set; }
}
