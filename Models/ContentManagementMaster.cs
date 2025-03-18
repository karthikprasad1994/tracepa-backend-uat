using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class ContentManagementMaster
{
    public int CmmId { get; set; }

    public string CmmCode { get; set; } = null!;

    public string? CmmDesc { get; set; }

    public string? CmmCategory { get; set; }

    public string? CmsRemarks { get; set; }

    public int? CmsKeyComponent { get; set; }

    public string? CmsModule { get; set; }

    public string CmmDelflag { get; set; } = null!;

    public string? CmmStatus { get; set; }

    public int? CmmUpdatedBy { get; set; }

    public DateTime? CmmUpdatedOn { get; set; }

    public int? CmmApprovedBy { get; set; }

    public DateTime? CmmApprovedOn { get; set; }

    public int? CmmDeletedBy { get; set; }

    public DateTime? CmmDeletedOn { get; set; }

    public int? CmmRecallBy { get; set; }

    public DateTime? CmmRecallOn { get; set; }

    public string? CmmIpaddress { get; set; }

    public int? CmmCompId { get; set; }

    public int? CmmRiskCategory { get; set; }

    public int? CmmCrBy { get; set; }

    public DateTime? CmmCrOn { get; set; }

    public decimal? CmmRate { get; set; }

    public string? CmmAct { get; set; }

    public string? CmmHsnsac { get; set; }

    public int? CmmAudrptType { get; set; }
}
