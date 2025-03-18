using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class SadIssueKnowledgeBaseMaster
{
    public int? IkbId { get; set; }

    public string? IkbIssueHeading { get; set; }

    public string? IkbIssueDetails { get; set; }

    public int? IkbIssueRatingId { get; set; }

    public string? IkbDelFlag { get; set; }

    public int? IkbCrBy { get; set; }

    public DateTime? IkbCrOn { get; set; }

    public int? IkbUpdatedBy { get; set; }

    public DateTime? IkbUpdatedOn { get; set; }

    public string? IkbStatus { get; set; }

    public int? IkbApprovedBy { get; set; }

    public DateTime? IkbApprovedOn { get; set; }

    public string? IkbIpaddress { get; set; }

    public int? IkbCompId { get; set; }
}
