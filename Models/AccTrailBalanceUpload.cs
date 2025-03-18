using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class AccTrailBalanceUpload
{
    public int AtbuId { get; set; }

    public string AtbuCode { get; set; } = null!;

    public string? AtbuDescription { get; set; }

    public int? AtbuCustId { get; set; }

    public int? AtbuBranchid { get; set; }

    public decimal? AtbuOpeningDebitAmount { get; set; }

    public decimal? AtbuOpeningCreditAmount { get; set; }

    public decimal? AtbuTrDebitAmount { get; set; }

    public decimal? AtbuTrCreditAmount { get; set; }

    public decimal? AtbuClosingDebitAmount { get; set; }

    public decimal? AtbuClosingCreditAmount { get; set; }

    public string? AtbuDelflg { get; set; }

    public DateTime? AtbuCron { get; set; }

    public int? AtbuCrby { get; set; }

    public int? AtbuApprovedby { get; set; }

    public DateTime? AtbuApprovedon { get; set; }

    public string? AtbuStatus { get; set; }

    public int? AtbuUpdatedby { get; set; }

    public DateTime? AtbuUpdatedon { get; set; }

    public int? AtbuDeletedby { get; set; }

    public DateTime? AtbuDeletedon { get; set; }

    public int? AtbuRecallby { get; set; }

    public DateTime? AtbuRecallon { get; set; }

    public string? AtbuIpaddress { get; set; }

    public int? AtbuCompId { get; set; }

    public int? AtbuYearid { get; set; }

    public decimal? AtbuClosingTotalDebitAmount { get; set; }

    public decimal? AtbuClosingTotalCreditAmount { get; set; }

    public string? AtbuProgress { get; set; }

    public int? AtbuQuarterId { get; set; }

    public virtual ICollection<AccTrailBalanceUploadDetail> AccTrailBalanceUploadDetails { get; set; } = new List<AccTrailBalanceUploadDetail>();
}
