using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class AccTransactionsDetail
{
    public int? AtdId { get; set; }

    public DateTime? AtdTransactionDate { get; set; }

    public int? AtdTrType { get; set; }

    public int? AtdLedgerId { get; set; }

    public int? AtdDbOrCr { get; set; }

    public decimal? AtdDebit { get; set; }

    public decimal? AtdCredit { get; set; }

    public int? AtdCreatedBy { get; set; }

    public DateTime? AtdCreatedOn { get; set; }

    public string? AtdStatus { get; set; }

    public int? AtdYearId { get; set; }

    public int? AtdCompId { get; set; }

    public string? AtdOperation { get; set; }

    public string? AtdIpaddress { get; set; }

    public string? AtdDelflag { get; set; }

    public int? AtdBranchId { get; set; }

    public decimal? AtdOpenDebit { get; set; }

    public decimal? AtdOpenCredit { get; set; }

    public decimal? AtdClosingDebit { get; set; }

    public decimal? AtdClosingCredit { get; set; }

    public int? AtdSeqReferenceNum { get; set; }

    public int? AtdOrgType { get; set; }

    public string? AtdBillName { get; set; }

    public int? AtdCustId { get; set; }
}
