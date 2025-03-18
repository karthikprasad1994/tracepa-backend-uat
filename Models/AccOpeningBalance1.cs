using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class AccOpeningBalance1
{
    public int OpnId { get; set; }

    public int OpnSerialNo { get; set; }

    public DateTime? OpnDate { get; set; }

    public int OpnAccHead { get; set; }

    public string? OpnGlcode { get; set; }

    public decimal? OpnDebitAmt { get; set; }

    public decimal? OpnCreditAmount { get; set; }

    public int OpnYearId { get; set; }

    public int OpnCreatedBy { get; set; }

    public DateTime? OpnCreatedOn { get; set; }

    public int OpnApprovedBy { get; set; }

    public DateTime? OpnApprovedOn { get; set; }

    public string? OpnStatus { get; set; }

    public int? OpnCompId { get; set; }

    public int? OpnGlId { get; set; }

    public int OpnCustType { get; set; }

    public int OpnIndType { get; set; }

    public string? OpnIpaddress { get; set; }

    public string? OpnOperation { get; set; }

    public decimal? OpnClosingBalanceDebit { get; set; }

    public decimal? OpnClosingBalanceCredit { get; set; }

    public int? OpnUpdatedBy { get; set; }

    public DateTime? OpnUpdatedOn { get; set; }
}
