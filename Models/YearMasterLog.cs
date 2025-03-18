using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class YearMasterLog
{
    public int LogPkid { get; set; }

    public string? LogOperation { get; set; }

    public DateTime? LogDate { get; set; }

    public int? LogUserId { get; set; }

    public decimal? YmsYearid { get; set; }

    public DateTime? YmsFromdate { get; set; }

    public DateTime? NYmsFromdate { get; set; }

    public DateTime? YmsTodate { get; set; }

    public DateTime? NYmsTodate { get; set; }

    public string? YmsId { get; set; }

    public string? NYmsId { get; set; }

    public int? YmsDefault { get; set; }

    public int? NYmsDefault { get; set; }

    public string? YmsIpaddress { get; set; }

    public string? YmsCompId { get; set; }
}
