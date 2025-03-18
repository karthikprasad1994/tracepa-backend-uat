using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class RiskRrfPlanningSchecdulingDetail
{
    public int? RpdPkid { get; set; }

    public string? RpdAsgNo { get; set; }

    public string? RpdRefNo { get; set; }

    public int? RpdCustId { get; set; }

    public int? RpdFunId { get; set; }

    public int? RpdSubFunId { get; set; }

    public string? RpdTitle { get; set; }

    public string? RpdScope { get; set; }

    public DateTime? RpdScheduleStartDate { get; set; }

    public DateTime? RpdScheduleClosure { get; set; }

    public int? RpdReviewerTypeId { get; set; }

    public int? RpdReviewerId { get; set; }

    public int? RpdCrBy { get; set; }

    public DateTime? RpdCrOn { get; set; }

    public int? RpdUpdatedBy { get; set; }

    public DateTime? RpdUpdatedOn { get; set; }

    public int? RpdSubmittedBy { get; set; }

    public DateTime? RpdSubmittedOn { get; set; }

    public string? RpdIpaddress { get; set; }

    public DateTime? RpdConductingActualStartDate { get; set; }

    public DateTime? RpdConductingActualClosure { get; set; }

    public string? RpdConductingRemarks { get; set; }

    public int? RpdConductingCrBy { get; set; }

    public DateTime? RpdConductingCrOn { get; set; }

    public int? RpdConductingUpdatedBy { get; set; }

    public DateTime? RpdConductingUpdatedOn { get; set; }

    public int? RpdConductingSubmittedBy { get; set; }

    public DateTime? RpdConductingSubmittedOn { get; set; }

    public string? RpdConductingIpaddress { get; set; }

    public int? RpdCompId { get; set; }

    public string? RpdStatus { get; set; }

    public int? RpdYearId { get; set; }

    public int? RpdPlanningAttachId { get; set; }

    public int? RpdConductAttachId { get; set; }

    public string? RpdConductingStatus { get; set; }

    public string? RpdConductingRrstatus { get; set; }

    public int? RpdPgedetailId { get; set; }

    public int? RpdConductPgedetailId { get; set; }
}
