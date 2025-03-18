using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class WfInwardMaster
{
    public int? WimPkid { get; set; }

    public string? WimInwardNo { get; set; }

    public int? WimMonthId { get; set; }

    public int? WimYearId { get; set; }

    public DateTime? WimInwardDate { get; set; }

    public string? WimInwardTime { get; set; }

    public string? WimTitle { get; set; }

    public string? WimDocReferenceno { get; set; }

    public DateTime? WimDocRecievedDate { get; set; }

    public DateTime? WimDateOnDocument { get; set; }

    public int? WimReceiptMode { get; set; }

    public int? WimDeptartment { get; set; }

    public int? WimCustomer { get; set; }

    public string? WimContactPerson { get; set; }

    public string? WimContactEmailId { get; set; }

    public string? WimContactPhNo { get; set; }

    public string? WimRemarks { get; set; }

    public int? WimAttachId { get; set; }

    public string? WimStatus { get; set; }

    public string? WimDelflag { get; set; }

    public int? WimInwardOrWorkFlow { get; set; }

    public string? WimAddress { get; set; }

    public int? WimStage { get; set; }

    public DateTime? WimCreatedOn { get; set; }

    public int? WimCreatedBy { get; set; }

    public DateTime? WimUpdatedOn { get; set; }

    public int? WimUpdatedBy { get; set; }

    public int? WimApprovedBy { get; set; }

    public DateTime? WimApprovedOn { get; set; }

    public DateTime? WimDeletedOn { get; set; }

    public int? WimDeletedBy { get; set; }

    public DateTime? WimRecalledOn { get; set; }

    public int? WimRecalledBy { get; set; }

    public string? WimIpadress { get; set; }

    public int? WimCompId { get; set; }

    public int? WimWorkFlowCreatedBy { get; set; }

    public DateTime? WimWorkFlowCreatedOn { get; set; }

    public int? WimWorkFlowId { get; set; }

    public int? WimWorkFlowArchiveId { get; set; }

    public string? WimWorkFlowComments { get; set; }

    public int? WimProgressStatus { get; set; }
}
