using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class MstRiskLibrary
{
    public int? MrlPkid { get; set; }

    public string? MrlRiskName { get; set; }

    public string? MrlRiskDesc { get; set; }

    public string? MrlCode { get; set; }

    public int? MrlRiskTypeId { get; set; }

    public int? MrlIsKey { get; set; }

    public string? MrlDelFlag { get; set; }

    public string? MrlStatus { get; set; }

    public int? MrlCrBy { get; set; }

    public DateTime? MrlCrOn { get; set; }

    public int? MrlUpdatedBy { get; set; }

    public DateTime? MrlUpdatedOn { get; set; }

    public int? MrlApprovedBy { get; set; }

    public DateTime? MrlApprovedOn { get; set; }

    public int? MrlDeletedBy { get; set; }

    public DateTime? MrlDeletedOn { get; set; }

    public int? MrlRecallBy { get; set; }

    public DateTime? MrlRecallOn { get; set; }

    public string? MrlIpaddress { get; set; }

    public int? MrlCompId { get; set; }

    public int? MrlInherentRiskId { get; set; }

    public string? MrlModule { get; set; }
}
