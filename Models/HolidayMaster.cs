using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class HolidayMaster
{
    public int? HolYearId { get; set; }

    public DateTime? HolDate { get; set; }

    public string? HolRemarks { get; set; }

    public int? HolCreatedby { get; set; }

    public DateTime? HolCreatedOn { get; set; }

    public int? HolUpdatedBy { get; set; }

    public DateTime? HolUpdatedOn { get; set; }

    public int? HolApprovedBy { get; set; }

    public DateTime? HolApprovedOn { get; set; }

    public string? HolStatus { get; set; }

    public string? HolDelFlag { get; set; }

    public string? HolIpaddress { get; set; }

    public int? HolCompId { get; set; }
}
