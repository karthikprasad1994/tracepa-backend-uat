using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class AuditSignOff
{
    public int? AsoPkid { get; set; }

    public int? AsoYearId { get; set; }

    public int? AsoAuditCodeId { get; set; }

    public int? AsoFunctionId { get; set; }

    public int? AsoCustId { get; set; }

    public int? AsoMasterId { get; set; }

    public int? AsoAuditRatingId { get; set; }

    public string? AsoSignOffStatus { get; set; }

    public string? AsoComments { get; set; }

    public string? AsoOverAllComments { get; set; }

    public string? AsoKeyObservation { get; set; }

    public string? AsoStatus { get; set; }

    public int? AsoCrBy { get; set; }

    public DateTime? AsoCrOn { get; set; }

    public int? AsoUpdatedBy { get; set; }

    public DateTime? AsoUpdatedOn { get; set; }

    public int? AsoSubmittedBy { get; set; }

    public DateTime? AsoSubmittedOn { get; set; }

    public int? AsoAttachId { get; set; }

    public string? AsoIpaddress { get; set; }

    public int? AsoCompId { get; set; }
}
