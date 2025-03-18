using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class CmacheckMaster
{
    public int CmId { get; set; }

    public int? CmFunctionId { get; set; }

    public int? CmAreaId { get; set; }

    public string? CmRiskCategory { get; set; }

    public double? CmRiskWeight { get; set; }

    public string? CmCheckPointNo { get; set; }

    public string? CmCheckPoint { get; set; }

    public int? CmMethodologyId { get; set; }

    public string? CmDelflag { get; set; }

    public int? CmSampleSize { get; set; }

    public string? CmAreaNo { get; set; }

    public int? CmYearId { get; set; }

    public string? CmFunType { get; set; }

    public string? CmStatus { get; set; }

    public int? CmCrBy { get; set; }

    public DateTime? CmCrOn { get; set; }

    public int? CmUpdatedBy { get; set; }

    public DateTime? CmUpdatedOn { get; set; }

    public int? CmApprovedBy { get; set; }

    public DateTime? CmApprovedOn { get; set; }

    public int? CmDeletedBy { get; set; }

    public DateTime? CmDeletedOn { get; set; }

    public int? CmRecallBy { get; set; }

    public DateTime? CmRecallOn { get; set; }

    public string? CmIpaddress { get; set; }

    public int? CmCompId { get; set; }
}
