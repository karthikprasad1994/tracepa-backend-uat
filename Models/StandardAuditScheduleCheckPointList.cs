using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class StandardAuditScheduleCheckPointList
{
    public int? SacId { get; set; }

    public int? SacSaId { get; set; }

    public int? SacCheckPointId { get; set; }

    public int? SacMandatory { get; set; }

    public int? SacAnnexure { get; set; }

    public string? SacRemarks { get; set; }

    public string? SacReviewerRemarks { get; set; }

    public int? SacStatus { get; set; }

    public int? SacReviewLedgerId { get; set; }

    public int? SacDrlid { get; set; }

    public int? SacSamplingId { get; set; }

    public int? SacTestResult { get; set; }

    public int? SacConductedBy { get; set; }

    public DateTime? SacLastUpdatedOn { get; set; }

    public int? SacAttachId { get; set; }

    public int? SacCrBy { get; set; }

    public DateTime? SacCrOn { get; set; }

    public string? SacIpaddress { get; set; }

    public int? SacCompId { get; set; }

    public int? SacWorkpaperId { get; set; }

    public int? SacAtchdocid { get; set; }
}
