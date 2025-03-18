using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class WfOutwardMaster
{
    public int WomId { get; set; }

    public string? WomOutwardNo { get; set; }

    public int? WomMonthId { get; set; }

    public int? WomYearId { get; set; }

    public DateTime? WomOutwardDate { get; set; }

    public string? WomOutwardTime { get; set; }

    public int? WomDepartment { get; set; }

    public int? WomCustomer { get; set; }

    public int? WomInwardId { get; set; }

    public string? WomInwardRefNo { get; set; }

    public string? WomInwardName { get; set; }

    public string? WomAddress { get; set; }

    public string? WomPage { get; set; }

    public int? WomSensitivity { get; set; }

    public string? WomOutwardRefNo { get; set; }

    public int? WomDispathMode { get; set; }

    public int? WomReplyAwaited { get; set; }

    public int? WomDocumentType { get; set; }

    public string? WomMailingExpenses { get; set; }

    public string? WomAttachmentDetails { get; set; }

    public string? WomRemarks { get; set; }

    public string? WomSendTo { get; set; }

    public int? WomAttachId { get; set; }

    public int? WomCreatedBy { get; set; }

    public DateTime? WomCreatedOn { get; set; }

    public int? WomUpdatedBy { get; set; }

    public DateTime? WomUpdatedOn { get; set; }

    public int? WomApprovedBy { get; set; }

    public DateTime? WomApprovedOn { get; set; }

    public int? WomDeletedBy { get; set; }

    public DateTime? WomDeletedOn { get; set; }

    public int? WomRecalledBy { get; set; }

    public DateTime? WomRecalledOn { get; set; }

    public string? WomStatus { get; set; }

    public string? WomDelflag { get; set; }

    public int? WomCompId { get; set; }

    public string? WomIpaddress { get; set; }
}
