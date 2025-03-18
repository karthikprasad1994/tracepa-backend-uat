using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class AdoBatch
{
    public int? BtId { get; set; }

    public int? BtCustomerId { get; set; }

    public int? BtTransactionType { get; set; }

    public int? BtBatchId { get; set; }

    public string? BtBatchNo { get; set; }

    public int? BtAttachId { get; set; }

    public string? BtTitle { get; set; }

    public int? BtNft { get; set; }

    public string? BtVouchers { get; set; }

    public DateTime? BtDate { get; set; }

    public string? BtComments { get; set; }

    public decimal? BtDebitTotal { get; set; }

    public decimal? BtCreditTotal { get; set; }

    public string? BtDelflag { get; set; }

    public string? BtStatus { get; set; }

    public int? BtCompId { get; set; }

    public int? BtYearId { get; set; }

    public int? BtCrBy { get; set; }

    public DateTime? BtCrOn { get; set; }

    public string? BtIpaddress { get; set; }
}
