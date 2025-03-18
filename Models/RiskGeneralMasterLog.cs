using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class RiskGeneralMasterLog
{
    public int LogPkid { get; set; }

    public string? LogOperation { get; set; }

    public DateTime? LogDate { get; set; }

    public int? LogUserId { get; set; }

    public int? RamPkid { get; set; }

    public int? RamYearId { get; set; }

    public int? NRamYearId { get; set; }

    public string? RamCode { get; set; }

    public string? NRamCode { get; set; }

    public string? RamCategory { get; set; }

    public string? NRamCategory { get; set; }

    public string? RamName { get; set; }

    public string? NRamName { get; set; }

    public string? RamRemarks { get; set; }

    public string? NRamRemarks { get; set; }

    public int? RamScore { get; set; }

    public int? NRamScore { get; set; }

    public double? RamStartValue { get; set; }

    public double? NRamStartValue { get; set; }

    public double? RamEndValue { get; set; }

    public double? NRamEndValue { get; set; }

    public string? RamColor { get; set; }

    public string? NRamColor { get; set; }

    public string? RamIpaddress { get; set; }

    public int? RamCompId { get; set; }
}
