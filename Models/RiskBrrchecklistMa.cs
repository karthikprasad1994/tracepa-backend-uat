using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class RiskBrrchecklistMa
{
    public int? BrrPkid { get; set; }

    public int? BrrCustId { get; set; }

    public int? BrrAsgId { get; set; }

    public int? BrrBranchId { get; set; }

    public int? BrrYearId { get; set; }

    public DateTime? BrrAsdate { get; set; }

    public DateTime? BrrAedate { get; set; }

    public string? BrrStatus { get; set; }

    public string? BrrFlag { get; set; }

    public string? BrrTitle { get; set; }

    public string? BrrRemarks { get; set; }

    public int? BrrAttachId { get; set; }

    public int? BrrCrBy { get; set; }

    public DateTime? BrrCrOn { get; set; }

    public int? BrrUpdatedBy { get; set; }

    public DateTime? BrrUpdatedOn { get; set; }

    public int? BrrSubmittedBy { get; set; }

    public DateTime? BrrSubmittedOn { get; set; }

    public int? BrrApprovedBy { get; set; }

    public DateTime? BrrApprovedOn { get; set; }

    public string? BrrIpaddress { get; set; }

    public int? BrrCompId { get; set; }

    public int? BrrPgedetailId { get; set; }
}
