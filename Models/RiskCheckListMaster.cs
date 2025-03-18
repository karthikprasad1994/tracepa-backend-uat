using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class RiskCheckListMaster
{
    public int RcmId { get; set; }

    public int? RcmCustId { get; set; }

    public int? RcmFunctionId { get; set; }

    public int? RcmAreaId { get; set; }

    public string? RcmRiskCategory { get; set; }

    public double? RcmRiskWeight { get; set; }

    public string? RcmCheckPointNo { get; set; }

    public string? RcmCheckPoint { get; set; }

    public int? RcmMethodologyId { get; set; }

    public string? RcmDelflag { get; set; }

    public int? RcmSampleSize { get; set; }

    public string? RcmAreaNo { get; set; }

    public int? RcmYearId { get; set; }

    public string? RcmFunType { get; set; }

    public string? RcmStatus { get; set; }

    public int? RcmCrBy { get; set; }

    public DateTime? RcmCrOn { get; set; }

    public int? RcmUpdatedBy { get; set; }

    public DateTime? RcmUpdatedOn { get; set; }

    public int? RcmApprovedBy { get; set; }

    public DateTime? RcmApprovedOn { get; set; }

    public int? RcmDeletedBy { get; set; }

    public DateTime? RcmDeletedOn { get; set; }

    public int? RcmRecallBy { get; set; }

    public DateTime? RcmRecallOn { get; set; }

    public string? RcmIpaddress { get; set; }

    public int? RcmCompId { get; set; }
}
