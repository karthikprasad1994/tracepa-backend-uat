using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class AccLtMaster
{
    public int? AccJeId { get; set; }

    public string? AccJeTransactionNo { get; set; }

    public int? AccJeParty { get; set; }

    public int? AccJeLocation { get; set; }

    public int? AccJeBillType { get; set; }

    public string? AccJeBillNo { get; set; }

    public DateTime? AccJeBillDate { get; set; }

    public decimal? AccJeBillAmount { get; set; }

    public decimal? AccJeAdvanceAmount { get; set; }

    public string? AccJeAdvanceNaration { get; set; }

    public decimal? AccJeBalanceAmount { get; set; }

    public decimal? AccJeNetAmount { get; set; }

    public string? AccJePaymentNarration { get; set; }

    public string? AccJeChequeNo { get; set; }

    public DateTime? AccJeChequeDate { get; set; }

    public string? AccJeIfsccode { get; set; }

    public string? AccJeBankName { get; set; }

    public string? AccJeBranchName { get; set; }

    public int? AccJeCreatedBy { get; set; }

    public DateTime? AccJeCreatedOn { get; set; }

    public int? AccJeApprovedBy { get; set; }

    public DateTime? AccJeApprovedOn { get; set; }

    public int? AccJeDeletedBy { get; set; }

    public DateTime? AccJeDeletedOn { get; set; }

    public int? AccJeRecalledBy { get; set; }

    public DateTime? AccJeRecalledOn { get; set; }

    public int? AccJeYearId { get; set; }

    public int? AccJeCompId { get; set; }

    public string? AccJeStatus { get; set; }

    public string? AccJeOperation { get; set; }

    public string? AccJeIpaddress { get; set; }

    public DateTime? AccJeBillCreatedDate { get; set; }

    public int? AccJeBranchId { get; set; }

    public string? AccJeComnments { get; set; }

    public int? AccJeQuarterId { get; set; }
}
