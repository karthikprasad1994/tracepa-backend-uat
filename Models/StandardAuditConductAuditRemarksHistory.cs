using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class StandardAuditConductAuditRemarksHistory
{
    public int? ScrId { get; set; }

    public int? ScrSaId { get; set; }

    public int? ScrSacId { get; set; }

    public int? ScrCheckPointId { get; set; }

    public int? ScrRemarksType { get; set; }

    public string? ScrRemarks { get; set; }

    public int? ScrRemarksBy { get; set; }

    public DateTime? ScrDate { get; set; }

    public string? ScrIpaddress { get; set; }

    public int? ScrCompId { get; set; }

    public int? ScrIsIssueRaised { get; set; }

    public string? ScrEmailIds { get; set; }

    public DateTime? ScrDueDate { get; set; }

    public string? ScrMailUniqueId { get; set; }

    public string? ScrDbflag { get; set; }

    public string? ScrComplStatus { get; set; }
}
