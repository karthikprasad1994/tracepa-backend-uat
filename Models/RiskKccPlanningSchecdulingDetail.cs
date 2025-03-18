using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class RiskKccPlanningSchecdulingDetail
{
    public int? KccPkid { get; set; }

    public int? KccCustId { get; set; }

    public string? KccAsgNo { get; set; }

    public string? KccRiskReportReferenceNo { get; set; }

    public int? KccFunId { get; set; }

    public int? KccSubFunId { get; set; }

    public int? KccYearId { get; set; }

    public string? KccTitle { get; set; }

    public string? KccScope { get; set; }

    public DateTime? KccScheduleStartDate { get; set; }

    public DateTime? KccScheduleClosure { get; set; }

    public int? KccReviewerTypeId { get; set; }

    public int? KccReviewerId { get; set; }

    public int? KccCrBy { get; set; }

    public DateTime? KccCrOn { get; set; }

    public int? KccUpdatedBy { get; set; }

    public DateTime? KccUpdatedOn { get; set; }

    public int? KccSubmittedBy { get; set; }

    public DateTime? KccSubmittedOn { get; set; }

    public string? KccIpaddress { get; set; }

    public DateTime? KccConductingActualStartDate { get; set; }

    public DateTime? KccConductingActualClosure { get; set; }

    public string? KccConductingRemarks { get; set; }

    public int? KccConductingCrBy { get; set; }

    public DateTime? KccConductingCrOn { get; set; }

    public int? KccConductingUpdatedBy { get; set; }

    public DateTime? KccConductingUpdatedOn { get; set; }

    public int? KccConductingSubmittedBy { get; set; }

    public DateTime? KccConductingSubmittedOn { get; set; }

    public string? KccConductingIpaddress { get; set; }

    public int? KccCompId { get; set; }

    public string? KccConductingStatus { get; set; }

    public string? KccConductingKccstatus { get; set; }

    public string? KccStatus { get; set; }

    public int? KccPlanningAttachId { get; set; }

    public int? KccConductAttachId { get; set; }

    public int? KccPlanningPgedetailId { get; set; }

    public int? KccConductPgedetailId { get; set; }
}
