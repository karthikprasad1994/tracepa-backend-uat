using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class SadCustAccountingTemplate
{
    public int? CustPkid { get; set; }

    public int? CustId { get; set; }

    public string? CustDesc { get; set; }

    public string? CustValue { get; set; }

    public int? CustLocationId { get; set; }

    public string? CustDelflag { get; set; }

    public string? CustStatus { get; set; }

    public int? CustAttchId { get; set; }

    public int? CustCrBy { get; set; }

    public DateTime? CustCrOn { get; set; }

    public int? CustUpdatedBy { get; set; }

    public DateTime? CustUpdatedOn { get; set; }

    public string? CustIpaddress { get; set; }

    public int? CustCompid { get; set; }
}
