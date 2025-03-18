using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class AccSubHeadingNoteDesc
{
    public int? AshnId { get; set; }

    public int? AshnSubHeadingId { get; set; }

    public int? AshnCustomerId { get; set; }

    public string? AshnDescription { get; set; }

    public string? AshnDelFlag { get; set; }

    public string? AshnStatus { get; set; }

    public int? AshnCreatedBy { get; set; }

    public DateTime? AshnCreatedOn { get; set; }

    public int? AshnUpdatedBy { get; set; }

    public DateTime? AshnUpdatedOn { get; set; }

    public int? AshnApprovedBy { get; set; }

    public DateTime? AshnApprovedOn { get; set; }

    public int? AshnCompId { get; set; }

    public int? AshnYearId { get; set; }

    public string? AshnIpaddress { get; set; }

    public string? AshnOperation { get; set; }
}
