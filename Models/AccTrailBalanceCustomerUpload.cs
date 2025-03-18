using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class AccTrailBalanceCustomerUpload
{
    public int AtbcuId { get; set; }

    public string AtbcuCode { get; set; } = null!;

    public string? AtbcuDescription { get; set; }

    public int? AtbcuCustId { get; set; }

    public decimal? AtbcuOpeningDebitAmount { get; set; }

    public decimal? AtbcuOpeningCreditAmount { get; set; }

    public decimal? AtbcuTrDebitAmount { get; set; }

    public decimal? AtbcuTrCreditAmount { get; set; }

    public decimal? AtbcuClosingDebitAmount { get; set; }

    public decimal? AtbcuClosingCreditAmount { get; set; }

    public string? AtbcuDelflg { get; set; }

    public DateTime? AtbcuCron { get; set; }

    public int? AtbcuCrby { get; set; }

    public int? AtbcuApprovedby { get; set; }

    public DateTime? AtbcuApprovedon { get; set; }

    public string? AtbcuStatus { get; set; }

    public int? AtbcuUpdatedby { get; set; }

    public DateTime? AtbcuUpdatedon { get; set; }

    public int? AtbcuDeletedby { get; set; }

    public DateTime? AtbcuDeletedon { get; set; }

    public int? AtbcuRecallby { get; set; }

    public DateTime? AtbcuRecallon { get; set; }

    public string? AtbcuIpaddress { get; set; }

    public int? AtbcuCompId { get; set; }

    public int? AtbcuYearid { get; set; }

    public decimal? AtbcuClosingTotalDebitAmount { get; set; }

    public decimal? AtbcuClosingTotalCreditAmount { get; set; }

    public string? AtbcuProgress { get; set; }

    public int? AtbcuBranchid { get; set; }

    public int? AtbcuMasId { get; set; }

    public int? AtbcuQuarterId { get; set; }
}
