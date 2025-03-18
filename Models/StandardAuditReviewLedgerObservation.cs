using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class StandardAuditReviewLedgerObservation
{
    public int? SroPkid { get; set; }

    public int? SroAeuId { get; set; }

    public int? SroLevel { get; set; }

    public int? SroObservationBy { get; set; }

    public string? SroObservations { get; set; }

    public DateTime? SroDate { get; set; }

    public int? SroCompId { get; set; }

    public string? SroIpaddress { get; set; }

    public int? SroIsIssueRaised { get; set; }

    public string? SroEmailIds { get; set; }

    public int? SroFinancialYearId { get; set; }

    public int? SroAuditId { get; set; }

    public int? SroCustId { get; set; }

    public int? SroAuditTypeId { get; set; }

    public string? SroStatusVarchar { get; set; }

    public DateTime? SroDueDate { get; set; }

    public string? SroMailUniqueId { get; set; }

    public string? SroComplStatus { get; set; }

    public string? SroDbflag { get; set; }
}
