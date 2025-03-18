using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class RiskGeneralMaster
{
    public int? RamPkid { get; set; }

    public int? RamYearId { get; set; }

    public string? RamCode { get; set; }

    public string? RamCategory { get; set; }

    public string? RamName { get; set; }

    public string? RamRemarks { get; set; }

    public int? RamScore { get; set; }

    public double? RamStartValue { get; set; }

    public double? RamEndValue { get; set; }

    public string? RamColor { get; set; }

    public string? RamDelFlag { get; set; }

    public string? RamStatus { get; set; }

    public int? RamCrBy { get; set; }

    public DateTime? RamCrOn { get; set; }

    public int? RamUpdatedBy { get; set; }

    public DateTime? RamUpdatedOn { get; set; }

    public int? RamApprovedBy { get; set; }

    public DateTime? RamApprovedOn { get; set; }

    public int? RamDeletedBy { get; set; }

    public DateTime? RamDeletedOn { get; set; }

    public int? RamRecallBy { get; set; }

    public DateTime? RamRecallOn { get; set; }

    public string? RamIpaddress { get; set; }

    public int? RamCompId { get; set; }
}
