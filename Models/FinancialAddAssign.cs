using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class FinancialAddAssign
{
    public int FaaId { get; set; }

    public int? FaaAccHead { get; set; }

    public int? FaaHead { get; set; }

    public int? FaaGl { get; set; }

    public int? FaaParent { get; set; }

    public string? FaaGlcode { get; set; }

    public string? FaaGldesc { get; set; }

    public string? FaaSgldesc { get; set; }

    public int? FaaCustId { get; set; }

    public int? FaaIndType { get; set; }

    public decimal? FaaObdebit { get; set; }

    public decimal? FaaObcredit { get; set; }

    public decimal? FaaTrDebit { get; set; }

    public decimal? FaaTrCredit { get; set; }

    public decimal? FaaCloseDebit { get; set; }

    public decimal? FaaCloseCredit { get; set; }

    public string? FaaComments { get; set; }

    public int? FaaNameoftheperson { get; set; }

    public int? FaaYearId { get; set; }

    public int? FaaCompId { get; set; }

    public int? FaaCreatedby { get; set; }

    public DateTime? FaaCreatedOn { get; set; }

    public string? FaaOperation { get; set; }

    public string? FaaIpaddress { get; set; }
}
