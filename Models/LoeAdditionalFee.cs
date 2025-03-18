using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class LoeAdditionalFee
{
    public int? LafId { get; set; }

    public int? LafLoeid { get; set; }

    public int? LafOtherExpensesId { get; set; }

    public int? LafCharges { get; set; }

    public string? LafCode { get; set; }

    public string? LafOtherExpensesName { get; set; }

    public string? LafDelflag { get; set; }

    public string? LafStatus { get; set; }

    public int? LafCrBy { get; set; }

    public DateTime? LafCrOn { get; set; }

    public int? LafUpdatedBy { get; set; }

    public DateTime? LafUpdatedOn { get; set; }

    public string? LafIpaddress { get; set; }

    public int? LafCompId { get; set; }
}
