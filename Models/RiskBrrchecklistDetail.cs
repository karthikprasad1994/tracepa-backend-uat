using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class RiskBrrchecklistDetail
{
    public int? BrrdPkid { get; set; }

    public int? BrrdBrrpkid { get; set; }

    public int? BrrdRcmid { get; set; }

    public int? BrrdFunctionId { get; set; }

    public int? BrrdAreaId { get; set; }

    public string? BrrdRefNo { get; set; }

    public string? BrrdCheckPoint { get; set; }

    public int? BrrdMethodologyId { get; set; }

    public string? BrrdMethodology { get; set; }

    public int? BrrdSampleSizeId { get; set; }

    public string? BrrdSampleSizeName { get; set; }

    public string? BrrdRiskCategory { get; set; }

    public int? BrrdYesnona { get; set; }

    public string? BrrdStatus { get; set; }

    public string? BrrdDelFlag { get; set; }

    public string? BrrdIssueDetails { get; set; }

    public double? BrrdOweightage { get; set; }

    public int? BrrdRiskScore { get; set; }

    public double? BrrdWeightedRiskScore { get; set; }

    public string? BrrdAnnexure { get; set; }

    public string? BrrdFunType { get; set; }

    public int? BrrdAttachId { get; set; }

    public string? BrrdIpaddress { get; set; }

    public int? BrrdCompId { get; set; }
}
