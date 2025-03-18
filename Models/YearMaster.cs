using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class YearMaster
{
    public string YmsId { get; set; } = null!;

    public DateTime? YmsFromdate { get; set; }

    public DateTime? YmsTodate { get; set; }

    public decimal? YmsYearid { get; set; }

    public int? YmsDefault { get; set; }

    public int? YmsCreatedBy { get; set; }

    public DateTime? YmsCreatedOn { get; set; }

    public int? YmsUpdatedBy { get; set; }

    public DateTime? YmsUpdatedOn { get; set; }

    public int? YmsApprovedBy { get; set; }

    public DateTime? YmsApprovedOn { get; set; }

    public int? YmsDeletedBy { get; set; }

    public DateTime? YmsDeletedOn { get; set; }

    public int? YmsRecalledBy { get; set; }

    public DateTime? YmsRecalledOn { get; set; }

    public string? YmsDelflag { get; set; }

    public string? YmsStatus { get; set; }

    public string? YmsIpaddress { get; set; }

    public int? YmsCompId { get; set; }
}
