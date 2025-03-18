using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class AccPartnershipFirm
{
    public int? ApfId { get; set; }

    public int? ApfYearId { get; set; }

    public int? ApfCustId { get; set; }

    public int? ApfBranchId { get; set; }

    public int? ApfPartnerId { get; set; }

    public decimal? ApfOpeningBalance { get; set; }

    public decimal? ApfUnsecuredLoanTreatedAsCapital { get; set; }

    public decimal? ApfInterestOnCapital { get; set; }

    public decimal? ApfPartnersSalary { get; set; }

    public decimal? ApfShareOfprofit { get; set; }

    public decimal? ApfTransferToFixedCapital { get; set; }

    public decimal? ApfDrawings { get; set; }

    public decimal? ApfAddOthers { get; set; }

    public decimal? ApfLessOthers { get; set; }

    public int? ApfCrBy { get; set; }

    public DateTime? ApfCrOn { get; set; }

    public int? ApfUpdateBy { get; set; }

    public DateTime? ApfUpdateOn { get; set; }

    public string? ApfIpaddress { get; set; }

    public int? ApfCompId { get; set; }

    public string? ApfCapitalAmount { get; set; }
}
