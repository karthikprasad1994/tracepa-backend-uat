using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class AccSubHeadingLedgerDesc
{
    public int? AshlId { get; set; }

    public int? AshlSubHeadingId { get; set; }

    public int? AshlCustomerId { get; set; }

    public string? AshlDescription { get; set; }

    public string? AshlDelFlag { get; set; }

    public string? AshlStatus { get; set; }

    public int? AshlCreatedBy { get; set; }

    public DateTime? AshlCreatedOn { get; set; }

    public int? AshlUpdatedBy { get; set; }

    public DateTime? AshlUpdatedOn { get; set; }

    public int? AshlApprovedBy { get; set; }

    public DateTime? AshlApprovedOn { get; set; }

    public int? AshlCompId { get; set; }

    public int? AshlYearId { get; set; }

    public string? AshlIpaddress { get; set; }

    public string? AshlOperation { get; set; }

    public int? AshlBranchId { get; set; }
}
