using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class BatchScanTable
{
    public int? BtId { get; set; }

    public int? BtCustomerId { get; set; }

    public string? BtBatchNo { get; set; }

    public int? BtTrType { get; set; }

    public int? BtNoOfTransaction { get; set; }

    public decimal? BtDebitTotal { get; set; }

    public decimal? BtCreditTotal { get; set; }

    public string? BtDelflag { get; set; }

    public string? BtStatus { get; set; }

    public int? BtCompId { get; set; }

    public int? BtYearId { get; set; }

    public int? BtCrBy { get; set; }

    public DateTime? BtCrOn { get; set; }

    public string? BtOperation { get; set; }

    public string? BtIpaddress { get; set; }
}
