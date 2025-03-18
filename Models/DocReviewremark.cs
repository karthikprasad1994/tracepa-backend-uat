using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class DocReviewremark
{
    public int? DrId { get; set; }

    public int? DrCustid { get; set; }

    public int? DrDocLoeIdBranchid { get; set; }

    public int? DrDocYearid { get; set; }

    public string? DrDocStatus { get; set; }

    public string? DrDocType { get; set; }

    public DateTime? DrDate { get; set; }

    public int? DrCreatedBy { get; set; }

    public DateTime? DrCreatedOn { get; set; }

    public int? DrUpdatedBy { get; set; }

    public DateTime? DrUpdatedOn { get; set; }

    public int? DrCompId { get; set; }

    public string? DrEmailSentTo { get; set; }

    public string? DrIpaddress { get; set; }

    public string? DrObservation { get; set; }

    public string? DrDocDelflag { get; set; }
}
