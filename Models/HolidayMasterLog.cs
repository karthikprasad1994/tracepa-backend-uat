using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class HolidayMasterLog
{
    public int LogPkid { get; set; }

    public DateTime? LogDate { get; set; }

    public string? LogOperation { get; set; }

    public int? LogUserId { get; set; }

    public int? HolYearId { get; set; }

    public DateTime? HolDate { get; set; }

    public DateTime? NHolDate { get; set; }

    public string? HolRemarks { get; set; }

    public string? NHolRemarks { get; set; }

    public string? HolCompId { get; set; }

    public string? HolIpaddress { get; set; }
}
