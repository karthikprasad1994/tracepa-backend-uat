using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class RiskIssueTrackerHistory
{
    public int? RithPkid { get; set; }

    public int? RithRitpkid { get; set; }

    public int? RithAsgNo { get; set; }

    public string? RithActionPlan { get; set; }

    public DateTime? RithTargetDate { get; set; }

    public int? RithOpenCloseStatus { get; set; }

    public int? RithManagerResponsible { get; set; }

    public int? RithIndividualResponsible { get; set; }

    public string? RithRemaks { get; set; }

    public int? RithCrBy { get; set; }

    public DateTime? RithCrOn { get; set; }

    public int? RithUpdatedBy { get; set; }

    public DateTime? RithUpdatedOn { get; set; }

    public string? RithIpaddress { get; set; }

    public int? RithCompId { get; set; }
}
