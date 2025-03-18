using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class StandardAuditAuditCompletion
{
    public int? SacId { get; set; }

    public int? SacCustId { get; set; }

    public int? SacYearId { get; set; }

    public int? SacAuditId { get; set; }

    public int? SacCheckPointId { get; set; }

    public int? SacSubPointId { get; set; }

    public string? SacRemarks { get; set; }

    public int? SacWorkPaperId { get; set; }

    public int? SacAttachmentId { get; set; }

    public int? SacCreatedBy { get; set; }

    public DateTime? SacCreatedOn { get; set; }

    public int? SacUpdatedBy { get; set; }

    public DateTime? SacUpdatedOn { get; set; }

    public int? SacCompId { get; set; }

    public string? SacIpaddress { get; set; }
}
