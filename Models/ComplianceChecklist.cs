using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class ComplianceChecklist
{
    public int? CrcdPkId { get; set; }

    public int? CrcdMasId { get; set; }

    public int? CrcdYearId { get; set; }

    public int? CrcdSubFunId { get; set; }

    public int? CrcdPid { get; set; }

    public int? CrcdSubPid { get; set; }

    public int? CrcdRiskId { get; set; }

    public int? CrcdControlId { get; set; }

    public int? CrcdCheckId { get; set; }

    public int? CrcdAttchId { get; set; }

    public string? CrcdOperation { get; set; }

    public string? CrcdIpaddress { get; set; }

    public string? CrcdSunFunc { get; set; }

    public string? CrcdProcess { get; set; }

    public string? CrcdSunProcess { get; set; }

    public string? CrcdRisk { get; set; }

    public string? CrcdControl { get; set; }

    public string? CrcdCheckDesc { get; set; }

    public int? CrcdInherentRiskId { get; set; }

    public int? CrcdCertId { get; set; }

    public string? CrcdRiskRemarks { get; set; }

    public string? CrcdCheckRemarks { get; set; }

    public int? CrcdCompId { get; set; }
}
