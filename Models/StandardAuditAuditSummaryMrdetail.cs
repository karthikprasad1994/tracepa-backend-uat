using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class StandardAuditAuditSummaryMrdetail
{
    public int? SamrPkid { get; set; }

    public int? SamrSaPkid { get; set; }

    public int? SamrCustId { get; set; }

    public int? SamrYearId { get; set; }

    public int? SamrMrid { get; set; }

    public DateTime? SamrRequestedDate { get; set; }

    public string? SamrRequestedByPerson { get; set; }

    public string? SamrRequestedRemarks { get; set; }

    public DateTime? SamrDueDateReceiveDocs { get; set; }

    public string? SamrEmailIds { get; set; }

    public DateTime? SamrResponsesReceivedDate { get; set; }

    public string? SamrResponsesDetails { get; set; }

    public string? SamrResponsesRemarks { get; set; }

    public int? SamrAttachId { get; set; }

    public int? SamrCrBy { get; set; }

    public DateTime? SamrCrOn { get; set; }

    public string? SamrIpaddress { get; set; }

    public int? SamrCompId { get; set; }

    public int CSamrPkid { get; set; }

    public string? SamrComplStatus { get; set; }

    public int? SamrAtchdocid { get; set; }

    public string? SamrDbflag { get; set; }
}
