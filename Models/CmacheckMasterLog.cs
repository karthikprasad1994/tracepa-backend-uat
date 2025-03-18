using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class CmacheckMasterLog
{
    public int LogPkid { get; set; }

    public string? LogOperation { get; set; }

    public DateTime? LogDate { get; set; }

    public int? LogUserId { get; set; }

    public int? CmId { get; set; }

    public int? CmFunctionId { get; set; }

    public int? NCmFunctionId { get; set; }

    public int? CmAreaId { get; set; }

    public int? NCmAreaId { get; set; }

    public string? CmRiskCategory { get; set; }

    public string? NCmRiskCategory { get; set; }

    public float? CmRiskWeight { get; set; }

    public float? NCmRiskWeight { get; set; }

    public string? CmCheckPointNo { get; set; }

    public string? NCmCheckPointNo { get; set; }

    public string? CmCheckPoint { get; set; }

    public string? NCmCheckPoint { get; set; }

    public int? CmMethodologyId { get; set; }

    public int? NCmMethodologyId { get; set; }

    public int? CmSampleSize { get; set; }

    public int? NCmSampleSize { get; set; }

    public string? CmAreaNo { get; set; }

    public string? NCmAreaNo { get; set; }

    public int? CmYearId { get; set; }

    public int? NCmYearId { get; set; }

    public string? CmFunType { get; set; }

    public string? NCmFunType { get; set; }

    public string? CmIpaddress { get; set; }

    public int? CmCompId { get; set; }
}
