using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class MstMappingMaster
{
    public int? MmmId { get; set; }

    public int? MmmYearId { get; set; }

    public int? MmmFunId { get; set; }

    public int? MmmSemid { get; set; }

    public int? MmmPmid { get; set; }

    public int? MmmSpmid { get; set; }

    public int? MmmRiskid { get; set; }

    public string? MmmRisk { get; set; }

    public int? MmmControlId { get; set; }

    public string? MmmControl { get; set; }

    public int? MmmChecksId { get; set; }

    public string? MmmCheckS { get; set; }

    public int? MmmInherentRiskId { get; set; }

    public string? MmmInherentRisk { get; set; }

    public string? MmmDelFlag { get; set; }

    public string? MmmStatus { get; set; }

    public string? MmmModule { get; set; }

    public int? MmmCrBy { get; set; }

    public DateTime? MmmCrOn { get; set; }

    public int? MmmUpdatedBy { get; set; }

    public DateTime? MmmUpdatedOn { get; set; }

    public int? MmmApprovedBy { get; set; }

    public DateTime? MmmApprovedOn { get; set; }

    public int? MmmDeletedBy { get; set; }

    public DateTime? MmmDeletedOn { get; set; }

    public int? MmmRecallBy { get; set; }

    public DateTime? MmmRecallOn { get; set; }

    public string? MmmIpaddress { get; set; }

    public int? MmmCompId { get; set; }

    public int? MmmSpmkey { get; set; }

    public int? MmmRiskKey { get; set; }

    public int? MmmControlKey { get; set; }

    public int? MmmChecksKey { get; set; }

    public int? MmmCustid { get; set; }
}
