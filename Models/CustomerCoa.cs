using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class CustomerCoa
{
    public int CcId { get; set; }

    public int? CcAccHead { get; set; }

    public int? CcHead { get; set; }

    public int? CcGl { get; set; }

    public int? CcParent { get; set; }

    public string? CcGlcode { get; set; }

    public string? CcGldesc { get; set; }

    public int? CcCustId { get; set; }

    public int? CcIndType { get; set; }

    public decimal? CcObdebit { get; set; }

    public decimal? CcObcredit { get; set; }

    public decimal? CcTrDebit { get; set; }

    public decimal? CcTrCredit { get; set; }

    public decimal? CcCloseDebit { get; set; }

    public decimal? CcCloseCredit { get; set; }

    public int? CcYearId { get; set; }

    public int? CcCompId { get; set; }

    public string? CcStatus { get; set; }

    public int? CcCreatedby { get; set; }

    public DateTime? CcCreatedOn { get; set; }

    public string? CcOperation { get; set; }

    public string? CcIpaddress { get; set; }
}
