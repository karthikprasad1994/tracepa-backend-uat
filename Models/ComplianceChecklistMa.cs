using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class ComplianceChecklistMa
{
    public int? CrcmId { get; set; }

    public int? CrcmCustId { get; set; }

    public int? CrcmFunId { get; set; }

    public int? CrcmJobId { get; set; }

    public int? CrcmAttchId { get; set; }

    public int? CrcmYearId { get; set; }

    public string? CrcmOperation { get; set; }

    public string? CrcmIpaddress { get; set; }

    public string? CrcmStatus { get; set; }

    public int? CrcmCrBy { get; set; }

    public DateTime? CrcmCrOn { get; set; }

    public int? CrcmUpdatedBy { get; set; }

    public DateTime? CrcmUpdatedOn { get; set; }

    public int? CrcmSubmittedBy { get; set; }

    public DateTime? CrcmSubmittedOn { get; set; }

    public int? CrcmCompId { get; set; }

    public int? CrcmPgedetailId { get; set; }
}
